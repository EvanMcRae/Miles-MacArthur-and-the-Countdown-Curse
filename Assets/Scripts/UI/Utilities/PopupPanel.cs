using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using System;
using System.Collections;

public class PopupPanel : Overlay
{
    [SerializeField] private Image _backingImage;
    [SerializeField] private GameObject _popupContents;
    private Tween _backingImageTween, _popupContentsTween;
    [SerializeField] private InputActionReference _exit;
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private float _backingImageOpacity = 0.5f;
    public static int unpausablePanelsOpen = 0, panelsOpen = 0;
    private bool _isOpen = false, _isClosing = false;
    [SerializeField] private bool _canCloseWithEsc = true;
    [SerializeField] private bool _overridesPause = true;
    [SerializeField] private bool _goesUp = true;
    private Vector3 startPos;
    public Action onClose;
    [SerializeField] private SoundClip closeSound;
    [SerializeField] private SoundPlayer soundPlayer;

    override protected void Awake()
    {
        base.Awake();
        if (_canCloseWithEsc)
            _exit.action.performed += Close;

        // Set start pos
        startPos = _popupContents.GetComponent<RectTransform>().anchoredPosition;
        startPos.y = (_goesUp ? -1 : 1) * 2000;
        _popupContents.GetComponent<RectTransform>().anchoredPosition = startPos;

        panelsOpen = 0;
    }

    override protected void OnEnable()
    {
        
    }

    override protected void OnDisable()
    {

    }

    public void Open()
    {
        if (_overridesPause)
            unpausablePanelsOpen++;
        panelsOpen++;
        _isOpen = true;
        _isClosing = false;

        GetComponent<GraphicRaycaster>().enabled = true;

        MenuButton.canMakeSound = false;
        SetSelection();
        MenuButton.canMakeSound = true;

        Utils.KillTween(ref _backingImageTween);
        _backingImageTween = _backingImage.DOFade(_backingImageOpacity, _duration).SetUpdate(true);

        Utils.KillTween(ref _popupContentsTween);
        _popupContentsTween = _popupContents.GetComponent<RectTransform>().DOAnchorPosY(0, _duration).SetUpdate(true).OnComplete(() =>
        {
            MenuButton.canMakeSound = true;
        });
    }

    public void Close(InputAction.CallbackContext _)
    {
        if (!_isOpen || _isClosing) return;
        soundPlayer.PlaySound(closeSound);
        Close();
    }

    public void Close()
    {
        if (!_isOpen || _isClosing) return;
        _isClosing = true;

        onClose?.Invoke();

        // Cancels active dragging -- thanks https://discussions.unity.com/t/disable-current-drag-of-ui-slider/156580/2
        var inputModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();
        inputModule.enabled = false;
        inputModule.enabled = true;

        MenuButton.canMakeSound = false;
        RestoreSelection();
        MenuButton.canMakeSound = true;

        StartCoroutine(UponClosePopup());

        Utils.KillTween(ref _backingImageTween);
        _backingImageTween = _backingImage.DOFade(0, _duration).SetUpdate(true);

        Utils.KillTween(ref _popupContentsTween);
        _popupContentsTween = _popupContents.GetComponent<RectTransform>().DOAnchorPosY((_goesUp ? -1 : 1) * 2000, _duration).SetUpdate(true).OnComplete(() => {
            GetComponent<GraphicRaycaster>().enabled = false;
            _isOpen = false;
            _isClosing = false;
            MenuButton.canHover = true; // Also needed to fix active dragging
            MenuButton.canMakeSound = true;
        });
    }

    override protected void Update()
    {
        base.Update();
    }

    IEnumerator UponClosePopup()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (_overridesPause)
            unpausablePanelsOpen--;
        panelsOpen--;
        if (panelsOpen < 0) panelsOpen = 0;
        if (unpausablePanelsOpen < 0) unpausablePanelsOpen = 0;
    }
}