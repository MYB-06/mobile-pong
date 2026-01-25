using DG.Tweening;
using PongGame.Audio;
using PongGame.Core;
using PongGame.Utilities;
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
        [Header("Fade")]
        [SerializeField] private CanvasGroup fade;
        [Header("Animation Settings")]
        [SerializeField] private float panelAnimDuration;
        [SerializeField] private float buttonAnimDuration;
        [SerializeField] private float buttonPunchScale;
        [SerializeField] private float bestAnimDuration;
        [SerializeField] private float bestPunchScale;
        [SerializeField] private float fadeDuration;
        private CanvasGroup _pauseGroup;
        private CanvasGroup _gameOverGroup;
        private RectTransform _pauseRect;
        private RectTransform _gameOverRect;
        void Start()
        {
            CachePanelComponents();
            HidePanels();
            SetupButtons();
            UpdateIcon();
            InitializeFade();
        }
        private void CachePanelComponents()
        {
            _pauseGroup = pausePanel.GetComponent<CanvasGroup>();
            _pauseRect = pausePanel.GetComponent<RectTransform>();
            _gameOverGroup = gameOverPanel.GetComponent<CanvasGroup>();
            _gameOverRect = gameOverPanel.GetComponent<RectTransform>();
        }
        private void SetupButtons()
        {
            pauseButton.onClick.AddListener(OnPauseClicked);
            
            soundButton.onClick.AddListener(OnSoundToggle);
            musicButton.onClick.AddListener(OnMusicToggle);
            resumeButton.onClick.AddListener(OnResumeClicked);
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            
            retryButton.onClick.AddListener(OnRetryClicked);
            gameOverMainMenuButton.onClick.AddListener(OnGameOverMainMenuClicked);
        }
        private void HidePanels()
        {
            pausePanel.SetActive(false);
            gameOverPanel.SetActive(false);
        }
         private void InitializeFade()
        {
            if (fade != null)
            {
                fade.alpha = 0f;
                fade.gameObject.SetActive(true);
            }
        }

        #region Button
        private void OnPauseClicked()
        {
            AnimateButton(pauseButton);
            AudioManager.Instance?.PlayButtonClick();
            Time.timeScale = 0;
            pausePanel.SetActive(true);
            AnimatePanel(_pauseGroup, _pauseRect, true);
        }
        private void OnSoundToggle()
        {
            AnimateButton(soundButton);
            AudioManager.Instance?.PlayButtonClick();
            AudioManager.Instance?.ToggleSFX();
            UpdateIcon();
        }
        private void OnMusicToggle()
        {
            AnimateButton(musicButton);
            AudioManager.Instance?.PlayButtonClick();
            AudioManager.Instance?.ToggleMusic();
            UpdateIcon();
        }
        private void OnResumeClicked()
        {
            AnimateButton(resumeButton);
            AudioManager.Instance?.PlayButtonClick();
            
            Sequence sequence = DOTween.Sequence();
            sequence.SetUpdate(true);
            sequence.AppendInterval(buttonAnimDuration);
            sequence.Append(AnimatePanel(_pauseGroup, _pauseRect, false));
            sequence.AppendCallback(() =>
            {
                Time.timeScale = 1f;
                pausePanel.SetActive(false);
                _pauseGroup.alpha = 1f;
            });
        }
        private void OnMainMenuClicked()
        {
            AnimateButton(mainMenuButton);
            AudioManager.Instance?.PlayButtonClick();
            
            LoadMainMenuWithFade();
        }
        private void OnGameOverMainMenuClicked()
        {
            AnimateButton(gameOverMainMenuButton);
            AudioManager.Instance?.PlayButtonClick();
            
            LoadMainMenuWithFade();
        }
        private void OnRetryClicked()
        {
            AnimateButton(retryButton);
            AudioManager.Instance?.PlayButtonClick();

            Sequence sequence = DOTween.Sequence();
            sequence.SetUpdate(true);
            sequence.AppendInterval(buttonAnimDuration);
            sequence.Append(AnimatePanel(_gameOverGroup, _gameOverRect, false));
            sequence.AppendCallback(() =>
            {
                Time.timeScale = 1f;
                GameManager.Instance.RestartGame();
                HidePanels();
            });
        }
        #endregion
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

            Sequence sequence = DOTween.Sequence();
            sequence.SetUpdate(true);
            sequence.Append(AnimatePanel(_gameOverGroup, _gameOverRect, true));

            if (isNewRecord)
            {
                sequence.AppendCallback(() =>
                {
                    titleText.transform.DOPunchScale(Vector3.one * bestPunchScale, bestAnimDuration, 3).SetUpdate(true);
                });
            } 
        }
        private void UpdateIcon()
        {
            AudioUIHelper.UpdateSoundIcon(soundButtonImage, soundOnIcon, soundOffIcon);
            AudioUIHelper.UpdateMusicIcon(musicButtonImage, musicOnIcon, musicOffIcon);
        }
        #region Animation
        private Tween AnimatePanel(CanvasGroup cg, RectTransform rt, bool fadeIn = true)
        {
            if (cg == null || rt == null) return null;

            if (fadeIn)
            {
                cg.alpha = 0f;
                rt.localScale = Vector3.zero;

                rt.DOScale(1f, panelAnimDuration).SetEase(Ease.OutBack).SetUpdate(true);
                return cg.DOFade(1f, panelAnimDuration).SetEase(Ease.OutQuad).SetUpdate(true);
            }
            else
            {
                rt.DOScale(0f, panelAnimDuration * 0.5f).SetEase(Ease.InBack).SetUpdate(true);
                return cg.DOFade(0f, panelAnimDuration * 0.5f).SetUpdate(true);
            }        
        }
        private void AnimateButton(Button button)
        {
            button.transform.DOPunchScale(Vector3.one * buttonPunchScale, buttonAnimDuration, 5).SetUpdate(true);
        }
         private void LoadMainMenuWithFade()
        {
            if (fade == null) 
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("MainMenu");
                return;
            }
            
            Sequence sequence = DOTween.Sequence();
            sequence.SetUpdate(true);
            sequence.AppendInterval(buttonAnimDuration);
            sequence.Append(fade.DOFade(1f, fadeDuration));
            sequence.AppendCallback(() =>
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("MainMenu");
            });
        }  
        #endregion
    }  
}