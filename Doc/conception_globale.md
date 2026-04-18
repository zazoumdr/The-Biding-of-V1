# 🩸 Document de Conception — ULTRAKILL Roguelike

---

## 1. Vue d'ensemble

**Nom du mod : The Binding of V1**

Un mod BepInEx pour ULTRAKILL ajoutant un mode roguelike complet avec ses propres salles, items, système de progression et économie. Publié sur Thunderstore via Angry Level Loader.

**Stack technique :**
- BepInEx + Harmony (logique roguelike)
- Spite / EnvyAndSpiteVanitized (map)
- Blockbench (modèles 3D des items)
- Unity 2019.4.40f1

---

## 2. Structure d'un Run

```
[Choix arme de départ]
        ↓
Salle 1 → Salle 2 → Salle 3 → [BOSS]
                                  ↓
                            [Shop d'étage]
                                  ↓
Salle 4 → Salle 5 → Salle 6 → [BOSS]
                                  ↓
                            [Shop d'étage]
                                  ↓
                              ...etc
```

- Chaque étage = X salles normales + 1 boss + 1 shop
- Le marchand spécial peut apparaître aléatoirement à la place d'une salle normale
- Nombre de salles par étage : à calibrer aux tests (suggestion : 3 à 5)

---

## 3. Arme de Départ

Au début de chaque run, le joueur choisit entre 3 armes de base :

| Arme | Style de jeu |
|---|---|
| Pistolet (Revolver) | Précision, longue portée |
| Shotgun | Agressif, close range |
| Nailgun | DPS continu, multi-ennemis |

- On repart de zéro à chaque run
- Les autres armes se débloquent via les items droppés en combat
- Les armes alternatives se débloquent via le marchand spécial

---

## 4. Système de Difficulté

Tout est basé sur **`room_index`** — le numéro de salle depuis le début du run (commence à 0).

### Quantité d'ennemis
```
nb_ennemis = base(3-4) + floor(room_index / 3)
```
+1 ennemi toutes les 3 salles.

### Pool de spawn — Déblocage par tier

| Tier | Ennemis | Se débloque à la salle |
|---|---|---|
| F | Filth, Strays | 0 (dès le début) |
| C | Schism, Soldier, Drone | ~salle 5 |
| B | Virtue, Stalker, Cerberus | ~salle 10 |
| A | Ferryman, Mindflayer | ~salle 15 |
| S | Hideous Mass, ennemis de boss | ~salle 20+ |

> Seuils à calibrer pendant les tests.

---

## 5. Système d'Âmes (Monnaie)

### Principe
Les âmes sont la monnaie du roguelike. Elles s'obtiennent en tuant des ennemis et servent à acheter dans les shops.

**Elles sont séparées du sang** — pas de tension entre se soigner et dépenser.

### Formule
```
âmes_gagnées = somme(valeur_flat_ennemis) * multiplicateur_style
```

### Valeur flat par tier

| Tier | Valeur flat |
|---|---|
| F | 5 âmes |
| C | 10 âmes |
| B | 25 âmes |
| A | 50 âmes |
| S | 100 âmes |
| Boss | 200 âmes |

### Multiplicateur de style

| Rang de fin de salle | Multiplicateur |
|---|---|
| D | x0.5 |
| C | x1 |
| B | x1.5 |
| A | x2 |
| S | x3 |
| SS / SSS | x4 |

### Animation
V1 verse du sang d'une bouteille d'eau dans le terminal du shop. 🩸

---

## 6. Système de Drop d'Items

### Principe
Chaque ennemi a une chance de dropper un item à sa mort, selon son tier.

### Table de drop

| Tier ennemi | Chance de drop | Rareté item droppé |
|---|---|---|
| F | 15% | Commun |
| C | 10% | Peu commun |
| B | 5% | Rare |
| A | 2% | Épique |
| S / Boss | 1% (boss = 100%) | Légendaire |

### Règles de rareté
- Commun → apparaît dans tous les pools
- Rare → uniquement Rare, Épique, Légendaire
- Légendaire → uniquement boss

### Pas de cap pour l'instant
Le snowball est volontairement possible. À réévaluer après les premiers tests.

---

## 7. Shops

### Shop d'étage (après chaque boss)
- Accessible après chaque boss
- Vend : armes, items, soins
- Payé en âmes

### Marchand Spécial
- Apparaît aléatoirement à la place d'une salle normale
- Vend uniquement les **armes alternatives**
- Rare — pas garanti dans un run
- Personnage : à définir (candidats : Soap, Gabriel déchu, perso original)

---

## 8. Salles

### Génération
Les salles sont **prédéfinies et tirées aléatoirement** — pas de génération procédurale. Une vingtaine de salles faites à la main dans Spite.

### Types de salles

| Type | Description |
|---|---|
| Combat normale | La majorité des salles |
| Boss | Une par étage, drop item garanti |
| Shop d'étage | Après chaque boss |
| Marchand spécial | Apparition aléatoire, armes alt |

### Thèmes de combat

| Thème | Favorise |
|---|---|
| Salle ouverte | Mouvement, nailgun |
| Salle avec piliers/obstacles | Ricochets, couverture |
| Salle verticale | Mouvement aérien, dash |
| Salle étroite | Shotgun, combat intense |

---

## 9. Items

Voir fichier séparé : `items_ultrakill.json`

### Rareté des items

| Item | Rareté |
|---|---|
| Soda Quelconque | Commun |
| Chapeau de Loup-Garou | Commun |
| Extincteur | Commun |
| Cookie Macadamia Blanc | Commun |
| Miroir | Peu commun |
| Gant de Boxe | Peu commun |
| Punch'in'Balls | Peu commun |
| Tirelire Cochon | Peu commun |
| Vaisselle de Mariage | Rare |
| Miroir Sans Teint | Rare |
| Lance-Armstrong | Rare |
| Mégaphone | Rare |
| Seringue à Sida | Rare |
| Cracker Vert | Rare |
| Saucisson Corse | Rare |
| Gangsta Gun | Rare |
| Zaza | Épique |
| Coupure Pub | Épique |
| The Power of Love | Épique |
| Livre | Épique |
| Petit Livre Rouge | Légendaire |
| Chèque en Blanc | Légendaire |
| Le Saint Canard | ??? (item secret — débloque un mode bonus) |

---

## 10. À Définir

- [x] Nom du mod → **The Binding of V1**
- [ ] Personnage du marchand spécial
- [ ] Prix des armes et items dans le shop
- [ ] Nombre de salles par étage
- [ ] Le Saint Canard et son mode bonus
- [ ] Meta-progression entre les runs (oui/non ?)
- [ ] Nombre exact de salles prédéfinies à créer
- [ ] Seuils de déblocage des tiers ennemis
