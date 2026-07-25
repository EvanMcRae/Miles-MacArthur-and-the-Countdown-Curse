using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sound Clip", menuName = "AudioAssets/Sound Clip")]
public class SoundClip : SoundPlayable
{
    public AudioClip clip;
    public bool isMusic;
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
}
