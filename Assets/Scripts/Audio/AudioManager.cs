using UnityEngine;

namespace PongGame.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance {get; private set;}

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip paddleHitClip;
        [SerializeField] private AudioClip wallHitClip;
        [SerializeField] private AudioClip goalClip;
        [SerializeField] private AudioClip gameOverClip;
        [SerializeField] private AudioClip buttonClickClip;

        [Header("Settings")]
        [SerializeField] private bool musicEnabled = true;
        [SerializeField] private bool sfxEnabled = true;

        private bool _isInMenu = true;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadSettings();
        }
        private void Start()
        {
            if(musicEnabled && musicSource != null)
            {
                musicSource.Play();
            }
        }
        public void PlayPaddleHit()
        {
            if (_isInMenu) return;
            PlaySFX(paddleHitClip);
        }
        public void PlayWallHit()
        {
            if (_isInMenu) return;
            PlaySFX(wallHitClip);
        }
        public void PlayButtonClick()
        {
            PlaySFX(buttonClickClip);
        }
        public void PlayGoalClip()
        {
            PlaySFX(goalClip);
        }
        public void PlayGameOver()
        {
            PlaySFX(gameOverClip);
        }
        public void SetMenuState(bool inMenu)
        {
            _isInMenu = inMenu;
        }
        private void PlaySFX(AudioClip clip)
        {
            if (sfxEnabled && sfxSource != null && clip != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }
        public void ToggleMusic()
        {
            musicEnabled = !musicEnabled;
            
            if(musicSource != null)
            {
                if(musicEnabled) musicSource.Play();
                else musicSource.Pause();
            }

            SaveSettings();
        }
        public void ToggleSFX()
        {
            sfxEnabled = !sfxEnabled;
            SaveSettings();
        }
        private void SaveSettings()
        {
            PlayerPrefs.SetInt("MusicEnabled", musicEnabled ? 1 : 0);
            PlayerPrefs.SetInt("SFXEnabled", sfxEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }
        private void LoadSettings()
        {
            musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
            sfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
        }
        public bool IsSFXEnabled()
        {
            return sfxEnabled;
        }

        public bool IsMusicEnabled()
        {       
            return musicEnabled;
        }
    }  
}