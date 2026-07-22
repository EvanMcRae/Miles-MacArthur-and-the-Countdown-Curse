using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private PopupPanel _settingsPanel, _creditsPanel;
    // [SerializeField] private AK.Wwise.Event _startMusic, _fadeMusic;
    [SerializeField] private Button _playButton, _creditsButton, _quitButton;

    void Start()
    {
        ScreenTransition.instance.postTransitionIn += PlayMenuMusic;
        if (Utils.IsWebPlayer())
        {
            _quitButton.gameObject.SetActive(false);
            Utils.SetNavigation(_playButton, _creditsButton, Utils.Direction.UP);
            Utils.SetNavigation(_creditsButton, _playButton, Utils.Direction.DOWN);
        }
    }

    void PlayMenuMusic()
    {
        // _startMusic.Post(WwiseGlobal.instance);
        ScreenTransition.instance.postTransitionIn -= PlayMenuMusic;
    }

    public void PressPlay()
    {
        ScreenTransition.instance.TransitionOut(Play);
        // _fadeMusic.Post(WwiseGlobal.instance);
    }

    void Play()
    {
        SceneManager.LoadScene("GameScene");
        // _fadeMusic.Post(WwiseGlobal.instance);
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
        // _fadeMusic.Post(WwiseGlobal.instance);
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
