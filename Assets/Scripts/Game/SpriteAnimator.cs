using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriterenderer;

    [SerializeField]
    Sprite[] Sprites;

    /// <summary>
    /// Number of beats until moving on to the next sprite
    /// </summary>
    [SerializeField]
    int BeatsToChangeSprite;

    int BeatsUnilNextChange;

    int currentSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSprite = 0;
        BeatsUnilNextChange = BeatsToChangeSprite;
        AudioManager.OnBeat += ChangeSprite;
    }

    private void ChangeSprite(int beatNum)
    {
        BeatsUnilNextChange -= 1;

        if(Sprites.Length > 0 && BeatsUnilNextChange <= 0)
        {
            currentSprite += 1;
            if(currentSprite >= Sprites.Length)
            {
                currentSprite = 0;
            }

            spriterenderer.sprite = Sprites[currentSprite];

            BeatsUnilNextChange = BeatsToChangeSprite;
        }
    }

}
