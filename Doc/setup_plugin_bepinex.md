# 🛠️ BepInEx Plugin Setup — The Binding of V1

## Repository structure before the first build

```
E:\Modding\The-Biding-of-V1\
│
├── .gitignore                          ← Unity gitignore (already present)
├── TheBindingOfV1.csproj               ← Visual Studio project file
├── TheBindingOfV1.sln                  ← Visual Studio solution
├── Plugin.cs                           ← main mod code
├── README.md                           ← project description
│
├── Properties\
│   └── AssemblyInfo.cs                 ← assembly metadata
│
└── lib\                                ← DLL references (DO NOT commit to GitHub)
    ├── BepInEx.dll
    ├── 0Harmony.dll
    ├── Assembly-CSharp.dll
    ├── UnityEngine.dll
    ├── UnityEngine.CoreModule.dll
    └── UnityEngine.PhysicsModule.dll
```

> ⚠️ **Important** — The `lib` folder must NOT be committed to GitHub as it contains game files. Make sure your `.gitignore` excludes it. Add this line if it's not already there:
> ```
> *.dll
> ```

---

## Plugin.cs contents

```csharp
using System;
using BepInEx;
using UnityEngine;
using HarmonyLib;

namespace TheBindingOfV1
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "killi.TheBindingOfV1";
        private const string modName = "TheBindingOfV1";
        private const string modVersion = "0.1.0";

        private static readonly Harmony Harmony = new Harmony(modGUID);

        private void Awake()
        {
            Debug.Log($"Mod {modName} version {modVersion} is loading...");
            Harmony.PatchAll();
            Debug.Log($"Mod {modName} version {modVersion} is loaded!");
        }

        private void OnDestroy()
        {
            Harmony.UnpatchSelf();
            Debug.Log($"Mod {modName} unpatched!");
        }
    }
}
```

---

## Required DLL references

| File | Source |
|---|---|
| `BepInEx.dll` | `r2modman profile folder\BepInEx\core\` |
| `0Harmony.dll` | `r2modman profile folder\BepInEx\core\` |
| `Assembly-CSharp.dll` | `ULTRAKILL_Data\Managed\` |
| `UnityEngine.dll` | `ULTRAKILL_Data\Managed\` |
| `UnityEngine.CoreModule.dll` | `ULTRAKILL_Data\Managed\` |
| `UnityEngine.PhysicsModule.dll` | `ULTRAKILL_Data\Managed\` |

Copy these files into your local `lib\` folder, then add them as references in Visual Studio:
- Right click **References** in Solution Explorer
- **Add Reference → Browse**
- Navigate to `lib\` and select all `.dll` files

---

## Building the project

1. In Visual Studio → **Build → Build Solution** (`Ctrl+Shift+B`)
2. The compiled file is located at:
   ```
   E:\Modding\The-Biding-of-V1\bin\Debug\TheBindingOfV1.dll
   ```

---

## Hot reload setup (ScriptEngine)

Instead of restarting the game every time, ScriptEngine automatically reloads your plugin when the `.dll` changes.

1. Download ScriptEngine from https://github.com/BepInEx/BepInEx.Debug/releases
2. Place `BepInEx.Debug.ScriptEngine.dll` in `BepInEx\plugins\`
3. Create a `scripts\` folder in your BepInEx profile folder
4. In `BepInEx\config\BepInEx.Debug.ScriptEngine.cfg` set:
   ```
   EnableFileSystemWatcher = true
   LoadOnStart = true
   ```
5. Add the `scripts\` folder as a PostBuild destination in your `.csproj`

**Workflow:** `Ctrl+Shift+B` in Visual Studio → plugin reloads automatically in game.

---

## Testing the plugin in ULTRAKILL

1. Place `TheBindingOfV1.dll` in the `scripts\` folder of your r2modman profile
2. Launch ULTRAKILL via r2modman (**Start modded**)
3. The BepInEx console should show:
   ```
   Mod TheBindingOfV1 version 0.1.0 is loading...
   Mod TheBindingOfV1 version 0.1.0 is loaded!
   ```

---

## Finding your r2modman profile folder

In r2modman → left menu → **"Browse profile folder"**

The path looks like:
```
C:\Users\YourName\AppData\Roaming\r2modmanPlus-local\ULTRAKILL\profiles\YourProfile\
```