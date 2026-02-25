# Fortune Heist Game

Repositorio híbrido con:
- Prototipo Python (CLI + simulación JSON)
- Fase MVP de Unity (Building/Attack/Rob con visuales placeholder)

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
  - `GameplayController.cs`
  - `DebugMenu.cs`
- Setup editor en `Assets/Editor/SceneSetupEditor.cs` para garantizar `GameScene` y Build Settings por defecto.
- Plan técnico actualizado en `PLANS.md`.
