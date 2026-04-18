# 🛠️ Setup Plugin BepInEx — The Binding of V1

## Structure du repo avant le premier build

```
E:\Modding\The-Biding-of-V1\
│
├── .gitignore                          ← gitignore Unity (déjà présent)
├── ULTRAKILLRemadeTemplate.csproj      ← fichier projet Visual Studio
├── ULTRAKILLRemadeTemplate.sln         ← solution Visual Studio
├── Plugin.cs                           ← code principal du mod
├── README.md                           ← description du projet
│
├── Properties\
│   └── AssemblyInfo.cs                 ← métadonnées de l'assembly
│
└── lib\                                ← DLL références (NE PAS commit sur GitHub)
    ├── BepInEx.dll
    ├── 0Harmony.dll
    ├── Assembly-CSharp.dll
    └── UnityEngine.dll
```

> ⚠️ **Important** — Le dossier `lib` ne doit PAS être commit sur GitHub car il contient des fichiers du jeu. Vérifie que ton `.gitignore` l'exclut bien. Ajoute cette ligne si elle n'est pas dedans :
> ```
> lib/
> ```

---

## Contenu de Plugin.cs

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine;
using HarmonyLib;
using GameConsole.pcon;

namespace ULTRAKILLBepInExTemplate
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

        private void Update()
        {
            // Exécuté à chaque frame
        }
    }
}
```

---

## Références DLL requises

| Fichier | Provenance |
|---|---|
| `BepInEx.dll` | `r2modman profile folder\BepInEx\core\` |
| `0Harmony.dll` | `r2modman profile folder\BepInEx\core\` |
| `Assembly-CSharp.dll` | `ULTRAKILL_Data\Managed\` |
| `UnityEngine.dll` | `ULTRAKILL_Data\Managed\` |

---

## Builder le projet

1. Dans Visual Studio → **Générer → Générer la solution** (`Ctrl+Shift+B`)
2. Le fichier compilé se trouve dans :
   ```
   E:\Modding\The-Biding-of-V1\bin\Debug\TheBindingOfV1.dll
   ```

---

## Tester le plugin dans ULTRAKILL

1. Copie `TheBindingOfV1.dll` dans le dossier plugins de ton profil r2modman :
   ```
   [r2modman profile folder]\BepInEx\plugins\
   ```
2. Lance ULTRAKILL via r2modman en mode moddé
3. La console BepInEx doit afficher :
   ```
   Mod TheBindingOfV1 version 0.1.0 is loading...
   Mod TheBindingOfV1 version 0.1.0 is loaded!
   ```

---

## Trouver le dossier de profil r2modman

Dans r2modman → menu de gauche → **"Browse profile folder"**

Le chemin ressemble à :
```
C:\Users\TonNom\AppData\Roaming\r2modmanPlus-local\ULTRAKILL\profiles\TonProfil\
```
