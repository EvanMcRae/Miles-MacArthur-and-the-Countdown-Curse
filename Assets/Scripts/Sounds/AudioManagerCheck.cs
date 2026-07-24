using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerCheck : MonoBehaviour
{
    public GameObject audioManager;
    [SerializeField] private MusicClip firstSong;
    [SerializeField] private float startDelay = 0.5f;

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
    }

    private IEnumerator StartMusic()
    {
        yield return new WaitForSeconds(startDelay);
        AudioManager.instance.ChangeBGM(firstSong);
    } 
}
