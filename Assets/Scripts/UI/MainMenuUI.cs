using PongGame.Audio;
using PongGame.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using PongGame.Utilities;

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
        [Header("Animation Settings")]
        [SerializeField] private float panelAnimDuration = 0.4f;
        [SerializeField] private float buttonAnimDuration = 0.4f;
        [SerializeField] private float buttonPunchScale = 0.1f;
        [SerializeField] private float arrowMoveDuration = 0.3f;
        
        private Dictionary<GameObject, (CanvasGroup canvasGroup, RectTransform rectTransform)> _panelCache;
        
        void Start()
        {
            AudioManager.Instance?.SetMenuState(true);
            CachePanelComponents();
            SetupButtons();
            UpdateIcon();
            UpdateDifficultyArrows();
        }
        
        private void CachePanelComponents()
        {
            _panelCache = new Dictionary<GameObject, (CanvasGroup, RectTransform)>
            {
                { mainMenuPanel, (mainMenuPanel.GetComponent<CanvasGroup>(), mainMenuPanel.GetComponent<RectTransform>()) },
                { settingsPanel, (settingsPanel.GetComponent<CanvasGroup>(), settingsPanel.GetComponent<RectTransform>()) },
                { difficultyPanel, (difficultyPanel.GetComponent<CanvasGroup>(), difficultyPanel.GetComponent<RectTransform>()) }
            };
        }
        
        private void SetupButtons()
        {
            playButton.onClick.AddListener(OnPlayClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);

            soundButton.onClick.AddListener(OnSoundToggle);
            musicButton.onClick.AddListener(OnMusicToggle);

            backButton.onClick.AddListener(OnBackClicked);

            easyButton.onClick.AddListener(()=> OnDifficultySelected(Difficulty.Easy));
            mediumButton.onClick.AddListener(()=> OnDifficultySelected(Difficulty.Medium));
            hardButton.onClick.AddListener(()=> OnDifficultySelected(Difficulty.Hard));
            difficultyBackButton.onClick.AddListener(OnDifficultyBackClicked);
        }
        #region Button
        private void OnPlayClicked()
        {
            AnimateButton(playButton);
            AudioManager.Instance?.PlayButtonClick();
            AnimatePanelTransition(mainMenuPanel, difficultyPanel);
        }
        
        private void OnSettingsClicked()
        {
            AnimateButton(settingsButton);
            AudioManager.Instance?.PlayButtonClick();
            AnimatePanelTransition(mainMenuPanel, settingsPanel);
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
        
        private void OnBackClicked()
        {
            AnimateButton(backButton);
            AudioManager.Instance?.PlayButtonClick();
            AnimatePanelTransition(settingsPanel, mainMenuPanel);
        }
        
        private void OnDifficultySelected(Difficulty difficulty)
        {
            Button selectedButton = difficulty switch
            {
                Difficulty.Easy => easyButton,
                Difficulty.Medium => mediumButton,
                Difficulty.Hard => hardButton,
                _ => mediumButton
            };
            
            AnimateButton(selectedButton);
            AudioManager.Instance?.PlayButtonClick();
            DifficultySettings.SaveDifficulty(difficulty);
            
            Sequence sequence = DOTween.Sequence();
            sequence.Append(UpdateDifficultyArrowsWithTween());
            sequence.AppendCallback(StartCountdown);
        }       
        
        private void UpdateDifficultyArrows()
        {
            UpdateDifficultyArrowsWithTween();
        }
        
        private void StartCountdown()
        {
            AudioManager.Instance.SetMenuState(false);
            SceneManager.LoadScene("GameScene");
        }
        
        private void OnDifficultyBackClicked()
        {
            AnimateButton(difficultyBackButton);
            AudioManager.Instance?.PlayButtonClick();
            AnimatePanelTransition(difficultyPanel, mainMenuPanel);
        }
        #endregion
        
        private void UpdateIcon()
        {
            AudioUIHelper.UpdateSoundIcon(soundButtonImage, soundOnIcon, soundOffIcon);
            AudioUIHelper.UpdateMusicIcon(musicButtonImage, musicOnIcon, musicOffIcon);
        }      

        #region Animation
        private void AnimatePanelTransition(GameObject fromPanel, GameObject toPanel)
        {
            Sequence sequence = DOTween.Sequence();
            
            var fromCache = _panelCache[fromPanel];
            var toCache = _panelCache[toPanel];
            
            if (fromCache.canvasGroup != null)
            {
                sequence.Append(fromCache.canvasGroup.DOFade(0f, panelAnimDuration * 0.5f));
            }
            
            sequence.AppendCallback(() =>
            {
                fromPanel.SetActive(false);
                if (fromCache.canvasGroup != null) fromCache.canvasGroup.alpha = 1f;
                
                toPanel.SetActive(true);
            });
            
            if (toCache.canvasGroup != null && toCache.rectTransform != null)
            {
                toCache.canvasGroup.alpha = 0f;
                toCache.rectTransform.localScale = Vector3.zero;
                
                sequence.Append(toCache.canvasGroup.DOFade(1f, panelAnimDuration).SetEase(Ease.OutQuad));
                sequence.Join(toCache.rectTransform.DOScale(1f, panelAnimDuration).SetEase(Ease.OutBack));
            }
        }
        private void AnimateButton(Button button)
        {
            button.transform.DOPunchScale(Vector3.one * buttonPunchScale, buttonAnimDuration, 5);
        }
        private Tween UpdateDifficultyArrowsWithTween()
        {
            Difficulty current = DifficultySettings.LoadDifficulty();

            float yPos = current switch
            {
                Difficulty.Easy => 0f,
                Difficulty.Medium => -150f,
                Difficulty.Hard => -300f,
                _ => 0f
            };

            return difficultyArrows.DOAnchorPosY(yPos, arrowMoveDuration).SetEase(Ease.OutCubic);
        }
        #endregion
    }
}