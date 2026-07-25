using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Fireball : MonoBehaviour
{
    [Tooltip("How fast the fireball moves in tiles per second")]
    [SerializeField]
    float MoveSpeed;

    public int xDirection = 0;
    public int yDirection = 1;

    [Tooltip("How many beats it takes for the fireball to disapear after being cast")]
    [SerializeField]
    private int BeatsToClean;
    //how many beats left unil this bomb disapears
    private int BeatsLeftUnilClean;

    [SerializeField]
    private Sprite[] animFrames;

    private int cursor = 0;

    private Rigidbody2D rb;

    [SerializeField]
    GameObject Particle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.OnBeat += Onbeat;
        AudioManager.OnHalfBeat += OnHalfBeat;

        BeatsLeftUnilClean = BeatsToClean;

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(MoveSpeed * xDirection, MoveSpeed * yDirection, 0);
    }

    public void Onbeat(int beatNum)
    {
        if (BeatsLeftUnilClean > 0)
        {
            BeatsLeftUnilClean -= 1;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void OnHalfBeat(int beatNum)
    {
        //Animate the fireball.
        gameObject.GetComponent<SpriteRenderer>().sprite = animFrames[cursor];
        cursor++;
        if (cursor >= animFrames.Length) cursor = 0;
    }

    private void OnDestroy()
    {
        AudioManager.OnBeat -= Onbeat;
        AudioManager.OnHalfBeat -= OnHalfBeat;
    }

    public void SetVisualRotation()
    {
        //if facing up
        if(yDirection == 1)
        {
            transform.Rotate(new Vector3(0, 0, 0));
        }
        //if facuing down
        else if (yDirection == -1)
        {
            transform.Rotate(new Vector3(0, 0, 180));
        }
        //if facing left
        else if (xDirection == 1)
        {
            transform.Rotate(new Vector3(0, 0, 270));
        }
        //if facing right
        else if(xDirection == -1)
        {
            transform.Rotate(new Vector3(0, 0, 90));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider is TilemapCollider2D)
        {
            Instantiate(Particle, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Torch"))
        {
            collision.gameObject.GetComponent<Torch>().LightTorch();
        }
    }
}
