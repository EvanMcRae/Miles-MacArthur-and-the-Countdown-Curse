using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITimer : MonoBehaviour
{
    public int totalBeats = 120;
    //Divide the count to make it look like less time that passes slower
    int counterReduction = 2;
    public TextMeshProUGUI text, shadowText;
    public Image image;
    public Sprite[] sprites;

    private void Start()
    {
        AudioManager.OnBeat += SetTimer;
        text.text = "" + totalBeats / counterReduction;
        shadowText.text = text.text;
    }

    public void SetTimer(int beatNum)
    {
        text.text = "" + (totalBeats - beatNum)/counterReduction;
        shadowText.text = text.text;
        image.sprite = sprites[Mathf.FloorToInt((float)beatNum / totalBeats * sprites.Length)];

        // Lose logic
        if ((totalBeats - beatNum) / counterReduction <= 0)
        {
            GameManager.instance.Lose();
        }
    }
}
