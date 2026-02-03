using PongGame.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace PongGame.Utilities
{
    public static class AudioUIHelper
    {
        public static void UpdateSoundIcon(Image iconImage, Sprite onSprite, Sprite offSprite)
        {
            bool sfxEnabled = AudioManager.Instance.IsSFXEnabled();
            iconImage.sprite = sfxEnabled ? onSprite : offSprite;
        }
        public static void UpdateMusicIcon(Image iconImage, Sprite onSprite, Sprite offSprite)
        {
            bool musicEnabled = AudioManager.Instance.IsMusicEnabled();
            iconImage.sprite = musicEnabled ? onSprite : offSprite;
        }
    }
}
