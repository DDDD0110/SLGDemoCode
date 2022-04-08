using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLostPanel : MonoBehaviour
{
    public GameObject WinText;
    public GameObject LostText;
    public GameObject ReturnButton;

    [HideInInspector]
    public bool iswin;

    private void OnEnable()
    {
        AudioManager.Instance.BgmDisPlay();
    }
    public void OpenTextAndButton()
    {
        if (iswin)
            WinText.SetActive(true);
        else
            LostText.SetActive(true);
        ReturnButton.SetActive(true);
        WinLostAudio();
    }

    public void WinLostAudio()
    {
        if (iswin)
            AudioManager.Instance.WinPlay();
        else
            AudioManager.Instance.LostPlay();
    }
}
