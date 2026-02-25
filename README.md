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

## Sprint 1 + Sprint 2 implementados
- **HUD feedback:** `GameplayHudPresenter` maneja estado de cabecera y mensajes con estilo (positivo/neutral/warning).
- **FTUE visual + gating:** `FtueOverlayPresenter` + reglas de acción permitida (`FtueSystem.IsActionAllowed`).
- **Daily reward:** racha persistente y claim automático diario.
- **Analytics adaptable:** `AnalyticsSystem` con modos `DebugLog`, `Buffered` y `UnityBackendPlaceholder`.

## Novedad: progreso persistente (Unity)
- El juego guarda estado local automáticamente tras spin, upgrade y cambio de target.
- Archivo guardado en `Application.persistentDataPath/fortune_heist_save.json`.
- Incluye oro, spins, heist streak, edificios del jugador, objetivos NPC, FTUE y daily reward.
- Desde `DebugMenu` puedes limpiar progreso con `ClearProgress`.

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

## Unity MVP
- Scripts principales en `Assets/Scripts/`:
  - `BuildingSystem.cs`
  - `AttackSystem.cs`
  - `WheelSystem.cs`
  - `SaveSystem.cs`
  - `FtueSystem.cs`
  - `FtueOverlayPresenter.cs`
  - `DailyRewardSystem.cs`
  - `AnalyticsSystem.cs`
  - `GameplayHudPresenter.cs`
  - `GameplayController.cs`
  - `DebugMenu.cs`
- Setup editor en `Assets/Editor/SceneSetupEditor.cs` para garantizar `GameScene` y Build Settings por defecto.
- Plan técnico actualizado en `PLANS.md`.
