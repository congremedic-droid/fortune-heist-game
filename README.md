# Fortune Heist Game

Repositorio híbrido con:
- Prototipo Python (CLI + simulación JSON)
- Fase MVP de Unity (Building/Attack/Rob con visuales placeholder)

## ⚠️ Cómo abrir en Unity (importante)
Si descargas ZIP desde GitHub, **descomprímelo completo** y en Unity Hub usa **Add/Open** sobre la carpeta raíz `fortune-heist-game`, NO sobre `fortune_heist/`.

La carpeta correcta debe contener al menos:
- `Assets/`
- `Packages/manifest.json`
- `ProjectSettings/ProjectVersion.txt`

## Si Unity no abre el proyecto
1. Verifica versión del editor: **2022.3.20f1** (o una 2022.3 LTS compatible).
2. Mueve la carpeta del proyecto fuera de rutas sincronizadas (OneDrive/Dropbox), por ejemplo `C:\UnityProjects\fortune-heist-game`.
3. Borra cachés locales (si existen): `Library/`, `Temp/`, `Obj/`, `Logs/`.
4. Abre Unity Hub como administrador una vez y vuelve a `Add project`.
5. Si falla, revisa log:
   - `%LOCALAPPDATA%\Unity\Editor\Editor.log`
   - busca las últimas líneas con `error` o `exception`.

## Premium visual pass (nuevo)
- Tema visual configurable por JSON en `Assets/StreamingAssets/theme_config.json`.
- `ThemeConfigService` carga el tema y lo aplica en runtime vía `GameplayHudPresenter.ApplyTheme(...)`.
- `UIFeedbackAnimator` soporta pulso configurable de resultado para mejorar percepción de recompensa.
- Guía de arte disponible en `ART_DIRECTION.md`.

## Roadmap implementado (primeros 4 pasos)
- **UI feedback mejorado:** `GameplayHudPresenter` + `UIFeedbackAnimator`.
- **FTUE visual y gating:** `FtueOverlayPresenter` + bloqueo de acciones por paso.
- **Analytics adaptable:** proveedores intercambiables en `AnalyticsSystem`.
- **Android pipeline:** build script `AndroidBuildPipeline` + workflow `android-build.yml`.

## Live balance (Remote Config local)
- Configurable en `Assets/StreamingAssets/remote_balance.json`.
- Ajusta probabilidades y economía sin recompilar:
  - gold/attack/rob chances
  - gold reward, attack bonus
  - rob min/max percent
  - heist streak cap
  - building upgrade cost multiplier

## Build Android local (Unity Editor)
- Menú: `FortuneHeist/Build/Android APK`
- Menú: `FortuneHeist/Build/Android AAB`

## Python (rápido)
### Requisitos
- Python 3.10+

### Ejecutar (modo interactivo)
```bash
python -m fortune_heist.cli
```

### Vista previa para Unity (JSON)
```bash
python -m fortune_heist.cli --preview-json
```

### Ejecutar pruebas
```bash
python -m unittest discover -s tests -p 'test_*.py'
```
