using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public GameObject AudioPanel;

    [Header("AudioClip")]
    public AudioClip Bgm;
    public AudioClip ButtonClip;
    public AudioClip ButtonClip02;
    public AudioClip ButtonClip03;
    public AudioClip WinClip;
    public AudioClip LostClip;

    private AudioSource BgmSource;
    private AudioSource ButtonSource;
    private AudioSource WinLostSource;


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        BgmSource = gameObject.AddComponent<AudioSource>();
        ButtonSource = gameObject.AddComponent<AudioSource>();
        WinLostSource = gameObject.AddComponent<AudioSource>();
    }
    public void BgmPlay()
    {
        BgmSource.clip = Bgm;
        BgmSource.Play();
        BgmSource.loop = true;
    }
    public void BgmDisPlay()
    {
        BgmSource.Stop();
    }
    public void ButtonPlay()
    {
        ButtonSource.clip = ButtonClip;
        ButtonSource.Play();
    }
    public void Button02Play()
    {
        ButtonSource.clip = ButtonClip02;
        ButtonSource.Play();
    }
    public void Button03Play()
    {
        ButtonSource.clip = ButtonClip03;
        ButtonSource.Play();
    }
    public void WinPlay()
    {
        WinLostSource.clip = WinClip;
        WinLostSource.Play();
    }
    public void LostPlay()
    {
        WinLostSource.clip = LostClip;
        WinLostSource.Play();
    }
    public void OpenAudioPanel()
    {
        AudioPanel.SetActive(true);
    }
    public void CloseAudioPanel()
    {
        AudioPanel.SetActive(false);
    }
    public void ChangeAudioVolume(float value)
    {
        BgmSource.volume = value;
    }


}
