using System.Collections.Generic;
using UnityEngine;

public class TimedDoor : MonoBehaviour
{
    public List<DoorScheduleEntry> schedule = new List<DoorScheduleEntry>();

    public Collider2D cldr;

    ////Probably should be a different measure to not require using all 5 fill sprites?
    //public int TimePerStage = 4;
    DoorScheduleEntry lastEvent;

    public Animator anim;
    private bool pastFirstEvent = false;

    public bool isPit = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.OnBeat += Onbeat;
        anim.SetBool("IsPit", isPit);
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
                    if (findNextScheduleEvent(beatNum).beatNum - lastEvent.beatNum != 0)
                    {
                        anim.SetFloat("Stage", (float)(findNextScheduleEvent(beatNum).beatNum - beatNum) / (findNextScheduleEvent(beatNum).beatNum - lastEvent.beatNum));
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
    }

    public void Close()
    {
        cldr.enabled = true;
        anim.SetFloat("Stage", 1);
    }

    // Update is called once per frame
    void Update()
    {
        
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
