using PongGame.Audio;
using PongGame.Gameplay;
using TMPro;
using UnityEngine;

namespace PongGame.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance {get; private set;}

        [Header("References")]
        [SerializeField] private Ball ball;

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
        }

        public void OnPlayerScored()
        {
            _scoreManager.AddScore();
            AudioManager.Instance.PlayGoalClip();
            ResetBall();
        }
        
        public void OnAIScored()
        {
            Time.timeScale = 0;

            // Game Over

            AudioManager.Instance.PlayGameOver();
            Debug.Log($"Game Over! Final Score: {CurrentScore}");
        }
        public void RestartGame()
        {
            Time.timeScale = 1;
            _scoreManager.ResetScore();
            ResetBall();
        }
        private void ResetBall()
        {
            if (ball != null) ball.ResetPosition();
        }
    }
}