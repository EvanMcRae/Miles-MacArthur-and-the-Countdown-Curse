using System;
using UnityEngine;

public class TorchDoor : MonoBehaviour
{
    [SerializeField]
    GameObject Opened;
    [SerializeField]
    GameObject Closed;

    [SerializeField]
    RemovableObject[] Torches;

    int torchesLeftToOpen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeOpenState(false);

        torchesLeftToOpen = Torches.Length;
        foreach(RemovableObject torch in Torches)
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
        if (WasOn)
        {
            torchesLeftToOpen -= 1;
        }

        if(torchesLeftToOpen <= 0)
        {
            ChangeOpenState(true);
        }
    }

    private void OnDestroy()
    {
        foreach (RemovableObject torch in Torches)
        {
            torch.UpdateDoor -= UpdateCount;
        }
    }
}
