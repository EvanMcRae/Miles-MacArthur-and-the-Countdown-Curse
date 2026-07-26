using System;
using System.Collections.Generic;
using UnityEngine;

public class TimedDoor : MonoBehaviour
{
    public List<DoorScheduleEntry> schedule = new List<DoorScheduleEntry>();

    public BoxCollider2D cldr;

    ////Probably should be a different measure to not require using all 5 fill sprites?
    //public int TimePerStage = 4;
    DoorScheduleEntry lastEvent;

    public Animator anim;
    private bool pastFirstEvent = false;

    public bool isPit = false;

    [SerializeField] private SoundPlayer soundPlayer;
    [SerializeField] private SoundClip close, open;
    [SerializeField] private SoundPlayable fill;
    private Sprite previousStage;

    [SerializeField]
    GameObject particleGameObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.OnBeat += Onbeat;
        anim.SetBool("IsPit", isPit);
        Onbeat(1);

        int totalBeats = 0;
        foreach (DoorScheduleEntry entry in schedule)
        {
            totalBeats += entry.beatNum;
        }
        
        if (particleGameObject != null)
        {
            int columns = (int)cldr.size.x;
            int rows = (int)cldr.size.y;
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    GameObject particleGO = Instantiate(particleGameObject, transform.position + new Vector3(-(int)(columns / 2) + i - .5f * (columns % 2 - 1), -(int)(rows / 2) + j - .5f * (rows % 2 - 1), 0), Quaternion.identity, this.transform);
                    Particle particle = particleGO.GetComponent<Particle>();
                    particle.BeatsToClean = totalBeats;
                    particle.BeatsLeftUnilClean = totalBeats;
                }
            }
        }
    }

    public void Update()
    {
        if (GameManager.paused || GameManager.quitting) return;

        if (anim.GetComponent<SpriteRenderer>().sprite != previousStage && anim.GetFloat("Stage") != 1 && anim.GetFloat("Stage") != 0)
        {
            soundPlayer.PlaySound(fill);
        }
        previousStage = anim.GetComponent<SpriteRenderer>().sprite;
    }

    public void Onbeat(int beatNum)
    {
        for(int i = schedule.Count - 1; i >= 0; i--)
        {
            if (schedule[i].beatNum == beatNum)
            {
                lastEvent = schedule[i];
                if (schedule[i].toState == DoorState.Open)
                    Open();
                else
                    Close();
                pastFirstEvent = true;
            }
            if (pastFirstEvent)
            {
                if (lastEvent == null)
                {
                    anim.SetFloat("Stage", (findNextScheduleEvent(beatNum).beatNum - beatNum) / 6.0f);
                }
                else
                {
                    //Divide space between events as evenly into 5 stages as possible.
                    DoorScheduleEntry nextEvent = findNextScheduleEvent(beatNum);
                    if (nextEvent.beatNum - lastEvent.beatNum != 0)
                    {
                        float progress = (float)(nextEvent.beatNum - beatNum) / (nextEvent.beatNum - lastEvent.beatNum);
                        if (nextEvent.toState == DoorState.Open)
                            anim.SetFloat("Stage", progress);
                        else
                            anim.SetFloat("Stage", 1 - progress);
                    }
                }
            }
        }
    }

    DoorScheduleEntry findNextScheduleEvent(int currBeatNum)
    {
        if (schedule.Count <= 0)
            return null;
        DoorScheduleEntry soonestEvent = schedule[0];
        foreach (DoorScheduleEntry entry in schedule)
        {
            if ((entry.beatNum - currBeatNum >= 0 && entry.beatNum < soonestEvent.beatNum)
                || (soonestEvent.beatNum - currBeatNum < 0))
                soonestEvent = entry;
        }
        return soonestEvent;
    }

    public void Open()
    {
        cldr.enabled = false;
        anim.SetFloat("Stage", 0);
        if (pastFirstEvent)
            soundPlayer.PlaySound(open);
    }

    public void Close()
    {
        cldr.enabled = true;
        anim.SetFloat("Stage", 1);
        if (pastFirstEvent)
            soundPlayer.PlaySound(close);
    }
}
[System.Serializable]
public class DoorScheduleEntry
{
    public int beatNum;
    public DoorState toState;
}

public enum DoorState
{
    Open,
    Closed
}
