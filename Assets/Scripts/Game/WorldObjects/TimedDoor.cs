using System.Collections.Generic;
using UnityEngine;

public class TimedDoor : MonoBehaviour
{
    public List<DoorScheduleEntry> schedule = new List<DoorScheduleEntry>();

    public Collider2D cldr;

    public int warningTime = 30;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.OnBeat += Onbeat;
    }

    public void Onbeat(int beatNum)
    {
        for(int i = schedule.Count - 1; i >= 0; i--)
        {
            if (schedule[i].beatNum == beatNum)
            {
                if (schedule[i].toState == DoorState.Open)
                    Open();
                else
                    Close();
            }
            //For animating the fill in, incremental visual changes should happen to show it is filling in soon
            if (schedule[i].beatNum == beatNum + warningTime)
            {

            }
            
        }
    }

    public void Open()
    {
        cldr.enabled = false;
    }

    public void Close()
    {
        cldr.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        AudioManager.OnBeat -= Onbeat;
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
