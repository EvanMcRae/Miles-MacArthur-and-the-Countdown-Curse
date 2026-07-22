using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SliderHelper : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, ISubmitHandler, IDeselectHandler, IBeginDragHandler, IEndDragHandler
{
    public bool navOn = true;
    [SerializeField] private Image image;
    [SerializeField] private Sprite inactiveSelected, inactivePressed, activeSelected, activePressed;
    private Navigation nav, newNav = new();
    [SerializeField] private TextMeshProUGUI valueText;

    public void Start()
    {
        nav = GetComponent<Selectable>().navigation;
    }

    public void ToggleNav()
    {
        if (navOn) DisableNav();
        else EnableNav();
    }

    public void DisableNav()
    {
        navOn = false;
        GetComponent<Selectable>().navigation = newNav;
        GetComponent<MenuButton>().SetSelectedSprite(activeSelected);
        GetComponent<MenuButton>().SetPressedSprite(activePressed);
    }

    public void EnableNav()
    {
        navOn = true;
        GetComponent<Selectable>().navigation = nav;
        GetComponent<MenuButton>().SetSelectedSprite(inactiveSelected);
        GetComponent<MenuButton>().SetPressedSprite(inactivePressed);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        DisableNav();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        ToggleNav();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        EnableNav();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EnableNav();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        MenuButton.canHover = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        MenuButton.canHover = true;
    }

    public void OnValueChange(float value)
    {
        valueText.text = "" + Mathf.Ceil(value);
    }
}
