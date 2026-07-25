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

    bool TorchLit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TorchLit = false;
        ChangeCollisionState(TorchLit);
    }

    public void ChangeCollisionState(bool changeStateTo)
    {
        TorchLit = changeStateTo;
        TorchOn.SetActive(changeStateTo);
        TorchOff.SetActive(!changeStateTo);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag(tagToBeLitBy))
    //    {
    //        UpdateDoor?.Invoke(TorchLit);
    //        ChangeCollisionState(false);
    //    }
    //}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag(tagToBeLitBy))
    //    {
    //        UpdateDoor?.Invoke(TorchLit);
    //        ChangeCollisionState(false);
    //    }
    //}

    public void LightTorch()
    {
        UpdateDoor?.Invoke(TorchLit);
        ChangeCollisionState(true);
    }

    public Action<bool> UpdateDoor;
}