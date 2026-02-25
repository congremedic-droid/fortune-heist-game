from __future__ import annotations

from dataclasses import dataclass, field
from enum import Enum
import random


class HeistAction(str, Enum):
    SCOUT = "scout"
    HACK = "hack"
    STEAL = "steal"
    HIDE = "hide"


@dataclass
class GameResult:
    message: str
    won: bool = False
    lost: bool = False


@dataclass
class FortuneHeistGame:
    target_loot: int = 100
    max_alert: int = 100
    rng: random.Random = field(default_factory=random.Random)
    loot: int = 0
    alert: int = 0
    intel: int = 0

    def apply_action(self, action: HeistAction) -> GameResult:
        if self.is_over:
            return GameResult("La partida ya terminó.", won=self.has_won, lost=self.has_lost)

        if action == HeistAction.SCOUT:
            self.intel = min(100, self.intel + self.rng.randint(15, 30))
            self.alert = min(self.max_alert, self.alert + self.rng.randint(5, 12))
            return self._finish_turn("Reconociste la bóveda y reuniste información.")

        if action == HeistAction.HACK:
            success_chance = 40 + self.intel // 2
            roll = self.rng.randint(1, 100)
            if roll <= success_chance:
                self.alert = max(0, self.alert - self.rng.randint(10, 20))
                self.intel = max(0, self.intel - 10)
                return self._finish_turn("Hack exitoso: bajaste el nivel de alerta.")
            self.alert = min(self.max_alert, self.alert + self.rng.randint(15, 25))
            return self._finish_turn("Hack fallido: los sistemas detectaron actividad.")

        if action == HeistAction.STEAL:
            base = self.rng.randint(18, 35)
            bonus = self.intel // 8
            stolen = base + bonus
            self.loot += stolen
            self.intel = max(0, self.intel - self.rng.randint(8, 15))
            self.alert = min(self.max_alert, self.alert + self.rng.randint(12, 20))
            return self._finish_turn(f"Robaste ${stolen}M en activos de alto valor.")

        if action == HeistAction.HIDE:
            reduced = self.rng.randint(12, 22)
            self.alert = max(0, self.alert - reduced)
            return self._finish_turn(f"Te escondiste y redujiste la alerta en {reduced} puntos.")

        return GameResult("Acción desconocida.")

    def _finish_turn(self, text: str) -> GameResult:
        if self.has_won:
            return GameResult(f"{text} ¡Escape perfecto! Lograste ${self.loot}M.", won=True)
        if self.has_lost:
            return GameResult(f"{text} La alarma máxima se activó. Te capturaron.", lost=True)
        return GameResult(text)

    @property
    def has_won(self) -> bool:
        return self.loot >= self.target_loot and self.alert < self.max_alert

    @property
    def has_lost(self) -> bool:
        return self.alert >= self.max_alert

    @property
    def is_over(self) -> bool:
        return self.has_won or self.has_lost

    def status_line(self) -> str:
        return (
            f"Botín: ${self.loot}M/{self.target_loot}M | "
            f"Alerta: {self.alert}/{self.max_alert} | "
            f"Inteligencia: {self.intel}/100"
        )
