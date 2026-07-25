using UnityEngine;

public class Particle : MonoBehaviour
{
    [SerializeField]
    private int BeatsToClean;
    //how many beats left unil this bomb disapears
    private int BeatsLeftUnilClean;
    [SerializeField] private SoundPlayer soundPlayer;
    [SerializeField] private SoundClip sound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BeatsLeftUnilClean = BeatsToClean;

        AudioManager.OnBeat += Onbeat;

        if (soundPlayer != null && sound != null)
        {
            soundPlayer.PlaySound(sound);
        }
    }

    public void Onbeat(int beatNum)
    {
        BeatsLeftUnilClean -= 1;

        if(BeatsLeftUnilClean <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        AudioManager.OnBeat -= Onbeat;
    }
}
