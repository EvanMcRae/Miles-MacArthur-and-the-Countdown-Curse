using NUnit.Framework;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class RemovableObject : MonoBehaviour
{
    [SerializeField]
    GameObject CollisionOn;
    [SerializeField]
    GameObject CollisionOff;

    [SerializeField]
    string tagToBeRemovedBy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeCollisionState(true);
    }

    public void ChangeCollisionState(bool changeStateTo)
    {
        CollisionOn.SetActive(changeStateTo);
        CollisionOff.SetActive(!changeStateTo);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(tagToBeRemovedBy))
        {
            ChangeCollisionState(false);
        }
    }
}
