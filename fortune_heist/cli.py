from __future__ import annotations

import argparse
import json

from .game import FortuneHeistGame, HeistAction
from .simulation import simulate_actions


def run() -> None:
    parser = argparse.ArgumentParser(description="Fortune Heist Game")
    parser.add_argument(
        "--preview-json",
        action="store_true",
        help="Genera una vista previa JSON para integrar en Unity.",
    )
    parser.add_argument(
        "--actions",
        default="scout,steal,hack,steal,hide,steal,steal",
        help="Lista de acciones separadas por coma usada por --preview-json.",
    )
    parser.add_argument(
        "--seed",
        type=int,
        default=42,
        help="Semilla determinística para --preview-json.",
    )
    parser.add_argument(
        "--steal-base-min",
        type=int,
        default=14,
        help="Balance tuning: robo mínimo base por acción STEAL.",
    )
    parser.add_argument(
        "--steal-base-max",
        type=int,
        default=28,
        help="Balance tuning: robo máximo base por acción STEAL.",
    )
    parser.add_argument(
        "--steal-alert-min",
        type=int,
        default=16,
        help="Balance tuning: alerta mínima agregada por STEAL.",
    )
    parser.add_argument(
        "--steal-alert-max",
        type=int,
        default=26,
        help="Balance tuning: alerta máxima agregada por STEAL.",
    )
    parser.add_argument(
        "--steal-heat-per-chain",
        type=int,
        default=6,
        help="Balance tuning: calor extra por robo consecutivo.",
    )

    args = parser.parse_args()

    if args.preview_json:
        actions = [item.strip() for item in args.actions.split(",") if item.strip()]
        tuning = {
            "steal_base_min": args.steal_base_min,
            "steal_base_max": args.steal_base_max,
            "steal_alert_min": args.steal_alert_min,
            "steal_alert_max": args.steal_alert_max,
            "steal_heat_per_chain": args.steal_heat_per_chain,
        }
        preview = simulate_actions(actions=actions, seed=args.seed, tuning=tuning)
        print(json.dumps(preview, indent=2, ensure_ascii=False))
        return

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
