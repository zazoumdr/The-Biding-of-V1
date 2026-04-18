# 🩸 Ressources Mod ULTRAKILL Roguelike

## 🛠️ Outils à installer

### Obligatoires
- **GitHub Desktop** → https://desktop.github.com/
- **Unity Hub** → https://unity.com/download
  - Version Unity à installer via Hub : **2019.4.40f1**
  - https://unity.com/releases/editor/whats-new/2019.4.40
- **Visual Studio** (Community, gratuit) → https://visualstudio.microsoft.com/fr/
  - Installer le workload **.NET desktop development**
- **r2modman** (mod manager pour ULTRAKILL) → https://thunderstore.io/package/ebkr/r2modman/
- **dnSpyEx** (lire le code du jeu) → https://github.com/dnSpyEx/dnSpy/releases

### Pour les modèles 3D
- **Blockbench** → https://www.blockbench.net/

---

## 📦 Mods à installer dans r2modman (pour jouer/tester)

| Mod | Lien |
|---|---|
| BepInExPack | https://thunderstore.io/c/ultrakill/p/BepInEx/BepInExPack/ |
| AngryLevelLoader | https://thunderstore.io/c/ultrakill/p/EternalsTeam/AngryLevelLoader/ |
| PluginConfigurator | https://thunderstore.io/c/ultrakill/p/EternalsTeam/PluginConfigurator/ |
| EnvyAndSpiteVanitized (éditeur de map) | https://thunderstore.io/c/ultrakill/p/ImNotSimon/EnvyAndSpiteVanitized/ |

---

## 📁 Repos GitHub à cloner / étudier

| Repo | Utilité |
|---|---|
| ULTRAKILLModdingTemplate | Template de base pour créer un plugin BepInEx |
| https://github.com/SatisfiedBucket/ULTRAKILLModdingTemplate | |
| UltraTweaker | Modifier les stats joueur/ennemis, référence principale |
| https://github.com/wafflethings/UltraTweaker | |
| HarderV2 | Exemple de Harmony patch minimaliste |
| https://github.com/m0tholith/HarderV2 | |
| UltraFunGuns | Référence pour ajouter de nouvelles mécaniques d'armes |
| https://github.com/Hydraxous/UltraFunGuns | |
| Vanity (setup éditeur de map) | Extrait les assets ULTRAKILL dans le projet Unity |
| https://github.com/ImNotSimon/Vanity-tool | |
| EnvyAndSpiteVanitized (éditeur de map) | Le projet Unity pour créer la map |
| https://github.com/Minepool9/EnvyAndSpiteVanity | |

---

## 💬 Discords à rejoindre

| Serveur | Lien | Pour quoi |
|---|---|---|
| UltraModding | https://discord.gg/wF8ttp8G | Modding général, aide technique, classes du jeu |
| Envy & Spite | https://discord.gg/bbUYVxMTYe | Aide pour la création de map |
| New Blood (officiel) | https://discord.gg/newblood | Canal #ultrakill-modding, accès Rude editor |

---

## 📚 Documentation

- **UltraModding docs** → https://ultramodding.github.io/
- **Thunderstore ULTRAKILL** (tous les mods) → https://thunderstore.io/c/ultrakill/
- **Spite LE Wiki** → https://envy-spite-team.github.io/Envy-Spite-Website/

---

## 🗺️ Ordre d'installation recommandé

1. Installer **r2modman**
2. Via r2modman : installer **BepInExPack** + **AngryLevelLoader** + **PluginConfigurator**
3. Installer **Visual Studio** + workload .NET
4. Cloner **ULTRAKILLModdingTemplate**
5. Installer **Unity Hub** + **Unity 2019.4.40f1**
6. Télécharger et lancer **Vanity** pour setup le projet de map
7. Installer **dnSpyEx** (garder sous le coude pour le reverse)
8. Installer **Blockbench** (pour plus tard, les skins d'items)

---

## 📋 Fichiers du projet

- `items_ultrakill.json` — Liste complète des 22 items avec effets et notes de dev
