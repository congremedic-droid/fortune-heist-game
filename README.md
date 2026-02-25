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


## Setup automático de UI en Unity (recomendado)
Si abriste `GameScene` pero te faltan referencias en Inspector, usa:

- `FortuneHeist > Setup > Bootstrap Everything` (recomendado, todo en un click)
- `FortuneHeist > Setup > Auto Wire Scene UI` (solo crea/conecta UI)
- `FortuneHeist > Setup > Validate Scene Setup` (diagnóstico de referencias faltantes)

`Bootstrap Everything` asegura escena/build settings, abre `Assets/Scenes/GameScene.unity`, ejecuta autowire de UI y corre validación.

El autowire crea Canvas/EventSystem, textos/botones base, un prefab de fila en `Assets/Prefabs/RowButton.prefab` y conecta automáticamente los campos serializados clave (`GameplayController`, `BuildingSystem`, `GameplayHudPresenter`, `FtueOverlayPresenter`, `DebugMenu`).

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
- Ajusta probabilidades, FTUE y analytics sin recompilar:
  - gold/attack/rob chances
  - gold reward, attack bonus
  - rob min/max percent
  - heist streak cap
  - building upgrade cost multiplier
  - `FtueEnabled` para activar/desactivar tutorial
  - `AnalyticsSampleRate` (0.0 - 1.0) para muestreo de eventos
  - `AnalyticsProviderOverride` (`debuglog`, `buffered`, `unityservices`)

## Build Android local (Unity Editor)
- Menú: `FortuneHeist/Build/Android APK`
- Menú: `FortuneHeist/Build/Android AAB`

## CI Android firmado (opcional)
Si quieres artefactos firmados en GitHub Actions, agrega estos secrets en el repo:
- `FH_ANDROID_KEYSTORE_BASE64` (keystore en base64)
- `FH_ANDROID_KEYSTORE_PASS`
- `FH_ANDROID_KEYALIAS_NAME`
- `FH_ANDROID_KEYALIAS_PASS`

Cuando `FH_ANDROID_KEYSTORE_BASE64` está presente, el workflow decodifica `BuildSecrets/release.keystore` y el build aplica firma custom desde variables de entorno.

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

### Vista previa con tuning de balance (nuevo)
```bash
python -m fortune_heist.cli --preview-json \
  --seed 42 \
  --actions "scout,steal,steal,hide,steal" \
  --steal-base-min 12 \
  --steal-base-max 24 \
  --steal-alert-min 18 \
  --steal-alert-max 30 \
  --steal-heat-per-chain 8
```

El JSON de salida ahora incluye un bloque `tuning` para registrar exactamente los parámetros usados en la simulación.


## Próximos pasos sugeridos
- Conectar `UnityBackendPlaceholder` con Firebase Analytics o Unity Gaming Services Analytics.
- Configurar firma Android en CI (keystore/base64 + secrets) para release interno.
- Vincular prefabs finales de HUD/FTUE en `GameScene` y validar en dispositivo Android.


## Vista gráfica rápida (web preview)
Si quieres ver una versión visual inmediata desde navegador (sin abrir Unity):

```bash
python -m http.server 8080
# luego abre http://localhost:8080/preview/
```

Esta vista replica el loop base de acciones (scout/hack/steal/hide) para validar UX rápidamente mientras ajustas balance.

