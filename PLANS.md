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

## Executed Sprint 1 (already completed)
1. UI/HUD foundation (`GameplayHudPresenter`).
2. FTUE + Daily Reward systems with persistence.
3. Local analytics event hooks.

## Executed Sprint 2 (current)
1. **UI feedback layer**
   - `GameplayHudPresenter.ShowResult(...)` now supports positive/neutral/warning styling.
   - `GameplayController` routes gameplay outcomes to HUD feedback helper.
2. **FTUE visual guidance + soft gating**
   - Added `FtueOverlayPresenter` (tutorial panel + blocked-action hint).
   - Added FTUE action gating in `FtueSystem` (`IsActionAllowed`).
   - Enforced gating for spin, target selection, and building upgrades.
3. **Analytics backend adapter architecture**
   - Reworked `AnalyticsSystem` to support provider modes:
     - `DebugLog`
     - `Buffered`
     - `UnityBackendPlaceholder` (ready swap for Firebase/Unity Analytics SDK).

## Next Open Items
- Create production UGUI prefabs and bind all presenter fields in scene.
- Replace analytics placeholder with Firebase/Unity Analytics real SDK implementation.
- Add Android build pipeline (keystore, versioning, CI artifacts).
- Add live balance controls (remote config) before monetization rollout.
- Monetization implementation (Rewarded Ads + IAP) after gameplay KPIs are stable.
