# 🔧 Harmony Patches — The Binding of V1

## C'est quoi Harmony ?

Harmony est une bibliothèque qui permet de **modifier le comportement de méthodes existantes** dans un jeu Unity sans toucher au code original. C'est le cœur du modding BepInEx.

Concrètement : ULTRAKILL est compilé en `.dll` — tu ne peux pas modifier son code directement. Harmony te permet de "s'accrocher" à ses méthodes et d'exécuter ton propre code avant, après, ou à la place.

---

## Les trois types de patches

### Prefix — Avant la méthode originale
Ton code s'exécute **avant** la méthode du jeu.
Utile pour : intercepter des valeurs, annuler la méthode originale.

```csharp
[HarmonyPatch(typeof(NewMovement), "GetHurt")]
public static class MonPatch
{
    [HarmonyPrefix]
    public static bool Prefix(ref int damage)
    {
        damage = 0; // annule tous les dégâts
        return true; // true = la méthode originale s'exécute quand même
                     // false = la méthode originale est annulée
    }
}
```

### Postfix — Après la méthode originale
Ton code s'exécute **après** la méthode du jeu.
Utile pour : modifier le résultat, appliquer des effets supplémentaires.

```csharp
[HarmonyPatch(typeof(NewMovement), "Start")]
public static class MonPatch
{
    [HarmonyPostfix]
    public static void Postfix(NewMovement __instance)
    {
        __instance.hp -= 5; // modifie le HP après l'initialisation
    }
}
```

### Transpiler — Modifier le code IL directement
Modifie les instructions compilées de la méthode.
Très puissant mais complexe — à éviter pour commencer.

---

## Les paramètres spéciaux

Harmony donne accès à des paramètres spéciaux dans les patches :

| Paramètre | Description |
|---|---|
| `__instance` | L'instance de la classe patchée (équivalent de `this`) |
| `__result` | La valeur de retour de la méthode (Postfix seulement) |
| `ref int monParam` | Un paramètre de la méthode originale (même nom exact) |

### Exemple avec `__instance`
```csharp
[HarmonyPatch(typeof(NewMovement), "Start")]
public static class MonPatch
{
    [HarmonyPostfix]
    public static void Postfix(NewMovement __instance)
    {
        // __instance = l'objet NewMovement du joueur
        __instance.hp = 50;
        __instance.walkSpeed = 20f;
    }
}
```

### Exemple avec un paramètre de la méthode originale
```csharp
// Méthode originale : public void GetHurt(int damage, bool invincible, ...)
[HarmonyPatch(typeof(NewMovement), "GetHurt")]
public static class MonPatch
{
    [HarmonyPrefix]
    public static void Prefix(ref int damage)
    {
        // "damage" correspond exactement au paramètre de GetHurt
        damage = damage / 2; // divise tous les dégâts par 2
    }
}
```

---

## Comment Harmony sait quelle méthode patcher ?

L'attribut `[HarmonyPatch]` prend deux arguments :
1. **Le type** (la classe) : `typeof(NewMovement)`
2. **Le nom de la méthode** : `"Start"`, `"GetHurt"`, etc.

```csharp
[HarmonyPatch(typeof(NewMovement), "Start")]
//                    ↑ classe          ↑ méthode
```

Ces noms viennent de ce que tu vois dans **dnSpyEx** — c'est pour ça qu'on l'utilise pour explorer le code du jeu.

---

## Comment Harmony est activé ?

Dans `Plugin.cs`, la ligne :
```csharp
Harmony.PatchAll();
```
...scanne automatiquement toutes les classes de ton plugin qui ont l'attribut `[HarmonyPatch]` et les applique au démarrage du mod.

Tu n'as rien d'autre à faire — tu écris le patch, BepInEx s'occupe du reste.

---

## Exemple complet — Soda Quelconque

```csharp
// Dans Plugin.cs, EN DEHORS de la classe Plugin mais DANS le namespace

[HarmonyPatch(typeof(NewMovement), "Start")]
public static class SodaQuelconquePatch
{
    [HarmonyPostfix]
    public static void Postfix(NewMovement __instance)
    {
        __instance.hp -= 5;         // -5 HP max
        __instance.walkSpeed += 5f; // +5 vitesse (affecte le slide x4)
    }
}
```

Ce patch s'exécute juste après le `Start()` de `NewMovement`, c'est-à-dire au moment où le joueur est initialisé dans la scène.

---

## Bonnes pratiques

- **Toujours utiliser Postfix** quand tu veux modifier des valeurs après initialisation
- **Utiliser Prefix avec `return false`** uniquement si tu veux complètement remplacer une méthode
- **Ne jamais modifier `__instance` dans un Prefix** si la méthode originale en a encore besoin
- **Nommer tes patches clairement** — un patch par item, un nom explicite

---

## Ressources

- Documentation Harmony : https://harmony.pardeike.net/articles/patching.html
- Discord UltraModding : https://discord.gg/wF8ttp8G
