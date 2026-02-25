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
    turns_played: int = 0
    consecutive_steals: int = 0

    def apply_action(self, action: HeistAction) -> GameResult:
        if self.is_over:
            return GameResult("La partida ya terminó.", won=self.has_won, lost=self.has_lost)

        self.turns_played += 1

        if action == HeistAction.SCOUT:
            self.consecutive_steals = 0
            self.intel = min(100, self.intel + self.rng.randint(15, 30))
            self.alert = min(self.max_alert, self.alert + self.rng.randint(5, 12))
            self._apply_security_pressure(action)
            return self._finish_turn("Reconociste la bóveda y reuniste información.")

        if action == HeistAction.HACK:
            self.consecutive_steals = 0
            success_chance = 40 + self.intel // 2
            roll = self.rng.randint(1, 100)
            if roll <= success_chance:
                self.alert = max(0, self.alert - self.rng.randint(10, 20))
                self.intel = max(0, self.intel - 10)
                self._apply_security_pressure(action)
                return self._finish_turn("Hack exitoso: bajaste el nivel de alerta.")
            self.alert = min(self.max_alert, self.alert + self.rng.randint(15, 25))
            self._apply_security_pressure(action)
            return self._finish_turn("Hack fallido: los sistemas detectaron actividad.")

        if action == HeistAction.STEAL:
            self.consecutive_steals += 1
            base = self.rng.randint(14, 28)
            bonus = self.intel // 12
            stolen = base + bonus
            self.loot += stolen
            self.intel = max(0, self.intel - self.rng.randint(8, 15))
            steal_heat = min(18, max(0, self.consecutive_steals - 1) * 6)
            self.alert = min(self.max_alert, self.alert + self.rng.randint(16, 26) + steal_heat)
            self._apply_security_pressure(action)
            return self._finish_turn(f"Robaste ${stolen}M en activos de alto valor.")

        if action == HeistAction.HIDE:
            self.consecutive_steals = 0
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

    def _apply_security_pressure(self, action: HeistAction) -> None:
        if action == HeistAction.HIDE:
            return

        if self.turns_played < 6:
            pressure = 0
        elif self.turns_played < 12:
            pressure = 1
        elif self.turns_played < 18:
            pressure = 2
        else:
            pressure = 3

        if pressure > 0:
            self.alert = min(self.max_alert, self.alert + pressure)

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
