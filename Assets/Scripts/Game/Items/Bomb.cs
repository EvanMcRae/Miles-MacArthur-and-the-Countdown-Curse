using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Tooltip("How many beats it takes total until the bomb explodes")]
    [SerializeField]
    private int BeatsToExplode;
    //how many beats left unil this bomb explodes
    private int BeatsLeftUnilExplosion;

    [Tooltip("How many beats it takes for the bomb to disapear after it explodes")]
    [SerializeField]
    private int BeatsToCleanExplosion;
    //how many beats left unil this bomb disapears
    private int BeatsLeftUnilClean;

    private bool exploded;

    [SerializeField]
    GameObject ExplosionHirtbox;

    [Tooltip("The strength at which the bomb's explosion shakes")]
    [SerializeField]
    float shakeStrength;

    [SerializeField] private Sprite[] sprites;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private SoundPlayer soundPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.OnBeat += Onbeat;
        BeatsLeftUnilExplosion = BeatsToExplode;
        BeatsLeftUnilClean = BeatsToCleanExplosion;
        spriteRenderer = GetComponent<SpriteRenderer>();

        exploded = false;
    }

    private void Update()
    {
        if (GameManager.paused) return;

        if (exploded)
        {
            ShakeExplosion();
        }
    }

    public void Onbeat(int beatNum) 
    {
        if(BeatsLeftUnilExplosion > 0)
        {
            BeatsLeftUnilExplosion -= 1;
            spriteRenderer.sprite = sprites[BeatsLeftUnilExplosion];
        }
        else if (!exploded)
        {
            Explode();
        }

        if (exploded && BeatsLeftUnilClean > 0)
        {
            BeatsLeftUnilClean -= 1;
        }
        else if (exploded)
        {
            Destroy(gameObject);
        }
    }

    private void Explode()
    {
        soundPlayer.PlaySound("Game.BombExplosion");
        exploded = true;
        ExplosionHirtbox.SetActive(true);
    }

    private void ShakeExplosion()
    {
        ExplosionHirtbox.transform.position = transform.position + new Vector3((UnityEngine.Random.value - 0.5f) * shakeStrength, (UnityEngine.Random.value - 0.5f) * shakeStrength, 0);
    }

    private void OnDestroy()
    {
        AudioManager.OnBeat -= Onbeat;
    }
}
