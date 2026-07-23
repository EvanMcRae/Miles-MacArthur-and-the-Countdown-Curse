using DG.Tweening;
using UnityEngine;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.OnBeat += Onbeat;

        BeatsLeftUnilClean = BeatsToClean;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float Actualspeed = MoveSpeed / 60;

        transform.Translate(new Vector3(MoveSpeed / 60 * xDirection, MoveSpeed/60 * yDirection, 0), Space.World);
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

    private void OnDestroy()
    {
        AudioManager.OnBeat -= Onbeat;
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
}
