using System.Collections.Generic;
using UnityEngine;

namespace Constants
{
    public static class Colors
    {
        public static class DifficultyColors
        {
            public struct DifficultySetting
            {
                public DifficultyLevel Difficulty;
                public Color32 Color;
            }

            private static readonly List<DifficultySetting> SETTINGS = new List<DifficultySetting>() {
                new DifficultySetting() {
                    Difficulty = DifficultyLevel.Easy,
                    Color = new Color32(0x0, 0xff, 0x6a, 0xb8)
                },
                new DifficultySetting() {
                    Difficulty = DifficultyLevel.Hard,
                    Color = new Color32(0xff, 0xd6, 0x0, 0xb8)
                },
                new DifficultySetting() {
                    Difficulty = DifficultyLevel.Hell,
                    Color = new Color32(0xff, 0x0, 0x93, 0xb8)
                },
            };

            /// <param name="level">The corresponding difficulty level</param>
            /// <returns>The theme color of the specified difficulty level.</returns>
            public static Color Get(DifficultyLevel level) {
                return SETTINGS.Find(x => x.Difficulty == level).Color;
            }
        }
    }
}