using DG.Tweening;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TextFader : MonoBehaviour
{
    [SerializeField] TMP_Text _text;
    [SerializeField] float textScreenFadeDifference;

    [SerializeField] float _fadeInDuration;
    private float _fadeOutDuration;
    private float _fadeOutDelay;
    private int _numSteps;

    private void Awake()
    {
        _text.enabled = true;
    }

    private void Start()
    {
        _fadeOutDuration = ScreenTransition.instance.Duration;
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

        DOTween.To(() => fade, x => fade = x, 0, _fadeInDuration).SetUpdate(true).OnUpdate(() =>
        {
            Color color = _text.color;
            color.a = Mathf.CeilToInt(fade * _numSteps) / (float)_numSteps;
            _text.alpha = color.a;
        }).OnComplete(() =>
        {
            this.enabled = false;
        });
    }
}
