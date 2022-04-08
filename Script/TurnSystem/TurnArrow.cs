using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnArrow : MonoBehaviour
{
    public Transform target;
    private Camera cam;
    private Image image;
    private void Awake()
    {
        cam = Camera.main;
        image = GetComponent<Image>();
    }
    void Update()
    {
        transform.forward = cam.transform.forward;
        if (target != null)
        {
            transform.position = new Vector3(target.position.x, target.position.y + 1.5f, target.position.z);
            image.enabled = true;
        }
        else
            image.enabled = false;

    }
}
