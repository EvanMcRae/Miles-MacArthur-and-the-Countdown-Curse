using UnityEngine;

public class TestSoundLooper : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<SoundPlayer>().PlaySound("Game.Click", 1, true);
    }
}
