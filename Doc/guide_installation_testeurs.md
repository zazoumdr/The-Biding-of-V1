# 🩸 The Binding of V1 — Guide d'installation pour testeurs

> Pas besoin de savoir coder ! Suis les étapes dans l'ordre et ça marchera.

---

## Ce dont tu as besoin

- **ULTRAKILL** installé sur Steam
- **Une connexion internet**
- **~500 Mo d'espace disque libre**

---

## Étape 1 — Télécharger r2modman

r2modman est un gestionnaire de mods. Il s'occupe de tout installer automatiquement à ta place.

1. Va sur : https://thunderstore.io/package/ebkr/r2modman/
2. Clique sur **"Manual Download"**

> 📸 *[Capture d'écran : page Thunderstore avec le bouton Manual Download entouré en rouge]*

3. Ouvre le fichier `.exe` téléchargé et installe le programme
4. Si Windows affiche un avertissement de sécurité → clique sur **"Informations complémentaires"** puis **"Exécuter quand même"**

> 📸 *[Capture d'écran : fenêtre d'avertissement Windows avec les boutons indiqués]*

---

## Étape 2 — Configurer r2modman pour ULTRAKILL

1. Lance **r2modman**
2. Dans la liste des jeux, cherche et clique sur **ULTRAKILL**

> 📸 *[Capture d'écran : liste des jeux dans r2modman avec ULTRAKILL sélectionné]*

3. Clique sur **"Select game"**
4. Clique sur **"Create new profile"**
5. Donne-lui un nom (ex: `TheBindingOfV1`) et confirme

> 📸 *[Capture d'écran : fenêtre de création de profil]*

---

## Étape 3 — Installer les mods nécessaires

Tu dois installer 3 mods dans cet ordre.

### 3.1 — BepInExPack

1. Dans r2modman, clique sur **"Online"** dans le menu à gauche
2. Dans la barre de recherche, tape `BepInExPack`
3. Clique sur le mod **"BepInExPack"** par *BepInEx*
4. Clique sur **"Download"** puis **"Install"**

> 📸 *[Capture d'écran : recherche BepInExPack dans r2modman avec le bouton Download]*

### 3.2 — PluginConfigurator

1. Dans la barre de recherche, tape `PluginConfigurator`
2. Clique sur le mod **"PluginConfigurator"** par *EternalsTeam*
3. Clique sur **"Download"** puis **"Install"**

> 📸 *[Capture d'écran : recherche PluginConfigurator]*

### 3.3 — AngryLevelLoader

1. Dans la barre de recherche, tape `AngryLevelLoader`
2. Clique sur le mod **"AngryLevelLoader"** par *EternalsTeam*
3. Clique sur **"Download"** puis **"Install"**

> 📸 *[Capture d'écran : recherche AngryLevelLoader]*

---

## Étape 4 — Installer The Binding of V1

1. Dans la barre de recherche, tape `TheBindingOfV1`
2. Clique sur le mod et installe-le

> 📸 *[Capture d'écran : page du mod The Binding of V1]*

> ⚠️ *Cette étape sera disponible une fois le mod publié sur Thunderstore. En attendant, demande au développeur le fichier `.dll` et place-le dans :*
> `ULTRAKILL/BepInEx/plugins/`

---

## Étape 5 — Lancer le jeu en mode moddé

> ⚠️ **Important** — Ne lance **pas** ULTRAKILL depuis Steam directement. Utilise toujours r2modman pour jouer avec les mods.

1. Dans r2modman, clique sur **"Start modded"** en haut à gauche

> 📸 *[Capture d'écran : bouton "Start modded" dans r2modman entouré en rouge]*

2. Le jeu se lance avec une fenêtre de console noire en arrière-plan — c'est normal, ne la ferme pas
3. Dans le menu principal du jeu → **Settings → Plugin Config → Angry Level Loader**
4. Clique sur **"Custom Levels"** pour accéder au mode roguelike

> 📸 *[Capture d'écran : menu Plugin Config dans ULTRAKILL]*

---

## ✅ C'est bon !

Si tu vois le menu du mod, l'installation est réussie. Bonne session de test !

---

## ❌ Quelque chose ne marche pas ?

| Problème | Solution |
|---|---|
| Le jeu ne se lance pas | Vérifie que les 3 mods sont bien installés dans r2modman |
| La console noire se ferme immédiatement | Réinstalle BepInExPack |
| Le mod n'apparaît pas dans Plugin Config | Vérifie que le fichier `.dll` est bien dans `BepInEx/plugins/` |
| Autre problème | Contacte le développeur en décrivant ce qui s'est passé |

---

*Guide rédigé pour The Binding of V1 — version bêta testeurs*
