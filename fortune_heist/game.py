from __future__ import annotations

from dataclasses import asdict, dataclass, field
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
class BalanceTuning:
    scout_intel_min: int = 15
    scout_intel_max: int = 30
    scout_alert_min: int = 5
    scout_alert_max: int = 12

    hack_success_base: int = 40
    hack_intel_divisor: int = 2
    hack_success_alert_reduction_min: int = 10
    hack_success_alert_reduction_max: int = 20
    hack_failure_alert_min: int = 15
    hack_failure_alert_max: int = 25

    steal_base_min: int = 14
    steal_base_max: int = 28
    steal_intel_divisor: int = 12
    steal_intel_consumption_min: int = 8
    steal_intel_consumption_max: int = 15
    steal_alert_min: int = 16
    steal_alert_max: int = 26
    steal_heat_per_chain: int = 6
    steal_heat_cap: int = 18

    hide_alert_reduction_min: int = 12
    hide_alert_reduction_max: int = 22

    security_pressure_curve: tuple[int, int, int, int] = (0, 1, 2, 3)
    security_pressure_steps: tuple[int, int, int] = (6, 12, 18)


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
    tuning: BalanceTuning = field(default_factory=BalanceTuning)

    def apply_action(self, action: HeistAction) -> GameResult:
        if self.is_over:
            return GameResult("La partida ya terminó.", won=self.has_won, lost=self.has_lost)

        self.turns_played += 1

        if action == HeistAction.SCOUT:
            self.consecutive_steals = 0
            self.intel = min(100, self.intel + self.rng.randint(self.tuning.scout_intel_min, self.tuning.scout_intel_max))
            self.alert = min(self.max_alert, self.alert + self.rng.randint(self.tuning.scout_alert_min, self.tuning.scout_alert_max))
            self._apply_security_pressure(action)
            return self._finish_turn("Reconociste la bóveda y reuniste información.")

        if action == HeistAction.HACK:
            self.consecutive_steals = 0
            success_chance = self.tuning.hack_success_base + self.intel // max(1, self.tuning.hack_intel_divisor)
            roll = self.rng.randint(1, 100)
            if roll <= success_chance:
                self.alert = max(
                    0,
                    self.alert - self.rng.randint(self.tuning.hack_success_alert_reduction_min, self.tuning.hack_success_alert_reduction_max),
                )
                self.intel = max(0, self.intel - 10)
                self._apply_security_pressure(action)
                return self._finish_turn("Hack exitoso: bajaste el nivel de alerta.")
            self.alert = min(self.max_alert, self.alert + self.rng.randint(self.tuning.hack_failure_alert_min, self.tuning.hack_failure_alert_max))
            self._apply_security_pressure(action)
            return self._finish_turn("Hack fallido: los sistemas detectaron actividad.")

        if action == HeistAction.STEAL:
            self.consecutive_steals += 1
            base = self.rng.randint(self.tuning.steal_base_min, self.tuning.steal_base_max)
            bonus = self.intel // max(1, self.tuning.steal_intel_divisor)
            stolen = base + bonus
            self.loot += stolen
            self.intel = max(0, self.intel - self.rng.randint(self.tuning.steal_intel_consumption_min, self.tuning.steal_intel_consumption_max))
            steal_heat = min(
                self.tuning.steal_heat_cap,
                max(0, self.consecutive_steals - 1) * self.tuning.steal_heat_per_chain,
            )
            self.alert = min(self.max_alert, self.alert + self.rng.randint(self.tuning.steal_alert_min, self.tuning.steal_alert_max) + steal_heat)
            self._apply_security_pressure(action)
            return self._finish_turn(f"Robaste ${stolen}M en activos de alto valor.")

        if action == HeistAction.HIDE:
            self.consecutive_steals = 0
            reduced = self.rng.randint(self.tuning.hide_alert_reduction_min, self.tuning.hide_alert_reduction_max)
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

        step1, step2, step3 = self.tuning.security_pressure_steps
        p0, p1, p2, p3 = self.tuning.security_pressure_curve

        if self.turns_played < step1:
            pressure = p0
        elif self.turns_played < step2:
            pressure = p1
        elif self.turns_played < step3:
            pressure = p2
        else:
            pressure = p3

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

    def tuning_snapshot(self) -> dict:
        return asdict(self.tuning)
