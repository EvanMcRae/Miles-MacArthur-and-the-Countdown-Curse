using DG.Tweening;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TextFader : MonoBehaviour
{
    [SerializeField] TMP_Text _text;
    [SerializeField] float textScreenFadeDifference;

    [SerializeField] private float _fadeInDuration;
    [SerializeField] private float _fadeOutDuration;
    private int _numSteps;

    private float _fadeOutDelay;

    private void Awake()
    {
        _text.enabled = true;
    }

    private void Start()
    {
        // fade text before screen
        _fadeOutDelay = Mathf.Clamp(
            ScreenTransition.instance.Delay - textScreenFadeDifference - _fadeInDuration, 
            0.0f, 
            ScreenTransition.instance.Delay
            );
        _numSteps = ScreenTransition.instance.NumSteps;

        FadeIn();
    }

    private void FadeIn()
    {
        Color c = _text.color;
        c.a = 1;
        _text.color = c;
        float fade = 0;

        DOTween.To(() => fade, x => fade = x, 1, _fadeInDuration).SetUpdate(true).OnUpdate(() =>
        {
            Color color = _text.color;
            color.a = Mathf.CeilToInt(fade * _numSteps) / (float)_numSteps;
            _text.alpha = color.a;
        });
        Invoke(nameof(FadeOut), _fadeOutDelay);
    }

    private void FadeOut()
    {
        float fade = _text.color.a;

        DOTween.To(() => fade, x => fade = x, 0, _fadeOutDuration).SetUpdate(true).OnUpdate(() =>
        {
            Color color = _text.color;
            color.a = Mathf.CeilToInt(fade * _numSteps) / (float)_numSteps;
            _text.alpha = color.a;
        }).OnComplete(() =>
        {
            enabled = false;
        });
    }

    public void ShowCompletedText()
    {
        enabled = true;
        FadeIn();
        _fadeOutDelay = 5f;
        _text.text += " CLEARED";
    }
}
