using UnityEngine;

public class Tornado : MonoBehaviour
{
    [Tooltip("How many beats it takes for the tornado to disapear after being cast")]
    [SerializeField]
    private int BeatsToClean;
    //how many beats left unil this tornado disapears
    private int BeatsLeftUnilClean;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.OnBeat += Onbeat;

        BeatsLeftUnilClean = BeatsToClean;
    }

    public void Onbeat(int beatNum)
    {
        gameObject.GetComponent<SpriteRenderer>().flipX = !gameObject.GetComponent<SpriteRenderer>().flipX; //Flips the sprite on beat for a simple 2 frame loop.
        if (BeatsLeftUnilClean > 0)
        {
            BeatsLeftUnilClean -= 1;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        AudioManager.OnBeat -= Onbeat;
    }
}
