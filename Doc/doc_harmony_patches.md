# 🔧 Harmony Patches — The Binding of V1

## What is Harmony?

Harmony is a library that allows you to **modify the behavior of existing methods** in a Unity game without touching the original code. It is the core of BepInEx modding.

In practice: ULTRAKILL is compiled into a `.dll` — you can't modify its code directly. Harmony lets you "hook into" its methods and execute your own code before, after, or instead of them.

---

## The three types of patches

### Prefix — Before the original method
Your code runs **before** the game's method.
Useful for: intercepting values, cancelling the original method.

```csharp
[HarmonyPatch(typeof(NewMovement), "GetHurt")]
public static class MyPatch
{
    [HarmonyPrefix]
    public static bool Prefix(ref int damage)
    {
        damage = 0; // cancel all damage
        return true; // true = the original method still runs
                     // false = the original method is cancelled
    }
}
```

### Postfix — After the original method
Your code runs **after** the game's method.
Useful for: modifying the result, applying additional effects.

```csharp
[HarmonyPatch(typeof(NewMovement), "Start")]
public static class MyPatch
{
    [HarmonyPostfix]
    public static void Postfix(NewMovement __instance)
    {
        __instance.hp -= 5; // modify HP after initialization
    }
}
```

### Transpiler — Modify IL code directly
Modifies the compiled instructions of the method.
Very powerful but complex — avoid for now.

---

## Special parameters

Harmony gives access to special parameters inside patches:

| Parameter | Description |
|---|---|
| `__instance` | The instance of the patched class (equivalent of `this`) |
| `__result` | The return value of the method (Postfix only) |
| `ref int myParam` | A parameter from the original method (exact same name) |

### Example with `__instance`
```csharp
[HarmonyPatch(typeof(NewMovement), "Start")]
public static class MyPatch
{
    [HarmonyPostfix]
    public static void Postfix(NewMovement __instance)
    {
        // __instance = the player's NewMovement object
        __instance.hp = 50;
        __instance.walkSpeed = 20f;
    }
}
```

### Example with an original method parameter
```csharp
// Original method: public void GetHurt(int damage, bool invincible, ...)
[HarmonyPatch(typeof(NewMovement), "GetHurt")]
public static class MyPatch
{
    [HarmonyPrefix]
    public static void Prefix(ref int damage)
    {
        // "damage" matches exactly the parameter name in GetHurt
        damage = damage / 2; // halve all incoming damage
    }
}
```

---

## Accessing private fields via Reflection

Some fields in ULTRAKILL classes are `private` and cannot be accessed directly. Use reflection to read or write them:

```csharp
// Read a private field
bool shootReady = (bool)typeof(Revolver)
    .GetField("shootReady", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
    .GetValue(__instance);

// Write a private field
typeof(Revolver)
    .GetField("shootReady", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
    .SetValue(__instance, true);
```

---

## How does Harmony know which method to patch?

The `[HarmonyPatch]` attribute takes two arguments:
1. **The type** (the class): `typeof(NewMovement)`
2. **The method name**: `"Start"`, `"GetHurt"`, etc.

```csharp
[HarmonyPatch(typeof(NewMovement), "Start")]
//                    ↑ class            ↑ method
```

These names come from what you see in **dnSpyEx** — that's why we use it to explore the game's code.

---

## How is Harmony activated?

In `Plugin.cs`, the line:
```csharp
Harmony.PatchAll();
```
...automatically scans all classes in your plugin that have the `[HarmonyPatch]` attribute and applies them when the mod loads.

You don't need to do anything else — write the patch, BepInEx handles the rest.

---

## Unpatching on reload (ScriptEngine)

When using ScriptEngine for hot reload, always unregister patches in `OnDestroy` to avoid duplicates:

```csharp
private void OnDestroy()
{
    Harmony.UnpatchSelf();
}
```

---

## Full example — Soda Quelconque

```csharp
// In Plugin.cs, OUTSIDE the Plugin class but INSIDE the namespace

[HarmonyPatch(typeof(NewMovement), "Start")]
public static class SodaQuelconquePatch
{
    [HarmonyPostfix]
    public static void Postfix(NewMovement __instance)
    {
        __instance.walkSpeed += 20f;  // +20 movement speed
    }
}
```

This patch runs right after `NewMovement.Start()`, i.e. when the player is initialized in the scene.

---

## Best practices

- **Always use Postfix** when modifying values after initialization
- **Use Prefix with `return false`** only when you want to completely replace a method
- **Never modify `__instance` in a Prefix** if the original method still needs it
- **Name your patches clearly** — one patch per item, one explicit name
- **Always call `Harmony.UnpatchSelf()`** in `OnDestroy` when using ScriptEngine

---

## Resources

- Harmony documentation: https://harmony.pardeike.net/articles/patching.html
- UltraModding Discord: https://discord.gg/wF8ttp8G