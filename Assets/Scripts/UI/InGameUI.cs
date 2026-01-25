using PongGame.Audio;
using PongGame.Core;
using PongGame.Utilies;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PongGame.UI
{
    public class InGameUI : MonoBehaviour
    {
        [Header("Gameplay UI")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private Button pauseButton;
        [Header("Pause Panel")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button soundButton;
        [SerializeField] private Button musicButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button mainMenuButton;
        [Header("Button Icons")]
        [SerializeField] private Image soundButtonImage;
        [SerializeField] private Image musicButtonImage;
        [Header("Icon Sprites")]
        [SerializeField] private Sprite soundOnIcon;
        [SerializeField] private Sprite soundOffIcon;
        [SerializeField] private Sprite musicOnIcon;
        [SerializeField] private Sprite musicOffIcon;
        [Header("Game Over Panel")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI gameOverHighScoreText;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button gameOverMainMenuButton;
        void Start()
        {
            HidePanels();
            SetupButtons();
            UpdateIcon();
        }
        private void SetupButtons()
        {
            pauseButton.onClick.AddListener(OnPauseClicked);
            
            soundButton.onClick.AddListener(OnSoundToggle);
            musicButton.onClick.AddListener(OnMusicToggle);
            resumeButton.onClick.AddListener(OnResumeClicked);
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            
            retryButton.onClick.AddListener(OnRetryClicked);
            gameOverMainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }
        private void HidePanels()
        {
            pausePanel.SetActive(false);
            gameOverPanel.SetActive(false);
        }
        private void OnPauseClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            Time.timeScale = 0;
            pausePanel.SetActive(true);
        }
        private void OnSoundToggle()
        {
            AudioManager.Instance?.PlayButtonClick();
            AudioManager.Instance?.ToggleSFX();
            UpdateIcon();
        }
        private void OnMusicToggle()
        {
            AudioManager.Instance?.PlayButtonClick();
            AudioManager.Instance?.ToggleMusic();
            UpdateIcon();
        }
        private void OnResumeClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
        }
        private void OnMainMenuClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
        private void OnRetryClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            Time.timeScale = 1f;
            GameManager.Instance.RestartGame();
            HidePanels();
        }
        public void UpdateScore(int currentScore, int highScore)
        {
            scoreText.text = $"Score: {currentScore}";
            highScoreText.text = $"Best: {highScore}";
        }
        public void ShowGameOver(int finalScore, int highScore, bool isNewRecord)
        {
            gameOverPanel.SetActive(true);

            titleText.text = isNewRecord ? "New Record!" : "Game Over!";
            titleText.color = isNewRecord ? Color.orange : Color.white;

            finalScoreText.text = $"Score: {finalScore}";
            gameOverHighScoreText.text = $"Best: {highScore}";
        }
        private void UpdateIcon()
        {
            AudioUIHelper.UpdateSoundIcon(soundButtonImage, soundOnIcon, soundOffIcon);
            AudioUIHelper.UpdateMusicIcon(musicButtonImage, musicOnIcon, musicOffIcon);
        }   
    }  
}
