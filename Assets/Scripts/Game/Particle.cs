using UnityEngine;

public class Particle : MonoBehaviour
{
    [SerializeField]
    private int BeatsToClean;
    //how many beats left unil this bomb disapears
    private int BeatsLeftUnilClean;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BeatsLeftUnilClean = BeatsToClean;

        AudioManager.OnBeat += Onbeat;
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
