# 🩸 The Binding of V1 — Installation Guide for Testers

> No coding knowledge needed! Just follow the steps in order and it will work.

---

## What you need

- **ULTRAKILL** installed on Steam
- **An internet connection**
- **~500 MB of free disk space**

---

## Step 1 — Download r2modman

r2modman is a mod manager. It handles everything automatically for you.

1. Go to: https://thunderstore.io/package/ebkr/r2modman/
2. Click **"Manual Download"**

> 📸 *[Screenshot: Thunderstore page with the Manual Download button highlighted in red]*

3. Open the downloaded `.exe` file and install the program
4. If Windows shows a security warning → click **"More info"** then **"Run anyway"**

> 📸 *[Screenshot: Windows security warning with the buttons indicated]*

---

## Step 2 — Configure r2modman for ULTRAKILL

1. Launch **r2modman**
2. In the game list, find and click on **ULTRAKILL**

> 📸 *[Screenshot: r2modman game list with ULTRAKILL selected]*

3. Click **"Select game"**
4. Click **"Create new profile"**
5. Give it a name (e.g. `TheBindingOfV1`) and confirm

> 📸 *[Screenshot: profile creation window]*

---

## Step 3 — Install the required mods

You need to install 3 mods in this order.

### 3.1 — BepInExPack

1. In r2modman, click **"Online"** in the left menu
2. In the search bar, type `BepInExPack`
3. Click on the mod **"BepInExPack"** by *BepInEx*
4. Click **"Download"** then **"Install"**

> 📸 *[Screenshot: BepInExPack search in r2modman with the Download button]*

### 3.2 — PluginConfigurator

1. In the search bar, type `PluginConfigurator`
2. Click on the mod **"PluginConfigurator"** by *EternalsTeam*
3. Click **"Download"** then **"Install"**

> 📸 *[Screenshot: PluginConfigurator search]*

### 3.3 — AngryLevelLoader

1. In the search bar, type `AngryLevelLoader`
2. Click on the mod **"AngryLevelLoader"** by *EternalsTeam*
3. Click **"Download"** then **"Install"**

> 📸 *[Screenshot: AngryLevelLoader search]*

---

## Step 4 — Install The Binding of V1

1. In the search bar, type `TheBindingOfV1`
2. Click on the mod and install it

> ⚠️ *This step will be available once the mod is published on Thunderstore. In the meantime, ask the developer for the `.dll` file and place it in:*
> `ULTRAKILL/BepInEx/plugins/`

---

## Step 5 — Launch the game in modded mode

> ⚠️ **Important** — Do **not** launch ULTRAKILL directly from Steam. Always use r2modman to play with mods.

1. In r2modman, click **"Start modded"** in the top left

> 📸 *[Screenshot: "Start modded" button in r2modman highlighted in red]*

2. The game launches with a black console window in the background — this is normal, do not close it
3. In the game's main menu → **Settings → Plugin Config → Angry Level Loader**
4. Click **"Custom Levels"** to access the roguelike mode

> 📸 *[Screenshot: Plugin Config menu in ULTRAKILL]*

---

## ✅ All done!

If you can see the mod menu, the installation was successful. Enjoy your testing session!

---

## ❌ Something isn't working?

| Problem | Solution |
|---|---|
| The game won't launch | Make sure all 3 mods are installed in r2modman |
| The black console closes immediately | Reinstall BepInExPack |
| The mod doesn't appear in Plugin Config | Make sure the `.dll` file is in `BepInEx/plugins/` |
| Any other issue | Contact the developer and describe what happened |

---

*Guide written for The Binding of V1 — beta testers version*