using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static bool paused = false;
    public static GameManager instance;
    [SerializeField] private Overlay _pauseMenu;
    [SerializeField] private PopupPanel _settingsPanel;
    // [SerializeField] private AK.Wwise.Event _quit, _pause, _resume, _loseGame;
    private bool quitting = false;

    void Start()
    {
        paused = false;
        instance = this;
        ScreenTransition.instance.postTransitionIn += () =>
        {
            // do anything we want here
        };
        PopupPanel.panelsOpen = 0;
        PopupPanel.unpausablePanelsOpen = 0;
    }

    void Update()
    {
        if (paused || ScreenTransition.active) return;

        if (!paused && PopupPanel.panelsOpen == 0)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void PressPause()
    {
        if (ScreenTransition.active) return;
        if (!paused)
        {
            Pause();
        }
        else
        {
            Unpause();
        }
    }

    public void PressSettings()
    {
        _settingsPanel.Open();
    }

    public void PressQuit()
    {
        if (quitting) return;
        quitting = true;
        // _quit.Post(WwiseGlobal.instance);
        ScreenTransition.instance.TransitionOut(() =>
        {
            Time.timeScale = 1;
            paused = false;
            SceneManager.LoadScene("MainMenu");
        });
    }

    public void PressRetry()
    {
        if (quitting) return;
        quitting = true;
        // _quit.Post(WwiseGlobal.instance);
        ScreenTransition.instance.TransitionOut(() =>
        {
            Time.timeScale = 1;
            paused = false;
            SceneManager.LoadScene("GameScene");
        });
    }

    void Pause()
    {
        paused = true;
        // _pause.Post(WwiseGlobal.instance);
        Time.timeScale = 0;
        _pauseMenu.enabled = true;
    }

    void Unpause()
    {
        paused = false;
        // _resume.Post(WwiseGlobal.instance);
        Time.timeScale = 1;
        _pauseMenu.enabled = false;
    }
}
