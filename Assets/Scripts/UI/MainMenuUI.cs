using PongGame.Audio;
using PongGame.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PongGame.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject difficultyPanel;
        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button soundButton;
        [SerializeField] private Button musicButton;
        [SerializeField] private Button keyboardButton;
        [SerializeField] private Button mouseButton;
        [SerializeField] private Button backButton;
        [Header("Difficulty Buttons")]
        [SerializeField] private Button easyButton;
        [SerializeField] private Button mediumButton;
        [SerializeField] private Button hardButton;
        [SerializeField] private Button difficultyBackButton;
        [Header("Difficulty Visual Feedback")]
        [SerializeField] private RectTransform difficultyArrows;
        [Header("Button Icons")]
        [SerializeField] private Image soundButtonImage;
        [SerializeField] private Image musicButtonImage;
        [Header("Icon Sprites")]
        [SerializeField] private Sprite soundOnIcon;
        [SerializeField] private Sprite soundOffIcon;
        [SerializeField] private Sprite musicOnIcon;
        [SerializeField] private Sprite musicOffIcon;
        void Start()
        {
            AudioManager.Instance?.SetMenuState(true);
            SetupButtons();
            UpdateIcon();
            ConfigurePlatformUI();
            UpdateDifficultyArrows();
        }
        private void SetupButtons()
        {
            playButton.onClick.AddListener(OnPlayClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);

            soundButton.onClick.AddListener(OnSoundToggle);
            musicButton.onClick.AddListener(OnMusicToggle);

            keyboardButton.onClick.AddListener(OnKeyboardSelected);
            mouseButton.onClick.AddListener(OnMouseSelected);

            backButton.onClick.AddListener(OnBackClicked);

            easyButton.onClick.AddListener(()=> OnDifficultySelected(Difficulty.Easy));
            mediumButton.onClick.AddListener(()=> OnDifficultySelected(Difficulty.Medium));
            hardButton.onClick.AddListener(()=> OnDifficultySelected(Difficulty.Hard));
            difficultyBackButton.onClick.AddListener(OnDifficultyBackClicked);
        }
        private void OnPlayClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            mainMenuPanel.SetActive(false);
            difficultyPanel.SetActive(true);
            UpdateDifficultyArrows();
        }
        private void OnSettingsClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            mainMenuPanel.SetActive(false);
            settingsPanel.SetActive(true);
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
        private void OnKeyboardSelected()
        {
            AudioManager.Instance?.PlayButtonClick();
            Debug.Log("Keyboard selected");
        }
        private void OnMouseSelected()
        {
            AudioManager.Instance?.PlayButtonClick();
            Debug.Log("Mouse selected");
        }
        private void OnBackClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            settingsPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
        private void OnDifficultySelected(Difficulty difficulty)
        {
            AudioManager.Instance?.PlayButtonClick();
            DifficultySettings.SaveDifficulty(difficulty);
            UpdateDifficultyArrows();

            StartCountdown();
        }
        private void UpdateDifficultyArrows()
        {
            Difficulty current = DifficultySettings.LoadDifficulty();

            float yPos = current switch
            {
                Difficulty.Easy => 0f,
                Difficulty.Medium => -150f,
                Difficulty.Hard => -300f,
                _ => -150f
            };

            Vector2 currentPos = difficultyArrows.anchoredPosition;
            difficultyArrows.anchoredPosition = new Vector2(currentPos.x, yPos); 
        }
        private void StartCountdown()
        {
            AudioManager.Instance.SetMenuState(false);
            SceneManager.LoadScene("GameScene");
        }
        private void OnDifficultyBackClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            difficultyPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
        private void UpdateIcon()
        {
            UpdateMusicIcon();
            UpdateSoundIcon();
        }
        private void UpdateSoundIcon()
        {
            bool sfxEnabled = AudioManager.Instance.IsSFXEnabled();
            soundButtonImage.sprite = sfxEnabled ? soundOnIcon : soundOffIcon;
        }
        private void UpdateMusicIcon()
        {
            bool musicEnabled = AudioManager.Instance.IsMusicEnabled();
            musicButtonImage.sprite = musicEnabled ? musicOnIcon : musicOffIcon;
        }
        private void ConfigurePlatformUI()
        {
            #if UNITY_ANDROID
                keyboardButton.gameObject.SetActive(false);
                mouseButton.gameObject.SetActive(false);

                RectTransform soundRT = soundButton.GetComponent<RectTransform>();
                RectTransform musicRT = musicButton.GetComponent<RectTransform>();

                soundRT.anchoredPosition = new Vector2(soundRT.anchoredPosition.x, 0);
                musicRT.anchoredPosition = new Vector2(musicRT.anchoredPosition.x, 0);
            #endif
        }
    }
}

