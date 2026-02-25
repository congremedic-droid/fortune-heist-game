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
  - Clear save progress

## Scene Setup
- Added editor bootstrap to ensure `Assets/Scenes/GameScene.unity` exists.
- Build settings auto-set so `GameScene` is the default launch scene.

## Executed Sprint 1 (3 Steps)
1. **UI/HUD foundation**
   - Added `GameplayHudPresenter` to centralize top-level HUD labels (gold, spins/streak, target, FTUE status, daily streak).
2. **FTUE + Daily Reward**
   - Added `FtueSystem` with 3-step onboarding: select target → spin → upgrade.
   - Added `DailyRewardSystem` with streak-based daily claim multiplier.
   - Persisted FTUE and daily reward data in `GameStateData`.
3. **Analytics hooks**
   - Added `AnalyticsSystem` with event capture for: session start, target select, spin result, building upgrade, daily reward claim.
   - Wired event tracking into `GameplayController` flow.

## Next Open Items
- Replace placeholder UI with production UGUI prefabs + animation transitions.
- Add full FTUE overlay (arrow/highlight/modal) beyond text hints.
- Integrate remote analytics backend (Firebase/Unity Analytics) replacing local logger.
- Add Android build pipeline (keystore, versioning, CI artifacts).
- Monetization implementation (Rewarded Ads + IAP) after gameplay KPIs are stable.
