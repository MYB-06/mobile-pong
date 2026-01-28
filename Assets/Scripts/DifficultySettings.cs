using UnityEngine;

namespace PongGame.Core
{
    public enum Difficulty { Easy, Medium, Hard }
    public static class DifficultySettings 
    {
        private const string DIFFICULTY_KEY = "GameDifficulty";

        public static (float reactionSpeed, float smoothSpeed, float predictionAccuracy) GetAISettings(Difficulty difficulty)
        {
            return difficulty switch 
            {
                Difficulty.Easy => (0.5f, 5f, 0.3f),
                Difficulty.Medium => (0.7f, 5f, 0.5f),
                Difficulty.Hard => (1f, 7f, 0.7f),
                _ => (0.7f, 5f, 0.5f) 
            };
        }
        public static void SaveDifficulty(Difficulty difficulty)
        {
            PlayerPrefs.SetInt(DIFFICULTY_KEY, (int)difficulty);
            PlayerPrefs.Save();
        }

        public static Difficulty LoadDifficulty()
        {
            return (Difficulty)PlayerPrefs.GetInt(DIFFICULTY_KEY, 1);
        }
    }
}
