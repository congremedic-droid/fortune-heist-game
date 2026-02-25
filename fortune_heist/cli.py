from __future__ import annotations

from .game import FortuneHeistGame, HeistAction


def run() -> None:
    game = FortuneHeistGame()
    print("=== Fortune Heist Game ===")
    print("Objetivo: consigue $100M sin llegar a alerta máxima.")

    while not game.is_over:
        print("\n" + game.status_line())
        print("Acciones disponibles: scout, hack, steal, hide")
        choice = input("> Elige acción: ").strip().lower()

        try:
            action = HeistAction(choice)
        except ValueError:
            print("Acción inválida. Intenta de nuevo.")
            continue

        result = game.apply_action(action)
        print(result.message)

    print("\n=== Fin de la partida ===")
    print(game.status_line())
    if game.has_won:
        print("¡Misión cumplida! Eres una leyenda del robo.")
    else:
        print("Misión fallida. La seguridad del casino ganó esta vez.")


if __name__ == "__main__":
    run()
