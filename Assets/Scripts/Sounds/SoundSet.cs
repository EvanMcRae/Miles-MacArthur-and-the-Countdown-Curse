using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sound Set", menuName = "AudioAssets/Sound Set")]
public class SoundSet : SoundPlayable
{
    public List<AudioClip> clips;
    private int lastClip;
    public bool isMusic, isOneShot;
    public float minPitch = 1, maxPitch = 1;
    public override AudioClip GetClip()
    {
        lastClip = Random.Range(0, clips.Count);
        return clips[lastClip];
    }
    public float length()
    {
        return clips[lastClip].length;
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