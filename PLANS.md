# Fortune Heist MVP Plans

## Building System Design
- `BuildingData` model includes:
  - `Name`
  - `Level`
  - `BaseUpgradeCost`
  - `RewardBonus`
- `BuildingSystem` responsibilities:
  - Holds building list and player gold.
  - Renders scrollable building rows via simple `Button` prefab + text.
  - Disables upgrade button when gold is insufficient.
  - Applies upgrade cost and increments building level/reward bonus.

## Attack / Rob Flow
1. Player selects a target from placeholder bases (3–5 entries).
2. Player spins wheel:
   - `ATTACK`
     - uses selected target (or random fallback).
     - if shielded: attack blocked.
     - else: downgrade one target building level and award attack gold bonus.
   - `ROB`
     - steals random percentage from target gold pool.
     - applies `HeistStreak` multiplier from `WheelSystem`.
3. Economy/UI update after each outcome.

## UI Sketch (Placeholder)
- Left panel: Buildings list (name, level, upgrade cost, bonus, upgrade button).
- Center/top: Spin button + spins counter + heist streak.
- Right panel: Target list (name, base level, gold pool, shield state, selected marker).
- Bottom: Result text and debug controls.

## Debug / Testing Helpers
- Debug menu actions:
  - Add gold
  - Add spins
  - Show building levels
  - Reset building levels

## Scene Setup
- Added editor bootstrap to ensure `Assets/Scenes/GameScene.unity` exists.
- Build settings auto-set so `GameScene` is the default launch scene.

## Next Open Items
- Persist player/base state between sessions.
- Replace placeholder buttons/text with production UI prefabs.
- Add target retaliation and player shield durability.
- Add balancing pass for wheel probabilities and economy.
