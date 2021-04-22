using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    [SerializeField]
    private AudioClip musicClip, menuMusicClip;
    [SerializeField]
    private AudioClip playerDamageClip, enemyDamagedClip, enemyDestroyedClip, menuItemClick;
    [SerializeField]
    private AudioSource musicSource, sfxSource;
    [SerializeField]
    private AudioMixer musicMixer, sfxMixer;

    // Start is called before the first frame update
    void Start()
    {
        musicSource.clip = menuMusicClip;
        musicSource.Play();
    }

    #region Music
    public void PlayMusic()
    {
        musicSource.clip = musicClip;
        musicSource.Play();
    }

    public void PlayMenuMusic()
    {
        musicSource.clip = menuMusicClip;
        musicSource.Play();
    }
    #endregion

    #region SFX
    private void PlaySFX(AudioClip audioClip)
    {
        sfxSource.clip = audioClip;
        sfxSource.Play();
    }

    public void ClickSound()
    {
        PlaySFX(menuItemClick);
    }
    public void PlayerDamageSound()
    {
        PlaySFX(playerDamageClip);
    }
    public void EnemyDamagedSound()
    {
        PlaySFX(enemyDamagedClip);
    }
    public void EnemyDestroyedSound()
    {
        PlaySFX(enemyDestroyedClip);
    }
    #endregion

    #region SoundSettings
    public void SetMusicVolume(float volumeSet)
    {
        musicMixer.SetFloat("musicVolume", volumeSet);
    }

    public void SetSFXVolume(float volumeSet)
    {
        sfxMixer.SetFloat("sfxVolume", volumeSet);
    }
    #endregion
}
