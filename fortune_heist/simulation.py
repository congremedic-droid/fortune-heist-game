from __future__ import annotations

from dataclasses import asdict
import random
from typing import Any, Iterable

from .game import BalanceTuning, FortuneHeistGame, HeistAction


def simulate_actions(actions: Iterable[Any], seed: int = 42, tuning: dict[str, Any] | None = None) -> dict:
    """Simula una partida y devuelve una vista previa estructurada para Unity."""
    game_tuning = BalanceTuning(**(tuning or {}))
    game = FortuneHeistGame(rng=random.Random(seed), tuning=game_tuning)
    turns: list[dict] = []

    for index, raw_action in enumerate(actions, start=1):
        if game.is_over:
            break

        normalized = _normalize_action(raw_action)
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
        "tuning": game.tuning_snapshot(),
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


def _normalize_action(raw_action: Any) -> str:
    if raw_action is None:
        return ""

    if isinstance(raw_action, str):
        return raw_action.strip().lower()

    return str(raw_action).strip().lower()
