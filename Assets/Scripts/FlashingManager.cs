using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingManager : MonoBehaviour
{
    //Components
    public Transform holeParentObject;

    //Gameplay variables
    List<Hole> holeList = new List<Hole>();

    public float amountOfBlueHolesNeeded = 2;

    public float minTime = 10f;
    public float maxTime = 20f;
    public float currentTime;
    public float targetTime;
    

    public bool readyToFlash = false;

    public bool timerRunning = false;

    private static FlashingManager instance;
    public static FlashingManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType(typeof(FlashingManager)) as FlashingManager;

            return instance;
        }
        set
        {
            instance = value;
        }
    }


    void Update()
    {
        //Timer logic.
        if (timerRunning)
        {
            currentTime += Time.deltaTime;

            //if timer reaches the target time, flashing event is ready to initiate.
            if (currentTime >= targetTime)
            {
                readyToFlash = true;
                timerRunning = false;
            }
        }
       
    }

    //Resets and starts the flashing timer and picks a random time for a new duration.
    public void ResetTimerAndStart()
    {
        readyToFlash = false;
        currentTime = 0;
        targetTime = Random.Range(minTime, maxTime);
        timerRunning = true;
    }

    public void FlashHoles()
    {
        //Makes a new list of all the current golf holes
        List<Hole> remaining = CurrentGolfHoles();

        //Picks a defined amount random holes and changes them to blue.
        for (int i = 0; i < amountOfBlueHolesNeeded; i++)
        {
            if (remaining.Count == 0)
            {
                break;
            }
            int randomHoleIndex = Random.Range(0, remaining.Count);
            Hole selectedHole = remaining[randomHoleIndex];
            selectedHole.StartCoroutine(selectedHole.FlashHole(HoleType.Blue));
            remaining.RemoveAt(randomHoleIndex);
        }

        //Rest are turned to red.
        foreach (Hole h in remaining)
        {
            h.StartCoroutine(h.FlashHole(HoleType.Red));
        }

        holeList.Clear();
    }

    //Makes a list of current golf holes
    private List<Hole> CurrentGolfHoles()
    {
        foreach (Hole hole in holeParentObject.GetComponentsInChildren<Hole>())
        {
            holeList.Add(hole);
        }

        List<Hole> remaining = new List<Hole>(holeList);
        return remaining;
    }
}
