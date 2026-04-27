# 🩸 Progression & Spawn System — The Binding of V1

---

## 🏗️ Run Structure

```
Room 1 → Room 2 → Room 3 → [MINI-BOSS] → [SHOP] → [BOSS] → Next Floor...
```

- A **floor** = several combat rooms + 1 mini-boss + 1 item room + 1 shop + 1 boss
- **3 floors per run** (fixed)
- The boss always drops a **legendary item** (guaranteed)
- The mini-boss always drops an **item** (guaranteed)

---

## ⚙️ Difficulty System

Based on **`room_index`** (room number since the start of the run, starts at 0).

### Enemy count
```
enemy_count = base + floor(room_index / 3)
```
- `base` = 3 or 4 enemies
- +1 enemy every 3 rooms

### Enemy spawn pool — tier unlocks
The higher the `room_index`, the stronger the enemies that can appear.

| Tier | Enemies | Unlocks at room |
|---|---|---|
| D | Filth, Strays | 0 (from the start) |
| C | Schism, Soldier, Drone | ~room 5 |
| B | Virtue, Stalker, Cerberus | ~room 10 |
| A | Ferryman, Mindflayer | ~room 15 |
| S | Hideous Mass, boss enemies | ~room 20+ |
| Ultra | Custom boss enemies | Boss rooms only |

> Exact thresholds to be calibrated during playtesting.

---

## 👾 Enemy Classes & Calculated Difficulty

Each enemy has a **base class** defined by the team (size, HP, overall difficulty).

| Class | Soul value | Base difficulty |
|---|---|---|
| D | 5 souls | 0.5 |
| C | 10 souls | 1 |
| B | 25 souls | 1.5 |
| A | 50 souls | 2 |
| S | 100 souls | 3 |
| Ultra | 200 souls | 5 |

### Variants & calculated difficulty
Enemies can spawn with variants that modify their calculated difficulty:
- **Mega** → base difficulty +1
- **Angry** → base difficulty +2
- *Other variants to be defined*

---

## 🎲 Item Drop System

### Principle
Each enemy has a **class** that determines its drop chance and the rarity of the dropped item.

Items are **not dropped during combat** — they are **rewards** given for completing:
- A boss room (legendary item, guaranteed)
- A mini-boss room (item guaranteed)
- Occasionally a normal combat room

### Drop table

| Enemy tier | Examples | Drop chance | Item rarity |
|---|---|---|---|
| D | Filth, Strays | 15% | Common |
| C | Schism, Soldier | 10% | Uncommon |
| B | Virtue, Stalker | 5% | Rare |
| A | Ferryman, Mindflayer | 2% | Epic |
| S / Boss | Hideous Mass, bosses | 1% (boss = 100%) | Legendary |

### Rarity rules
- A **common** item can appear in all pools
- A **rare** item only appears in Rare, Epic and Legendary pools
- A **legendary** item only appears in the Legendary pool (boss only)

### No cap for now
- No drop limit per room
- No diminishing returns
- Snowballing is intentionally allowed — to be reassessed after playtesting

---

## 💰 Soul System

### Formula
```
souls_earned = sum(enemy_flat_value) * style_multiplier
```

### Style multiplier

| End of room rank | Multiplier |
|---|---|
| D | x0.5 |
| C | x1 |
| B | x1.5 |
| A | x2 |
| S | x3 |
| SS / SSS | x4 |

---

## 📋 Item rarity reference

| Item | Suggested rarity |
|---|---|
| Soda Quelconque | Common |
| Miroir | Uncommon |
| Miroir Sans Teint | Rare |
| Chapeau de Loup-Garou | Common |
| Extincteur | Common |
| Cookie Macadamia | Common |
| Seringue à Sida | Rare |
| Coupure Pub | Epic |
| Petit Livre Rouge | Legendary |
| Chèque en Blanc | Legendary |
| Le Saint Canard | ??? (secret) |

---

## 🔧 Implementation Notes

- `room_index` is the central variable — everything depends on it
- Spawn pools are **weighted lists**: the stronger the enemy, the lower its weight
- The drop system hooks onto **each enemy's death**
- Keep a **seed per run** to reproduce a run (debug + fun)

---

## 📅 To be defined later
- Exact number of combat rooms per floor
- Enemy tier unlock thresholds (to calibrate during tests)
- Meta-progression system between runs (permanent unlocks?)
- Whether to keep the snowball or cap it eventually
- Complete enemy class table (which enemy belongs to which tier)
- Additional enemy variants and their difficulty modifiers