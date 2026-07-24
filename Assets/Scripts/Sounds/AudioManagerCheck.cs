using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AudioManagerCheck : MonoBehaviour
{
    public GameObject audioManager;
    [SerializeField] private MusicClip firstSong;
    [SerializeField] private float startDelay = 0.5f;
    [SerializeField] private InputActionReference _pauseAction;

    // Start is called before the first frame update
    void Awake()
    {
        if (!FindAnyObjectByType<AudioManager>())
        {
            Instantiate(audioManager, transform.position, transform.rotation);
        }
        if (firstSong != null)
        {
            StartCoroutine(StartMusic());
        }

        if (Utils.IsWebPlayer())
        {
            _pauseAction.action.ApplyBindingOverride(0, "<Keyboard>/tab");
        }
    }

    private IEnumerator StartMusic()
    {
        yield return new WaitForSeconds(startDelay);
        AudioManager.instance.ChangeBGM(firstSong);
    }
}
