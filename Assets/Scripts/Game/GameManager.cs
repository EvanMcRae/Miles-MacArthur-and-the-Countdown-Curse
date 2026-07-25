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
    [SerializeField] private MusicClip _gameMusic;
    public static bool quitting = false;
    [SerializeField] private SoundPlayer _soundPlayer;
    [SerializeField] private string _nextScene;
    [SerializeField] private TextFader _textFader;

    void Start()
    {
        paused = false;
        quitting = false;
        instance = this;
        ScreenTransition.instance.postTransitionIn += PlayGameMusic;
        PopupPanel.panelsOpen = 0;
        PopupPanel.unpausablePanelsOpen = 0;
    }

    void PlayGameMusic()
    {
        AudioManager.instance.ChangeBGM(_gameMusic);
        ScreenTransition.instance.postTransitionIn -= PlayGameMusic;
    }

    void Update()
    {
        if (paused || ScreenTransition.active) return;

        if (!paused && PopupPanel.panelsOpen == 0)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void PressPause()
    {
        if (ScreenTransition.active || quitting) return;
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
        AudioManager.instance.Stop();
        GoToScene("MainMenu");
    }

    public void PressRetry()
    {
        if (quitting) return;
        quitting = true;
        AudioManager.instance.Stop();
        GoToScene(SceneManager.GetActiveScene().name);
    }

    public void Lose()
    {
        if (quitting) return;
        quitting = true;
        Player.instance.Die();
        _soundPlayer.PlaySound("Game.Lose");
        AudioManager.instance.Stop();
        StartCoroutine(LoseRoutine());
    }

    public void Win()
    {
        if (quitting) return;
        quitting = true;
        AudioManager.instance.Stop();
        switch (AudioManager.instance.currentArea)
        {
            case AudioManager.GameArea.SANDSCAPE:
                _soundPlayer.PlaySound("Game.Win1");
                break;
            case AudioManager.GameArea.CRYSTALSCAPE:
                _soundPlayer.PlaySound("Game.Win2");
                break;
            case AudioManager.GameArea.GARDENSCAPE:
                _soundPlayer.PlaySound("Game.Win3");
                break;
        }
        _textFader.ShowCompletedText();
        StartCoroutine(WinRoutine());
    }

    private IEnumerator LoseRoutine()
    {
        yield return new WaitForSeconds(1f);
        GoToScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator WinRoutine()
    {
        yield return new WaitForSecondsRealtime(2.5f);
        ScreenTransition.instance.TransitionOut();
        yield return new WaitForSecondsRealtime(3.5f);
        paused = false;
        SceneManager.LoadScene(_nextScene);
    }

    private void GoToScene(string scene)
    {
        ScreenTransition.instance.TransitionOut(() =>
        {
            Time.timeScale = 1;
            paused = false;
            SceneManager.LoadScene(scene);
        });
    }

    void Pause()
    {
        paused = true;
        Time.timeScale = 0;
        _pauseMenu.enabled = true;
        AudioManager.instance.PauseCurrent();
        foreach (SoundPlayer p in FindObjectsByType<SoundPlayer>())
        {
            p.PauseSound();
        }
    }

    void Unpause()
    {
        paused = false;
        Time.timeScale = 1;
        _pauseMenu.enabled = false;
        AudioManager.instance.UnPauseCurrent();
        foreach (SoundPlayer p in FindObjectsByType<SoundPlayer>())
        {
            p.UnPauseSound();
        }
    }

    void OnDestroy()
    {
        AudioManager.OnBeat = null;
    }
}
