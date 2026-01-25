using PongGame.Audio;
using PongGame.Gameplay;
using PongGame.UI;
using TMPro;
using UnityEngine;

namespace PongGame.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance {get; private set;}

        [Header("References")]
        [SerializeField] private Ball ball;
        [SerializeField] private InGameUI inGameUI;

        private ScoreManager _scoreManager;

        public int CurrentScore => _scoreManager.CurrentScore;
        public int HighScore => _scoreManager.HighScore;

        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _scoreManager = new ScoreManager();

            if(inGameUI != null)
            {
                inGameUI.UpdateScore(_scoreManager.CurrentScore, _scoreManager.HighScore);
            }
        }

        public void OnPlayerScored()
        {
            _scoreManager.AddScore();
            AudioManager.Instance.PlayGoalClip();
            if (inGameUI != null)
            {
                inGameUI.UpdateScore(_scoreManager.CurrentScore, _scoreManager.HighScore);
            }

            ResetBall();
        }
        
        public void OnAIScored()
        {
            Time.timeScale = 0;
            AudioManager.Instance.PlayGameOver();
    
            if (inGameUI != null)
            {
                inGameUI.ShowGameOver(_scoreManager.CurrentScore, _scoreManager.HighScore, _scoreManager.IsNewHighScore);
            }
        }
        public void RestartGame()
        {
            Time.timeScale = 1;
            _scoreManager.ResetScore();

            if (inGameUI != null)
            {
                inGameUI.UpdateScore(_scoreManager.CurrentScore, _scoreManager.HighScore);
            }

            ResetBall();
        }
        private void ResetBall()
        {
            if (ball != null) ball.ResetPosition();
        }
    }
}