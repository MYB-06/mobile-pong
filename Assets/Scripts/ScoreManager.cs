using System;
using UnityEngine;

namespace PongGame.Core
{
    public class ScoreManager
    {
        public int CurrentScore {get; private set;}
        public int HighScore {get; private set;}
        
        public ScoreManager()
        {
            LoadHighScore();
        }
        public void AddScore()
        {
            CurrentScore++;
            CheckAndSaveHighScore();
        }
        public void ResetScore()
        {
            CurrentScore = 0;
        }

        private void CheckAndSaveHighScore()
        {
            if (CurrentScore > HighScore)
            {
                HighScore = CurrentScore;
                SaveHighScore();
            }
        }

        private void SaveHighScore()
        {
            PlayerPrefs.SetInt("HighScore", HighScore);
            PlayerPrefs.Save();
        }

        private void LoadHighScore()
        {
            HighScore = PlayerPrefs.GetInt("HighScore", 0);
        }
    }
}
