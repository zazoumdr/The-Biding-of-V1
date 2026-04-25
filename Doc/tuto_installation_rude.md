# 🗺️ Installing Rude Level Editor — Complete Guide

---

## Prerequisites
- ULTRAKILL installed via Steam
- ~10 GB of free disk space (Vanity extracts a lot of assets)
- A non-system drive recommended (not C:)

---

## Step 1 — Install Unity Hub

Download Unity Hub: https://public-cdn.cloud.unity3d.com/hub/prod/UnityHubSetup.exe

Run the installer and follow the instructions.

> ⚠️ Unity Hub will offer to install Unity 6 on first launch — **decline**.

---

## Step 2 — Install Unity 2022.3.28f1

Click this direct link: `unityhub://2022.3.28f1/4af31df58517`

It will open Unity Hub and start the installation automatically. **This can take up to an hour** — start this first while doing something else.

---

## Step 3 — Download and launch Vanity Reprise

1. Go to https://github.com/eternalUnion/VanityReprised/releases/latest
2. Download the `VanityReprised-1.x.x.zip` file
3. Extract it into a folder
4. Run `Vanity.GUI.Free.exe`

A black Command Prompt window will open, then a webpage in your browser.

---

## Step 4 — Configure Vanity

In the webpage:

1. **Select the scenes to export** — navigate with the arrows. It is recommended to export **P-2** and **1-3** to see how official ULTRAKILL levels are structured. You can choose more but it will take more time and storage.

2. Click **"Open ULTRAKILL folder"** and select your ULTRAKILL install folder. To find it: Steam → right click ULTRAKILL → **Manage → Browse local files**.

3. **Choose a destination folder** for the Rude project — create a dedicated folder on your non-system drive (e.g. `E:\Modding\RudeLevelEditor\`).

4. Wait until the export finishes (a completion message will appear).

---

## Step 5 — Open the project in Unity Hub

1. In Unity Hub, click the **small arrow** next to the **Add** button
2. Click **Add from disk**
3. Navigate to `E:\Modding\RudeLevelEditor\` (or your chosen folder)
4. Select the folder

> ⚠️ **IMPORTANT** — If Unity shows popups asking to update backends or APIs → **always say NO**. These updates will break the editor.

5. Click on the project to open it

---

## ✅ Rude is installed!

You should see a **RUDE** menu in the Unity toolbar with a **Rude Exporter** option.

If the RUDE menu only shows "Check for updates", join the ULTRAKILL Mapping Discord for help: https://discord.gg/KqK5yDsRjQ

---

## Next step

Check the official docs to set up your first scene:
https://envy-spite-team.github.io/ULTRAMappingDocs/Setup/Scene%20Setup
