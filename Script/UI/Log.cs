using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Log : MonoBehaviour
{
    public Text text;
    private void OnEnable()
    {
        Invoke("CloseSelf", 2f);
    }

    private void CloseSelf()
    {
        gameObject.SetActive(false);
    }

    public void SetLog(string str)
    {
        text.text = str;
    }
}
