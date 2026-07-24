using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerCheck : MonoBehaviour
{
    public GameObject audioManager;
    [SerializeField] private MusicClip firstSong;

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
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.ChangeBGM(firstSong);
    } 
}
