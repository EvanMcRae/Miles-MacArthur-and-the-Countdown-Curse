using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class GlowBeat : MonoBehaviour
{

    //[Tooltip("How many beats it takes for the object to disapear after being cast")]
    //[SerializeField]
    //private int BeatsToClean;
    ////how many beats left unil this bomb disapears
    //private int BeatsLeftUnilClean;

    [SerializeField]
    private Sprite[] animFrames;

    private int cursor = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.OnBeat += Onbeat;

        //BeatsLeftUnilClean = BeatsToClean;
    }

    public void Onbeat(int beatNum)
    {
        //Animate.
        gameObject.GetComponent<SpriteRenderer>().sprite = animFrames[cursor];
        cursor++;
        if (cursor >= animFrames.Length) cursor = 0;

        //if (BeatsLeftUnilClean > 0)
        //{
        //    BeatsLeftUnilClean -= 1;
        //}
        //else
        //{
        //    Destroy(this.gameObject);
        //}
    }

    private void OnDestroy()
    {
        AudioManager.OnBeat -= Onbeat;
    }
}
