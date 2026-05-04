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
├── Plugin.cs                  ← main plugin entry point
├── Properties/
│   └── AssemblyInfo.cs
├── Generation/                ← procedural generation system
│   ├── RoomType.cs
│   ├── VerticalTransitionType.cs
│   ├── RoomData.cs
│   ├── PlacedRoom.cs
│   ├── GraphGenerator.cs
│   ├── RoomPlacer.cs
│   ├── NavigationGrid.cs
│   ├── AStarPathfinder.cs
│   ├── CorridorInstancer.cs
│   └── FloorGenerator.cs
├── Managers/                  ← runtime managers
│   ├── RoomManager.cs
│   └── RunManager.cs
├── Doc/                       ← design documents and references
├── lib/                       ← DLL references (gitignored, fill manually)
└── bin/                       ← build output (gitignored)
```

---

## Documentation

| Document | Description |
|---|---|
| [Game Design — Conception Globale](Doc/conception_globale.md) | Full game design document — run structure, items, souls, shops |
| [Game Design — Items](Doc/items_ultrakill.json) | Complete item list with effects, rarity and dev notes |
| [Game Design — Progression System](Doc/systeme_progression_en.md) | Enemy classes, drop system, difficulty scaling |
| [Game Design — GDD Mechanics Reference](Doc/game_design_mechanics_reference.md) | All ULTRAKILL mechanics available for item design |
| [Technical — Room Creation & Pathfinding](Doc/TBV1_Room_And_Pathfinding_TDD.md) | Rude editor setup, arena creation, A* corridor routing |
| [Technical — Room Placement Theory](Doc/TBV1_Room_Placement_Theory.docx) | Theoretical doc with diagrams — room placement, navigation grid, corridor instancing |
| [Technical — Unity & C# Course](Doc/unity_csharp_course_en.md) | Unity and C# concepts for developers new to the engine |
| [Technical — Harmony Patches](Doc/doc_harmony_patches_en.md) | How to use Harmony to patch ULTRAKILL code |
| [Technical — BepInEx Plugin Setup](Doc/setup_plugin_bepinex_en.md) | Full plugin setup guide for new contributors |
| [Technical — Unity ↔ BepInEx Interface](Doc/interface_unity_bepinex.md) | Component contracts between mapper and plugin developer |
| [Resources](Doc/ressources_ultrakill_mod.md) | All tools, links and Discord servers |

---

## Contributing

- Join the [UltraModding Discord](https://discord.gg/wF8ttp8G) for modding help
- Join the [ULTRAKILL Mapping Discord](https://discord.gg/KqK5yDsRjQ) for map editing help
- Read the [BepInEx Plugin Setup guide](Doc/setup_plugin_bepinex_en.md) before starting

---

## License

[MIT](LICENSE)
