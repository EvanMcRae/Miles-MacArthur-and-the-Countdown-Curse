using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioMixer musicMixer, sfxMixer;
    public AudioMixerGroup musicMixerGroup, sfxMixerGroup;
    public MusicClip currentSong = null;
    public GameArea currentArea;

    private int activePlayer = 0;
    public AudioSource[] BGM1, BGM2;
    private readonly Tween[] outFader = new Tween[2];
    private readonly Tween[] inFader = new Tween[2];
    public float musicVolume = 1.0f, sfxVolume = 10.0f, masterVolume = 10f, targetSFXVolume= -80.0f, actualSFXVolume = -80.0f;
    public float fadeDuration = 1.0f;
    private int loopPointSamples, preEntryPointSamples;
    private bool firstSet = true;
    private bool firstSongPlayed = false;
    public bool paused = false;
    public bool carryOn = true;
    public bool inCutscene = false;

    public SoundCategory soundDatabase;
    public MusicCategory musicDatabase;

    private int beatLength, lastTime, absoluteTime, currentBeat, absoluteHalfTime, currentHalfBeat;

    public static Action<int> OnBeat, OnHalfBeat;

    private float lowPass = 22000.00f;

    private AudioSource permaSource;

    /// <summary>
    /// List of all different game areas that may have different sets of music
    /// </summary>
    public enum GameArea
    {
        CURRENT, MENU, SANDSCAPE, CRYSTALSCAPE, GARDENSCAPE
    }

    /// <summary>
    /// Set up the AudioSources
    /// </summary>
    private void Awake()
    {
        AudioSettings.Reset(AudioSettings.GetConfiguration());

        if (FindObjectsByType<AudioManager>().Length > 1)
        {
            instance = null;
            Destroy(gameObject);
            return;
        }

        // Generate two AudioSource lists
        BGM1 = new AudioSource[2]{
            gameObject.AddComponent<AudioSource>(),
            gameObject.AddComponent<AudioSource>()
        };

        BGM2 = new AudioSource[2]{
            gameObject.AddComponent<AudioSource>(),
            gameObject.AddComponent<AudioSource>()
        };

        // Set default values
        foreach (AudioSource s in BGM1)
        {
            s.loop = false;
            s.playOnAwake = false;
            s.volume = 0.0f;
            s.outputAudioMixerGroup = musicMixerGroup;
            s.pitch = 1f;
            s.dopplerLevel = 0;
            s.spatialBlend = 0;
            s.reverbZoneMix = 0;
        }

        foreach (AudioSource s in BGM2)
        {
            s.loop = false;
            s.playOnAwake = false;
            s.volume = 0.0f;
            s.outputAudioMixerGroup = musicMixerGroup;
            s.pitch = 1f;
            s.dopplerLevel = 0;
            s.spatialBlend = 0;
            s.reverbZoneMix = 0;
        }

        // Hehehaha fun
        permaSource = gameObject.AddComponent<AudioSource>();
        permaSource.loop = false;
        permaSource.playOnAwake = false;
        permaSource.volume = 1.0f;
        permaSource.outputAudioMixerGroup = sfxMixerGroup;
        permaSource.pitch = 1f;
        permaSource.dopplerLevel = 0;
        permaSource.spatialBlend = 0;
        permaSource.reverbZoneMix = 0;

        // Singleton pattern
        instance = this;
        DontDestroyOnLoad(gameObject);

        musicVolume = Mathf.Log10(PlayerPrefs.GetFloat("musicVolume") / 100f + 0.00001f) * 20; 
        sfxVolume = Mathf.Log10(PlayerPrefs.GetFloat("soundVolume") / 100f + 0.00001f) * 20;
        masterVolume = Mathf.Log10(PlayerPrefs.GetFloat("masterVolume") / 100f + 0.00001f) * 20;

        sfxMixer.SetFloat("Volume", sfxVolume + masterVolume);
        musicMixer.SetFloat("Volume", musicVolume + masterVolume);
    }

    // Update is called once per frame
    void Update()
    {
        if (instance == null)
        {
            instance = this;
        }

        // Sync low pass value
        musicMixer.SetFloat("LowPass", lowPass);

        // Check for desyncing during crossfades
        if (carryOn && outFader[0] != null && inFader[0] != null)
        {
            if (firstSet)
            {
                if (BGM2[activePlayer].timeSamples != BGM1[activePlayer].timeSamples && BGM2[activePlayer].isPlaying && BGM1[activePlayer].isPlaying)
                    BGM1[activePlayer].timeSamples = BGM2[activePlayer].timeSamples;

                if (BGM2[1-activePlayer].timeSamples != BGM1[1-activePlayer].timeSamples && BGM2[1-activePlayer].isPlaying && BGM1[1-activePlayer].isPlaying)
                    BGM1[1-activePlayer].timeSamples = BGM2[1-activePlayer].timeSamples;
            }
            else
            {
                if (BGM1[activePlayer].timeSamples != BGM2[activePlayer].timeSamples && BGM1[activePlayer].isPlaying && BGM2[activePlayer].isPlaying)
                    BGM2[activePlayer].timeSamples = BGM1[activePlayer].timeSamples;

                if (BGM1[1-activePlayer].timeSamples != BGM2[1-activePlayer].timeSamples && BGM1[1-activePlayer].isPlaying && BGM2[1-activePlayer].isPlaying)
                    BGM2[1-activePlayer].timeSamples = BGM1[1-activePlayer].timeSamples;
            }
        }

        // Manages looping tracks
        if (firstSet)
        {
            if (BGM1[activePlayer].clip != null && BGM1[activePlayer].time >= loopPointSamples)
            {
                activePlayer = 1 - activePlayer;
                lastTime = 0;
                if (currentSong != null)
                    BGM1[activePlayer].clip = currentSong.GetClip();
                BGM1[activePlayer].Play();
                BGM1[activePlayer].time = preEntryPointSamples;
            }
        }
        else
        {
            if (BGM2[activePlayer].clip != null && BGM2[activePlayer].timeSamples >= loopPointSamples)
            {
                activePlayer = 1 - activePlayer;
                lastTime = 0;
                if (currentSong != null)
                    BGM2[activePlayer].clip = currentSong.GetClip();
                BGM2[activePlayer].Play();
                BGM2[activePlayer].time = preEntryPointSamples;
            }
        }

        musicVolume = Mathf.Log10(PlayerPrefs.GetFloat("musicVolume") / 100f + 0.00001f) * 20;
        if (GameManager.paused) musicVolume -= 2f;
        sfxVolume = Mathf.Log10(PlayerPrefs.GetFloat("soundVolume") / 100f + 0.00001f) * 20;
        masterVolume = Mathf.Log10(PlayerPrefs.GetFloat("masterVolume") / 100f + 0.00001f) * 20;

        sfxMixer.SetFloat("Volume", sfxVolume + masterVolume);
        musicMixer.SetFloat("Volume", musicVolume + masterVolume);

        if (GameManager.paused || GameManager.quitting) return;

        // Beat tracking
        AudioSource currentPlayer = firstSet ? BGM1[activePlayer] : BGM2[activePlayer];
        if (currentSong != null && currentPlayer.clip != null && currentArea != GameArea.MENU)
        {
            int currentTime = currentPlayer.timeSamples;
            int delta = currentTime - lastTime;
            if (currentTime < lastTime)
            {
                delta += currentSong.GetClip().samples;
                currentBeat = 0;
                currentHalfBeat = 0;
            }
            absoluteTime += delta;
            absoluteHalfTime += delta;

            lastTime = currentTime;

            if ((absoluteTime / beatLength != 0) || currentBeat == 0)
            {
                if (currentBeat > 0)
                    absoluteTime -= beatLength;
                currentBeat++;
                OnBeat?.Invoke(currentBeat);
            }

            if ((absoluteHalfTime / (beatLength/2) != 0) || currentHalfBeat == 0)
            {
                if (currentHalfBeat > 0)
                    absoluteHalfTime -= beatLength/2;
                currentHalfBeat++;
                OnHalfBeat?.Invoke(currentHalfBeat);
            }
        }
    }

    public void ChangeBGM(string musicPath, float duration = 1f)
    {
        MusicClip music = FindMusic(musicPath);
        ChangeBGM(music, duration);
    }

    public void ChangeBGM(string musicPath, string area, float duration = 1f)
    {
        GameArea theArea;
        switch (area.Trim().ToUpper())
        {
            case "CURRENT":
                theArea = currentArea;
                break;
            case "MENU":
                theArea = GameArea.MENU;
                break;
            case "SANDSCAPE":
                theArea = GameArea.SANDSCAPE;
                break;
            case "CRYSTALSCAPE":
                theArea = GameArea.CRYSTALSCAPE;
                break;
            case "GARDENSCAPE":
                theArea = GameArea.GARDENSCAPE;
                break;
            default:
                Debug.LogWarning("Invalid area provided! Using current");
                theArea = currentArea;
                break;
        }
        ChangeBGM(FindMusic(musicPath), theArea, duration);
    }

    public void ChangeBGM(string musicPath, GameArea area, float duration = 1f)
    {
        ChangeBGM(FindMusic(musicPath), area, duration);
    }

    public void ChangeBGM(MusicClip music, float duration = 1f)
    {
        ChangeBGM(music, music.area, duration);
    }

    public void ChangeBGM(MusicClip music, GameArea newArea, float duration = 1f)
    {
        // Support cutscenes keeping music area
        if (newArea == GameArea.CURRENT) newArea = currentArea;

        // Carry on music if area has not changed
        carryOn = newArea == currentArea;
        currentArea = newArea;

        // Calculate loop point
        float loopPointSeconds = 60.0f * ((music.barsLength + music.preEntryBars) * 4.0f * music.timeSignature / music.timeSignatureBottom) / music.BPM;
        float preEntryPointSeconds = 60.0f * (music.preEntryBars * 4.0f * music.timeSignature / music.timeSignatureBottom) / music.BPM;
        if (loopPointSeconds > music.length())
        {
            Debug.LogWarning($"{music} is too short to loop! True length = {music.length()} seconds, loop point = {loopPointSeconds} seconds. Using true length.");
            loopPointSeconds = music.length();
        }
        loopPointSamples = (int)loopPointSeconds * music.clip.frequency;
        preEntryPointSamples = (int)preEntryPointSeconds * music.clip.frequency;

        // Prevent fading the same clip on both players
        if (music == currentSong)
            return;

        if (currentSong == null)
        {
            duration = 0f;
            absoluteTime = 0;
            lastTime = 0;
            currentBeat = 0;
        }
        beatLength = (int)(60.0f / music.BPM * music.clip.frequency * music.beatFrequency * music.timeSignature / music.timeSignatureBottom);

        // Kill all playing
        for (int i = 0; i < outFader.Length; i++)
        {
            if (outFader[i] != null)
            {
                Utils.KillTween(ref outFader[i]);
                outFader[i] = null;
            }
        }

        if (firstSet)
        {
            // Fade-out the active play, if it is not silent (eg: first start)
            if (BGM1[activePlayer].volume > 0)
            {
                if (duration > 0)
                {
                    Utils.KillTween(ref outFader[0]);
                    Utils.KillTween(ref outFader[1]);
                    outFader[0] = BGM1[activePlayer].DOFade(0, duration).SetUpdate(true).OnComplete(() => outFader[0] = null);
                    outFader[1] = BGM1[1-activePlayer].DOFade(0, duration).SetUpdate(true).OnComplete(() => outFader[1] = null);
                }
                else
                {
                    BGM1[activePlayer].volume = 0;
                    BGM1[1-activePlayer].volume = 0;
                }
            }

            // Fade-in the new clip
            BGM2[activePlayer].clip = music.GetClip();
            BGM2[activePlayer].Play();
            if (carryOn && BGM1[activePlayer].isPlaying && BGM1[activePlayer].clip != null)
            {
                BGM2[activePlayer].timeSamples = BGM1[activePlayer].timeSamples; // syncs up time
            }
            else
            {
                BGM2[activePlayer].timeSamples = 0;
                absoluteTime = 0;
                lastTime = 0;
                currentBeat = 0;
            }

            if (firstSongPlayed && duration > 0)
            {
                Utils.KillTween(ref inFader[0]);
                Utils.KillTween(ref inFader[1]);
                BGM2[activePlayer].volume = 0;
                BGM2[1-activePlayer].volume = 0;
                inFader[0] = BGM2[activePlayer].DOFade(1, duration).SetUpdate(true).OnComplete(() => inFader[0] = null);
                inFader[1] = BGM2[1-activePlayer].DOFade(1, duration).SetUpdate(true).OnComplete(() => inFader[1] = null);
            }
            else
            {
                BGM2[activePlayer].volume = 1;
                BGM2[1-activePlayer].volume = 1;
            }
        }
        else
        {
            // Fade-out the active play, if it is not silent (eg: first start)
            if (BGM2[activePlayer].volume > 0)
            {
                if (duration > 0)
                {
                    Utils.KillTween(ref outFader[0]);
                    Utils.KillTween(ref outFader[1]);
                    outFader[0] = BGM2[activePlayer].DOFade(0, duration).SetUpdate(true).OnComplete(() => outFader[0] = null);
                    outFader[1] = BGM2[1-activePlayer].DOFade(0, duration).SetUpdate(true).OnComplete(() => outFader[1] = null);
                }
                else
                {
                    BGM2[activePlayer].volume = 0;
                    BGM2[1-activePlayer].volume = 0;
                }
            }

            // Fade-in the new clip
            BGM1[activePlayer].clip = music.GetClip();
            BGM1[activePlayer].Play();
            if (carryOn && BGM2[activePlayer].isPlaying && BGM2[activePlayer].clip != null)
            {
                BGM1[activePlayer].timeSamples = BGM2[activePlayer].timeSamples; // Syncs up time
            }
            else
            {
                BGM1[activePlayer].timeSamples = 0;
                absoluteTime = 0;
                lastTime = 0;
                currentBeat = 0;
            }
            
            if (firstSongPlayed && duration > 0)
            {
                Utils.KillTween(ref inFader[0]);
                Utils.KillTween(ref inFader[1]);
                BGM1[activePlayer].volume = 0;
                BGM1[1-activePlayer].volume = 0;
                inFader[0] = BGM1[activePlayer].DOFade(1, duration).SetUpdate(true).OnComplete(() => inFader[0] = null);
                inFader[1] = BGM1[1-activePlayer].DOFade(1, duration).SetUpdate(true).OnComplete(() => inFader[1] = null);
            }
            else
            {
                BGM1[activePlayer].volume = 1;
                BGM1[1-activePlayer].volume = 1;
            }
        }

        firstSet = !firstSet;
        firstSongPlayed = true;

        // Set new clip to current song
        currentSong = music;
    }

    public void FadeOutCurrent(float duration = 1f)
    {
        carryOn = false;
        Utils.KillTween(ref outFader[0]);
        Utils.KillTween(ref outFader[1]);
        if (firstSet)
        {
            outFader[0] = BGM1[activePlayer].DOFade(0, duration).SetUpdate(true).OnComplete(() => { outFader[0] = null; currentSong = null; });
            outFader[1] = BGM1[1-activePlayer].DOFade(0, duration).SetUpdate(true).OnComplete(() => { outFader[1] = null; currentSong = null; });
        }
        else
        {
            outFader[0] = BGM2[activePlayer].DOFade(0, duration).SetUpdate(true).OnComplete(() => { outFader[0] = null; currentSong = null; });
            outFader[1] = BGM2[1-activePlayer].DOFade(0, duration).SetUpdate(true).OnComplete(() => { outFader[1] = null; currentSong = null; });
        }
    }

    public void FadeInCurrent(float duration = 1f)
    {
        if (gameObject == null) return;
        carryOn = false;

        // Not using inFader here because this is not a crossfade in the traditional sense, and we want fade in to interrupt fade out/vice versa
        Utils.KillTween(ref outFader[0]);
        Utils.KillTween(ref outFader[1]);
        if (firstSet)
        {
            outFader[0] = BGM1[activePlayer].DOFade(1, duration).SetUpdate(true).OnComplete(() => outFader[0] = null);
            outFader[1] = BGM1[1-activePlayer].DOFade(1, duration).SetUpdate(true).OnComplete(() => outFader[1] = null);
        }
        else
        {
            outFader[0] = BGM2[activePlayer].DOFade(1, duration).SetUpdate(true).OnComplete(() => outFader[0] = null);
            outFader[1] = BGM2[1-activePlayer].DOFade(1, duration).SetUpdate(true).OnComplete(() => outFader[1] = null);
        }
    }

    public void PauseCurrent()
    {
        if (firstSet)
        {
            BGM1[activePlayer].Pause();
            BGM1[1-activePlayer].Pause();
            if (carryOn && outFader[0] != null)
            {
                BGM2[activePlayer].Pause();
                BGM2[1-activePlayer].Pause();
                outFader[0].Pause();
                outFader[1].Pause();
            }
        }
        else
        {
            BGM2[activePlayer].Pause();
            BGM2[1-activePlayer].Pause();
            if (carryOn && outFader[0] != null)
            {
                BGM1[activePlayer].Pause();
                BGM1[1-activePlayer].Pause();
                outFader[0].Pause();
                outFader[1].Pause();
            }
        }
        paused = true;
    }

    public void UnPauseCurrent()
    {
        if (firstSet)
        {
            BGM1[activePlayer].UnPause();
            BGM1[1-activePlayer].UnPause();
            if (carryOn && outFader[0] != null)
            {
                BGM2[activePlayer].UnPause();
                BGM2[1-activePlayer].UnPause();
                outFader[0].Play();
                outFader[1].Play();
            }
        }
        else
        {
            BGM2[1-activePlayer].UnPause();
            BGM2[activePlayer].UnPause();
            if (carryOn && outFader[0] != null)
            {
                BGM1[activePlayer].UnPause();
                BGM1[1 - activePlayer].UnPause();
                outFader[0].Play();
                outFader[1].Play();
            }
        }
        paused = false;
    }

    public void Stop()
    {
        foreach (AudioSource source in BGM1)
        {
            source.Stop();
            source.clip = null;
        }
        foreach (AudioSource source in BGM2)
        {
            source.Stop();
            source.clip = null;
        }
        currentSong = null;
        paused = false;
    }

    public SoundPlayable FindSound(string soundPath)
    {
        List<string> path = new(soundPath.Trim().Split("."));
        return FindSound(soundDatabase, path);
    }

    public SoundPlayable FindSound(SoundNode current, List<string> path)
    {
        if (current is SoundPlayable playable)
        {
            return playable;
        }
        else if (current is SoundCategory category)
        {
            foreach (SoundNode node in category.children)
            {
                if (path.Count > 0 && string.Equals(node.name, path[0], System.StringComparison.OrdinalIgnoreCase))
                {
                    // Debug.Log("Found " + path[0]);
                    current = node;
                    path.RemoveAt(0);
                    return FindSound(node, path);
                }
            }
            Debug.LogError("Invalid sound path provided!");
            return null;
        }
        Debug.LogError("Invalid sound path provided!");
        return null;
    }

    public MusicClip FindMusic(string musicPath)
    {
        List<string> path = new(musicPath.Trim().Split("."));
        return FindMusic(musicDatabase, path);
    }

    public MusicClip FindMusic(SoundNode current, List<string> path)
    {
        if (current is MusicClip clip)
        {
            return clip;
        }
        else if (current is MusicCategory category)
        {
            foreach (SoundNode node in category.children)
            {
                if (path.Count > 0 && string.Equals(node.name, path[0], System.StringComparison.OrdinalIgnoreCase))
                {
                    current = node;
                    path.RemoveAt(0);
                    return FindMusic(node, path);
                }
            }
            Debug.LogError("Invalid music path provided!");
            return null;
        }
        Debug.LogError("Invalid music path provided!");
        return null;
    }

    public bool OwnsSource(AudioSource source)
    {
        return source == BGM1[0] || source == BGM1[1] || source == BGM2[0] || source == BGM2[1];
    }

    public void PauseEffect(bool active)
    {
        if (inCutscene)
        {
            if (active) PauseCurrent();
            else UnPauseCurrent();
            return;
        }
        
        DOTween.To(() => lowPass, x => lowPass = x, active ? 1815.00f : 22000.00f, 0.5f).SetUpdate(true);
    }

    public AudioSource GetPermaSource()
    {
        return permaSource;
    }
}