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

    [SerializeField] private SoundPlayer soundPlayer;

    [SerializeField] private SoundClip sound;

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
            soundPlayer.PlaySound(sound);
            ChangeCollisionState(false);
        }
    }
}
