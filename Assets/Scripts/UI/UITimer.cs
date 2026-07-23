using TMPro;
using UnityEngine;

public class UITimer : MonoBehaviour
{
    public int totalBeats = 120;
    //Divide the count to make it look like less time that passes slower
    int counterReduction = 2;
    public TextMeshProUGUI text;

    private void Start()
    {
        AudioManager.OnBeat += SetTimer;
        text.text = "" + totalBeats / counterReduction;
    }

    public void SetTimer(int beatNum)
    {
        text.text = "" + (totalBeats - beatNum)/counterReduction;
    }

    private void OnDestroy()
    {
        AudioManager.OnBeat -= SetTimer;
    }
}
