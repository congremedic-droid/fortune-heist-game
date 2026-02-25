# Fortune Heist Game

Juego de consola por turnos donde intentas robar una fortuna sin disparar la alarma.

## Requisitos
- Python 3.10+

## Ejecutar (modo interactivo)
```bash
python -m fortune_heist.cli
```

## Vista previa para Unity (JSON)
```bash
python -m fortune_heist.cli --preview-json
```

También puedes pasar acciones y semilla propias:
```bash
python -m fortune_heist.cli --preview-json --actions "scout,steal,hide,steal" --seed 7
```

## Ejecutar pruebas
```bash
python -m unittest discover -s tests -p 'test_*.py'
```
