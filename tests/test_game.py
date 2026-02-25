import random
import unittest

from fortune_heist.game import FortuneHeistGame, HeistAction
from fortune_heist.simulation import simulate_actions


class FortuneHeistGameTests(unittest.TestCase):
    def test_scout_increases_intel_and_alert(self):
        game = FortuneHeistGame(rng=random.Random(1))

        result = game.apply_action(HeistAction.SCOUT)

        self.assertIn("Reconociste", result.message)
        self.assertGreater(game.intel, 0)
        self.assertGreater(game.alert, 0)

    def test_hide_reduces_alert(self):
        game = FortuneHeistGame(rng=random.Random(2), alert=30)

        result = game.apply_action(HeistAction.HIDE)

        self.assertIn("redujiste", result.message)
        self.assertLess(game.alert, 30)

    def test_win_condition_when_target_loot_reached(self):
        game = FortuneHeistGame(rng=random.Random(3), target_loot=20, intel=100)

        result = game.apply_action(HeistAction.STEAL)

        self.assertTrue(result.won)
        self.assertTrue(game.has_won)

    def test_loss_condition_when_alert_caps(self):
        game = FortuneHeistGame(rng=random.Random(4), alert=95)

        result = game.apply_action(HeistAction.STEAL)

        self.assertTrue(result.lost)
        self.assertTrue(game.has_lost)

    def test_no_actions_after_game_ends(self):
        game = FortuneHeistGame(rng=random.Random(5), alert=100)

        result = game.apply_action(HeistAction.HIDE)

        self.assertTrue(result.lost)
        self.assertIn("ya terminó", result.message)

    def test_simulation_returns_unity_friendly_payload(self):
        preview = simulate_actions(["scout", "steal", "hide"], seed=7)

        self.assertEqual(preview["seed"], 7)
        self.assertEqual(len(preview["turns"]), 3)
        self.assertIn("final", preview)
        self.assertIn("status_line", preview["final"])

    def test_simulation_maps_invalid_action_to_hide(self):
        preview = simulate_actions(["invalid"], seed=9)

        first_turn = preview["turns"][0]
        self.assertFalse(first_turn["valid_input"])
        self.assertEqual(first_turn["applied_action"], "hide")


if __name__ == "__main__":
    unittest.main()
