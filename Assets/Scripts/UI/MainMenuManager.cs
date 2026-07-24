using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private PopupPanel _settingsPanel, _creditsPanel;
    [SerializeField] private MusicClip _menuMusic;
    [SerializeField] private Button _playButton, _creditsButton, _quitButton;

    void Start()
    {
        // ScreenTransition.instance.postTransitionIn += PlayMenuMusic;
        if (Utils.IsWebPlayer())
        {
            _quitButton.gameObject.SetActive(false);
            Utils.SetNavigation(_playButton, _creditsButton, Utils.Direction.UP);
            Utils.SetNavigation(_creditsButton, _playButton, Utils.Direction.DOWN);
        }
    }

    void PlayMenuMusic()
    {
        AudioManager.instance.ChangeBGM(_menuMusic);
        ScreenTransition.instance.postTransitionIn -= PlayMenuMusic;
    }

    public void PressPlay()
    {
        ScreenTransition.instance.TransitionOut(Play);
        AudioManager.instance.FadeOutCurrent();
    }

    void Play()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void PressSettings()
    {
        _settingsPanel.Open();
    }

    public void PressCredits()
    {
        _creditsPanel.Open();
    }

    public void PressQuit()
    {
        ScreenTransition.instance.TransitionOut(() => Invoke(nameof(Quit), 1f));
        AudioManager.instance.FadeOutCurrent();
    }

    void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
