using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sound Clip", menuName = "AudioAssets/Sound Clip")]
public class SoundClip : SoundPlayable
{
    public AudioClip clip;
    public bool isMusic, isOneShot;
    public float minPitch = 1, maxPitch = 1;
    public override AudioClip GetClip()
    {
        return clip;
    }
    public float length()
    {
        return clip.length;
    }
    public override bool IsMusic()
    {
        return isMusic;
    }
    public override float GetPitch()
    {
        if (minPitch == maxPitch) return minPitch;
        return Random.Range(minPitch, maxPitch);
    }
    public override bool IsOneShot()
    {
        return isOneShot;
    }
}
