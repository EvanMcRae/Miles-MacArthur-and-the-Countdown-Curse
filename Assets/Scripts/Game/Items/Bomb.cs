using UnityEngine;

public class Bomb : MonoBehaviour
{
    //how many beats it takes total until the bomb explodes
    [SerializeField]
    private int BeatsToExplode;
    //how many beats left unil this bomb explodes
    private int BeatsLeftUnilExplosion;

    //how many beats it takes for the bomb to disapear after it explodes
    [SerializeField]
    private int BeatsToCleanExplosion;
    //how many beats left unil this bomb disapears
    private int BeatsLeftUnilClean;

    private bool exploded;

    [SerializeField]
    GameObject ExplosionHirtbox;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.OnBeat += Onbeat;
        BeatsLeftUnilExplosion = BeatsToExplode;
        BeatsLeftUnilClean = BeatsToCleanExplosion;

        exploded = false;
    }

    public void Onbeat(int beatNum) 
    {
        if(BeatsLeftUnilExplosion > 0)
        {
            BeatsLeftUnilExplosion -= 1;
        }
        else
        {
            Explode();
        }

        if (exploded && BeatsLeftUnilClean > 0)
        {
            BeatsLeftUnilClean -= 1;
        }
        else if (exploded)
        {
            Destroy(this.gameObject);
        }
    }

    private void Explode()
    {
        exploded = true;
        ExplosionHirtbox.SetActive(true);
    }

    private void OnDestroy()
    {
        AudioManager.OnBeat -= Onbeat;
    }
}
