# Fortune Heist — Art Direction (Premium Pass v1)

## Visual Pillars
- **Readable in <1s**: player must instantly read target, spins, and rewards.
- **Reward-first feedback**: wins feel rich via color, motion, and micro-FX.
- **Stylized premium**: clean gradients + neon accents + polished cards.

## Color System
- Positive: `#6CFF8A`
- Neutral: `#FFFFFF`
- Warning: `#FF9A4D`
- Accent: `#8ED0FF`

These colors are configurable at runtime through `Assets/StreamingAssets/theme_config.json`.

## UI Style Rules
- Card corners rounded, with soft outer shadow.
- Primary buttons high-contrast, large tap area for mobile.
- Critical numbers (gold, spins) use bold typography and icon pairing.
- FTUE overlays must never fully hide the main objective area.

## Asset Production Specs
- Export UI sprites in 2 variants: `1x` and `2x`.
- Use sprite atlases by domain (`ui_hud`, `ui_targets`, `ui_buildings`).
- Keep texture compression Android-friendly (ASTC where possible).

## Motion Language
- Win messages: short pulse + fade in/out.
- Warnings: warm tint + slight shake (later pass).
- Target select: border glow transition.

## Next Premium Tasks
1. Build first prefab kit (`HUDCard`, `PrimaryButton`, `TargetTile`).
2. Replace placeholder text rows with card-based prefabs.
3. Add result particle bursts for Gold/Attack/Rob outcomes.
