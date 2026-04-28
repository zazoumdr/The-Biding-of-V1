# The Binding of V1

A roguelike mod for ULTRAKILL. Run through handcrafted rooms, collect bizarre items dropped by defeated enemies, and spend your hard-earned souls at the shop between floors. Your combat style directly impacts your economy — the more stylish your kills, the more souls you earn.

> **Status: Early development — not yet playable**

---

## Requirements

- [ULTRAKILL](https://store.steampowered.com/app/1229490/ULTRAKILL/) (Steam)
- [r2modman](https://thunderstore.io/package/ebkr/r2modman/)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) with **.NET desktop development** workload
- [Unity 2022.3.28f1](https://unity.com/releases/editor/whats-new/2022.3.28) via Unity Hub (for map editing only)

---

## Setting up the project

### 1. Clone the repo

```bash
git clone https://github.com/zazoumdr/The-Biding-of-V1.git
```

### 2. Install required mods via r2modman

Create a profile for ULTRAKILL and install:
- BepInExPack
- AngryLevelLoader
- PluginConfigurator

### 3. Set up the `lib/` folder

Create a `lib/` folder at the root of the project and copy the following files into it:

| File | Location |
|---|---|
| `BepInEx.dll` | `[r2modman profile]\BepInEx\core\` |
| `0Harmony.dll` | `[r2modman profile]\BepInEx\core\` |
| `Assembly-CSharp.dll` | `[ULTRAKILL install]\ULTRAKILL_Data\Managed\` |
| `UnityEngine.dll` | `[ULTRAKILL install]\ULTRAKILL_Data\Managed\` |
| `UnityEngine.CoreModule.dll` | `[ULTRAKILL install]\ULTRAKILL_Data\Managed\` |
| `UnityEngine.PhysicsModule.dll` | `[ULTRAKILL install]\ULTRAKILL_Data\Managed\` |

> These files are excluded from version control (`.gitignore`). Every contributor must supply their own copies.

**Finding your r2modman profile folder:**
Open r2modman → select your ULTRAKILL profile → **Browse profile folder** in the left menu.

**Finding your ULTRAKILL install folder:**
Steam → right click ULTRAKILL → **Manage → Browse local files**

### 4. Add references in Visual Studio

- Right click **References** in the Solution Explorer
- **Add Reference → Browse**
- Navigate to the `lib/` folder
- Select all `.dll` files → **Add → OK**

### 5. Build the project

`Ctrl+Shift+B` or **Build → Build Solution**

The compiled `.dll` will be in `bin/Debug/`.

---

## Hot reload (optional but recommended)

Instead of restarting the game after every change, use ScriptEngine to reload your plugin automatically.

1. Download [ScriptEngine](https://github.com/BepInEx/BepInEx.Debug/releases) and place the `.dll` in `BepInEx/plugins/`
2. Create a `scripts/` folder in your BepInEx profile folder
3. In `BepInEx/config/BepInEx.Debug.ScriptEngine.cfg` set:
```
EnableFileSystemWatcher = true
LoadOnStart = true
```
4. Add `scripts/` as a PostBuild destination in your `.csproj`

**Workflow:** `Ctrl+Shift+B` → plugin reloads automatically in game.

---

## Testing in game

1. Launch ULTRAKILL via r2modman (**Start modded**)
2. The BepInEx console should show:
```
Mod TheBindingOfV1 version 0.1.0 is loading...
Mod TheBindingOfV1 version 0.1.0 is loaded!
```

---

## Project structure

```
The-Biding-of-V1/
├── Plugin.cs               ← main plugin entry point
├── Properties/
│   └── AssemblyInfo.cs
├── Doc/                    ← design documents and references
├── lib/                    ← DLL references (gitignored, fill manually)
└── bin/                    ← build output (gitignored)
```

---

## Contributing

- Join the [UltraModding Discord](https://discord.gg/wF8ttp8G) for modding help
- Join the [ULTRAKILL Mapping Discord](https://discord.gg/KqK5yDsRjQ) for map editing help
- Check the [item list](Doc/items_ultrakill.json) for design reference

---

## License

[MIT](LICENSE)
