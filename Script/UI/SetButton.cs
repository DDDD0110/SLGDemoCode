using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SetButton : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    private Image button;

    private void Awake()
    {
        button = GetComponent<Image>();
    }
    private void OnEnable()
    {
        SetColor(0.5f);
    }
    private void SetColor(float a)
    {
        Color color_b = new Color(1f, 1f, 1f, a);
        button.color = color_b;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetColor(1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetColor(0.5f);
    }
}
