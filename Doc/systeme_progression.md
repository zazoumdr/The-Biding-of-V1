# 🩸 Système de Progression & Spawn — ULTRAKILL Roguelike

---

## 🏗️ Structure d'un run

```
Salle 1 → Salle 2 → Salle 3 → [BOSS] → Salle 4 → Salle 5 → Salle 6 → [BOSS] → ...
```

- Un **étage** = X salles normales + 1 salle de boss
- Le boss drop **toujours** un item (garanti)
- Nombre de salles par étage : à définir (suggestion : 3 à 5)

---

## ⚙️ Système de difficulté

Deux variables évoluent en parallèle, basées sur **`room_index`** (numéro de salle depuis le début du run, commence à 0).

### Quantité d'ennemis
```
nb_ennemis = base + floor(room_index / 3)
```
- `base` = 3 ou 4 ennemis
- +1 ennemi toutes les 3 salles

### Force des ennemis — Pool de spawn
La difficulté détermine quels ennemis peuvent apparaître.
Plus `room_index` augmente, plus les tiers élevés se débloquent.

| Tier | Ennemis | Se débloque à la salle |
|---|---|---|
| F | Filth, Strays, Schism | 0 (dès le début) |
| C | Soldier, Drone | ~salle 5 |
| B | Virtue, Stalker, Cerberus | ~salle 10 |
| A | Ferryman, Mindflayer | ~salle 15 |
| S | Hideous Mass, ennemis de boss | ~salle 20+ |

> Les seuils exacts sont à calibrer pendant les tests.

---

## 🎲 Système de drop d'items

### Principe
Chaque ennemi a un **tier** qui détermine sa chance de dropper un item et la rareté de cet item.

### Table de drop

| Tier ennemi | Exemples | Chance de drop | Rareté item droppé |
|---|---|---|---|
| F | Filth, Strays | 15% | Commun |
| C | Schism, Soldier | 10% | Peu commun |
| B | Virtue, Stalker | 5% | Rare |
| A | Ferryman, Mindflayer | 2% | Épique |
| S | Hideous Mass, boss | 1% (boss = 100%) | Légendaire |

### Règle de rareté
- Un item **commun** peut apparaître dans tous les pools
- Un item **rare** n'apparaît que dans les pools Rare, Épique et Légendaire
- Un item **légendaire** n'apparaît que dans le pool Légendaire (boss uniquement)

### Pas de cap pour l'instant
- Pas de limite de drops par salle
- Pas de diminishing returns
- Le snowball est volontairement possible — à réévaluer après les premiers tests

---

## 📋 Rareté des items de la liste

> À remplir au fur et à mesure

| Item | Rareté suggérée | Justification |
|---|---|---|
| Soda Quelconque | Commun | Effet simple, bon item de départ |
| Miroir | Peu commun | Mécanique intéressante mais pas abusée seule |
| Miroir Sans Teint | Rare | Très fort en combo avec Miroir |
| Chapeau de Loup-Garou | Commun | Stackable mais effet modéré |
| Extincteur | Commun | Buff de mobilité simple |
| Cookie Macadamia | Commun | Regen passive, tradeoff raisonnable |
| Seringue à Sida | Rare | Burst HP potentiellement abusé |
| Coupure Pub | Épique | x2 HP c'est énorme |
| Petit Livre Rouge | Légendaire | Instakill toute la vague = win bouton |
| Chèque en Blanc | Légendaire | Double tous les effets = game breaking |
| Le Saint Canard | ??? | Mystère total |

---

## 🔧 Notes d'implémentation

- Le `room_index` est la variable centrale — tout en dépend
- Les pools de spawn sont des **listes pondérées** : plus un ennemi est fort, moins il a de poids dans le tirage
- Le système de drop se hook sur **la mort de chaque ennemi**
- Garder une seed par run pour pouvoir reproduire un run (debug + fun)

---

## 📅 À définir plus tard
- Nombre exact de salles par étage
- Seuils de déblocage des tiers ennemis (à calibrer aux tests)
- Système de meta-progression entre les runs (débloquages permanents ?)
- Est-ce qu'on garde le snowball ou on cap à terme
