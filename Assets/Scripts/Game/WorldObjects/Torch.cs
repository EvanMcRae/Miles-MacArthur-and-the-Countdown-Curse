using NUnit.Framework;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Torch : MonoBehaviour
{
    [SerializeField]
    GameObject TorchOn;
    [SerializeField]
    GameObject TorchOff;

    [SerializeField]
    string tagToBeLitBy;

    [SerializeField] private bool TorchLit;

    [SerializeField] private SoundPlayer soundPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeCollisionState(TorchLit);
    }

    public void ChangeCollisionState(bool changeStateTo)
    {
        TorchLit = changeStateTo;
        TorchOn.SetActive(changeStateTo);
        TorchOff.SetActive(!changeStateTo);
    }

    public void LightTorch()
    {
        soundPlayer.PlaySound("Game.LightTorch");
        UpdateDoor?.Invoke(TorchLit);
        ChangeCollisionState(true);
    }

    public Action<bool> UpdateDoor;
}