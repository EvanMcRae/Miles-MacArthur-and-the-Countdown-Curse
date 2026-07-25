using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.EventSystems;
using TMPro;

public class ScreenTransition : MonoBehaviour
{
    private Image _image;
    public static ScreenTransition instance;
    [SerializeField] private float _duration = 1f;
    [SerializeField] private float _delay = 0.5f;
    [SerializeField] private int _numSteps = 4;
    public float Duration => _duration;
    public float Delay => _delay;
    public int NumSteps => _numSteps;


    public Action postTransitionIn;
    public static bool active;

    void Awake()
    {
        MenuButton.canMakeSound = false;
        active = true;
        _image = GetComponent<Image>();
        _image.enabled = true;
        instance = this;
        Invoke(nameof(StartTransition), _delay);
    }
    
    public void TransitionOut(Action callback = null)
    {
        // CursorManager.SetCursorState(CursorManager.CursorState.LOADING);
        active = true;
        EventSystem.current.sendNavigationEvents = false;
        _image.enabled = true;

        float fade = _image.color.a;
        DOTween.To(() => fade, x => fade = x, 1, _duration).SetUpdate(true).OnUpdate(() =>
        {
            Color c = _image.color;
            c.a = Mathf.CeilToInt(fade * _numSteps) / (float)_numSteps;
            _image.color = c;
        }).OnComplete(() =>
        {
            callback?.Invoke();
            active = false;
        });
    }

    public void TransitionIn(Action callback = null)
    {
        // CursorManager.SetCursorState(CursorManager.CursorState.LOADING);
        active = true;
        if (callback != null)
        {
            postTransitionIn = callback;
        }
        _image.enabled = true;

        // TODO: fancier animation
        float fade = _image.color.a;
        DOTween.To(() => fade, x => fade = x, 0, _duration).SetUpdate(true).OnUpdate(() =>
        {
            Color c = _image.color;
            c.a = Mathf.CeilToInt(fade * _numSteps) / (float)_numSteps;
            _image.color = c;
        }).OnComplete(() => {
            postTransitionIn?.Invoke();
            _image.enabled = false;
            active = false;
            EventSystem.current.sendNavigationEvents = true;
            // CursorManager.SetCursorState(CursorManager.CursorState.NORMAL);
        });
    }

    void StartTransition()
    {
        MenuButton.canMakeSound = true;
        EventSystem.current.sendNavigationEvents = false;
        TransitionIn();
    }
}

