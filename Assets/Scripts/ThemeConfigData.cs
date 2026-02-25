using System;

namespace FortuneHeist
{
    [Serializable]
    public class ThemeConfigData
    {
        public string PositiveHex = "#6CFF8A";
        public string NeutralHex = "#FFFFFF";
        public string WarningHex = "#FF9A4D";
        public string AccentHex = "#8ED0FF";

        public float ResultPulseDuration = 0.2f;

        public static ThemeConfigData Default()
        {
            return new ThemeConfigData();
        }
    }
}
