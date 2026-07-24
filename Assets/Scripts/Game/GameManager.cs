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
    public bool quitting = false;
    [SerializeField] private SoundPlayer _soundPlayer;

    void Start()
    {
        paused = false;
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

    private IEnumerator LoseRoutine()
    {
        yield return new WaitForSeconds(1f);
        GoToScene(SceneManager.GetActiveScene().name);
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
        // TODO: pause all sounds
    }

    void Unpause()
    {
        paused = false;
        Time.timeScale = 1;
        _pauseMenu.enabled = false;
        AudioManager.instance.UnPauseCurrent();
        // TODO: resume all sounds
    }

    void OnDestroy()
    {
        AudioManager.OnBeat = null;
    }
}
