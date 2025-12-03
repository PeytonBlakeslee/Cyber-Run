using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;    // for jump/death
    [SerializeField] private AudioSource musicSource;  // for menu + game music

    [Header("SFX Clips")]
    public AudioClip jumpClip;
    public AudioClip deathClip;

    [Header("Music Clips")]
    public AudioClip menuMusicClip;
    public AudioClip gameMusicClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    // SFX
    public void PlayJump()
    {
        if (sfxSource && jumpClip)
            sfxSource.PlayOneShot(jumpClip);
    }

    public void PlayDeath()
    {
        if (sfxSource && deathClip)
            sfxSource.PlayOneShot(deathClip);
    }

    // Music
    public void PlayMenuMusic()
    {
        if (!musicSource || !menuMusicClip)
            return;

        if (musicSource.clip != menuMusicClip)
            musicSource.clip = menuMusicClip;

        if (!musicSource.isPlaying)
            musicSource.Play();
    }

    public void PlayGameMusic()
    {
        if (!musicSource || !gameMusicClip)
            return;

        if (musicSource.clip != gameMusicClip)
            musicSource.clip = gameMusicClip;

        if (!musicSource.isPlaying)
            musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource && musicSource.isPlaying)
            musicSource.Stop();
    }

    public bool IsMuted { get; private set; } = false;
    // Toggle all game audio on/off
    public void ToggleMute()
    {
    IsMuted = !IsMuted;
    AudioListener.volume = IsMuted ? 0f : 1f;
    }

}
