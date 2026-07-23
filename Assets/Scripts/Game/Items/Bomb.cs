using UnityEngine;

public class Bomb : MonoBehaviour
{
    //how many beats it takes total until the bomb explodes
    [SerializeField]
    private int BeatsToExplode;
    //how many beats left unil this bomb explodes
    private int BeatsLeftUnilExplosion;

    [SerializeField]
    GameObject ExplosionHirtbox;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.OnBeat += Onbeat;
        BeatsLeftUnilExplosion = BeatsToExplode;
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
    }

    private void Explode()
    {
        ExplosionHirtbox.SetActive(true);
    }
}
