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

## Executed Roadmap (first 4 steps)
1. **UI production foundation**
   - Added result pulse support through `UIFeedbackAnimator` and richer result styling in `GameplayHudPresenter`.
2. **FTUE visual complete baseline**
   - Added `FtueOverlayPresenter` + FTUE action gating for target/spin/upgrade.
3. **Analytics backend-ready architecture**
   - `AnalyticsSystem` now has pluggable providers (`DebugLog`, `Buffered`, `UnityBackendPlaceholder`).
4. **Android build pipeline**
   - Added `AndroidBuildPipeline` editor script and CI workflow (`.github/workflows/android-build.yml`).

## Live Balance / Remote Config
- Added `RemoteConfigService` loading `StreamingAssets/remote_balance.json`.
- Wheel/attack/rob/building costs now read runtime config values from `BalanceConfigData`.

## Premium Visual Pass (current)
- Added `ThemeConfigService` and `ThemeConfigData` to load runtime UI theme from `StreamingAssets/theme_config.json`.
- Integrated theme application in `GameplayController -> GameplayHudPresenter.ApplyTheme(...)`.
- Added `ART_DIRECTION.md` as production art style guide and asset pipeline baseline.

## Next Open Items
- Bind production prefabs in `GameScene` (HUD text, FTUE overlay and result animator references).
- Replace `UnityBackendPlaceholder` analytics provider with Firebase/Unity Analytics SDK.
- ✅ Added optional keystore secret handling for signed Android CI builds (`FH_ANDROID_KEYSTORE_*` + alias secrets).
- ✅ Extended remote config with `FtueEnabled`, `AnalyticsSampleRate` and `AnalyticsProviderOverride` (local file + runtime application).
- Monetization implementation (Rewarded Ads + IAP) after gameplay KPIs are stable.
