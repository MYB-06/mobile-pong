using UnityEngine;

namespace PongGame.Core
{
    public enum Difficulty { Easy, Medium, Hard }
    public static class DifficultySettings 
    {
        private const string DIFFICULTY_KEY = "GameDifficulty";

        public static (float reactionDelay, float reactionSpeed, float smoothSpeed) GetAISettings(Difficulty difficulty)
        {
            return difficulty switch 
            {
                Difficulty.Easy => (1f, 0.4f, 5f),
                Difficulty.Medium => (0.4f, 0.6f, 7f),
                Difficulty.Hard => (0.1f, 1f, 10f),
                _ => (0.2f, 0.8f, 7f) 
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
