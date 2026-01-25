using PongGame.Audio;
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
        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button soundButton;
        [SerializeField] private Button musicButton;
        [SerializeField] private Button keyboardButton;
        [SerializeField] private Button mouseButton;
        [SerializeField] private Button backButton;
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
        }
        private void OnPlayClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            AudioManager.Instance?.SetMenuState(false);
            SceneManager.LoadScene("GameScene");
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

