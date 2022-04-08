using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Camera cam;
    private Text text;
    public Transform target;

    private void Awake()
    {
        cam = Camera.main;
        text=GetComponent<Text>();
    }

    private void Update()
    {
        if (target != null)
        {
            transform.position = target.position;
            transform.forward = cam.transform.forward;
        }
    }
    public void SetHealthBar(int current, int max)
    {
        text.text = current.ToString() + "/" + max.ToString();
    }
}
