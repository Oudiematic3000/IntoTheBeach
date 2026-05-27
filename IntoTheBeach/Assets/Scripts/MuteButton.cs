using UnityEngine;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    [SerializeField] Sprite muted, unmuted;
    [SerializeField] Image image;
    bool musicMuted = false;
    bool sfxMuted = false;
    public void ToggleMuteMusic()
    {
        if (!musicMuted)
        {
            image.sprite=muted;
            AudioManager.instance.MuteMusic();
        }
        else
        {
            image.sprite=unmuted;
            AudioManager.instance.UnmuteMusic();
        }
        musicMuted = !musicMuted;

    }
    public void ToggleMuteSfx()
    {
        if (!sfxMuted)
        {
            image.sprite=muted;
            AudioManager.instance.MuteSfx();
        }
        else
        {
            image.sprite=unmuted;
            AudioManager.instance.UnmuteSfx();
        }
        sfxMuted= !sfxMuted;
    }
}
