using TMPro;
using UnityEngine;

public class UITimer : MonoBehaviour
{
    public int totalBeats = 120;
    //Divide the count to make it look like less time that passes slower
    int counterReduction = 2;
    public TextMeshProUGUI text;
    [SerializeField] private SoundPlayer soundPlayer;

    private void Start()
    {
        AudioManager.OnBeat += SetTimer;
        text.text = "" + totalBeats / counterReduction;
    }

    public void SetTimer(int beatNum)
    {
        text.text = "" + (totalBeats - beatNum)/counterReduction;

        // Lose logic
        if ((totalBeats - beatNum) / counterReduction <= 0) 
        {
            Player.instance.Die();
            soundPlayer.PlaySound("Game.Lose");
            GameManager.instance.PressRetry();
        }
    }
}
