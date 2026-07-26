using System;
using UnityEngine;

public class TorchDoor : MonoBehaviour
{
    [SerializeField]
    GameObject Opened;
    [SerializeField]
    GameObject Closed;

    [SerializeField]
    Torch[] Torches;

    int torchesLeftToTurnOn;
    private bool playedSound = false;
    [SerializeField] private SoundPlayer soundPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeOpenState(false);

        torchesLeftToTurnOn = Torches.Length;
        foreach(Torch torch in Torches)
        {
            torch.UpdateDoor += UpdateCount;
        }
    }

    public void ChangeOpenState(bool changeStateTo)
    {
        Opened.SetActive(changeStateTo);
        Closed.SetActive(!changeStateTo);
    }

    public void UpdateCount(bool WasOn)
    {
        if (!WasOn)
        {
            torchesLeftToTurnOn -= 1;
        }

        if(torchesLeftToTurnOn <= 0)
        {
            if (!playedSound)
            {
                soundPlayer.PlaySound("Game.UnlockDoor");
                playedSound = true;
            }
            ChangeOpenState(true);
        }
    }

    private void OnDestroy()
    {
        foreach (Torch torch in Torches)
        {
            torch.UpdateDoor -= UpdateCount;
        }
    }
}
