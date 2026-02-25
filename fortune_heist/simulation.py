from __future__ import annotations

from dataclasses import asdict
import random
from typing import Iterable

from .game import FortuneHeistGame, HeistAction


def simulate_actions(actions: Iterable[str], seed: int = 42) -> dict:
    """Simula una partida y devuelve una vista previa estructurada para Unity."""
    game = FortuneHeistGame(rng=random.Random(seed))
    turns: list[dict] = []

    for index, raw_action in enumerate(actions, start=1):
        if game.is_over:
            break

        normalized = raw_action.strip().lower()
        before = {"loot": game.loot, "alert": game.alert, "intel": game.intel}

        try:
            action = HeistAction(normalized)
            result = game.apply_action(action)
            valid = True
            applied_action = action.value
        except ValueError:
            result = game.apply_action(HeistAction.HIDE)
            valid = False
            applied_action = "hide"

        after = {"loot": game.loot, "alert": game.alert, "intel": game.intel}
        turns.append(
            {
                "turn": index,
                "input": normalized,
                "applied_action": applied_action,
                "valid_input": valid,
                "before": before,
                "after": after,
                "result": asdict(result),
            }
        )

    return {
        "seed": seed,
        "target_loot": game.target_loot,
        "max_alert": game.max_alert,
        "turns": turns,
        "final": {
            "loot": game.loot,
            "alert": game.alert,
            "intel": game.intel,
            "won": game.has_won,
            "lost": game.has_lost,
            "status_line": game.status_line(),
        },
    }
