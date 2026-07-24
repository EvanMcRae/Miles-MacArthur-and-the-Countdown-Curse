using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerMoveHandler
{
    [SerializeField] private SoundClip selectSound, pressSound;
    [SerializeField] private SoundPlayer soundPlayer;
    [SerializeField] private Sprite normalSprite, selectedSprite, pressedSprite;
    [SerializeField] private Image image;
    private bool isPressed, isSelected, isHovered;

    // TODO: TEMP
    [SerializeField] private bool usesColors = false, usesAlphaTest = true;
    [SerializeField] private Color normalColor, selectedColor, pressedColor;

    [SerializeField] private InputActionReference submit;
    public static bool canMakeSound = false, canHover = true;

    void Awake()
    {
        if (usesAlphaTest)
            image.alphaHitTestMinimumThreshold = 0.5f;
        submit.action.canceled += Unpress;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (canMakeSound)
            soundPlayer.PlaySound(selectSound);
        isSelected = true;

        if (image == null || !image.gameObject.activeSelf)
            return;
        
        // TODO: TEMP
        if (usesColors)
        {
            image.color = isPressed ? pressedColor : selectedColor;
        }
        else
        {
            image.sprite = isPressed ? pressedSprite : selectedSprite;
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;

        if (image == null || !image.gameObject.activeSelf)
            return;

        // TODO: TEMP
        if (usesColors)
        {
            image.color = normalColor;
        }
        else
        {
            image.sprite = normalSprite;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // stub
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!canHover) return;
        isHovered = true;
        if (!CursorMoveDetector.Idle)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Press();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Unpress();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        Press();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (canHover && isHovered && !isSelected)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    void Press()
    {
        if (canMakeSound)
            soundPlayer.PlaySound(pressSound);
        isPressed = true;

        if (image == null || !image.gameObject.activeSelf)
            return;
        
        // TODO: TEMP
        if (usesColors)
        {
            image.color = pressedColor;
        }
        else
        {
            image.sprite = pressedSprite;
        }
    }

    private void Unpress(InputAction.CallbackContext _) => Unpress();

    private void Unpress()
    {
        isPressed = false;

        if (image == null || !image.gameObject.activeSelf)
            return;

        // TODO: TEMP
        if (usesColors)
        {
            image.color = isSelected ? selectedColor : normalColor;
        }
        else
        {
            image.sprite = isSelected ? selectedSprite : normalSprite;
        }
    }

    public void SetSelectedSprite(Sprite sprite)
    {
        selectedSprite = sprite;
        RefreshSelectedSprite();
    }

    public void SetSelectedColor(Color color)
    {
        selectedColor = color;
        RefreshSelectedSprite();
    }

    public void SetPressedSprite(Sprite sprite)
    {
        pressedSprite = sprite;
        RefreshSelectedSprite();
    }

    public void SetPressedColor(Color color)
    {
        pressedColor = color;
        RefreshSelectedSprite();
    }

    void RefreshSelectedSprite()
    {
        if (!isSelected) return;

        // TODO: TEMP
        if (usesColors)
        {
            image.color = isPressed ? pressedColor : selectedColor;
        }
        else
        {
            image.sprite = isPressed ? pressedSprite : selectedSprite;
        }
    }
}
