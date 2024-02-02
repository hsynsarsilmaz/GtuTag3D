using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Button : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public ButtonGroup buttonGroup;
    public Image background;
    public TMPro.TextMeshProUGUI text;
    public bool isText;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        buttonGroup.onEnter(this);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        buttonGroup.onExit(this);
    }

    void Start()
    {
        background = GetComponent<Image>();
        text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        buttonGroup.subscribe(this);
    }

}
