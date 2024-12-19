using UnityEngine;
using System.Collections.Generic;

public class TimeFrameSwitcher : MonoBehaviour
{
    public List<GameObject> timeframes;  // List of all timeframe objects
    public ParentController parentController; // Reference to ParentController to access active child info
    private int currentFrameIndex = 0;   // Index of the currently active timeframe

    void Start()
    {
        UpdateActiveTimeFrame();
    }

    void Update()
    {
        // Check for Enter key or Meta Quest button press
        if (Input.GetKeyDown(KeyCode.Return) || IsRightGestureDetected())
        {
            SwitchToNextTimeFrame();
        }
        else if (IsLeftGestureDetected())
        {
            SwitchToPreviousTimeFrame();
        }
    }

    public void SwitchToNextTimeFrame()
    {
        timeframes[currentFrameIndex].SetActive(false);
        currentFrameIndex = (currentFrameIndex + 1) % timeframes.Count;
        UpdateActiveTimeFrame();
    }

    public void SwitchToPreviousTimeFrame()
    {
        timeframes[currentFrameIndex].SetActive(false);
        currentFrameIndex = (currentFrameIndex - 1 + timeframes.Count) % timeframes.Count;
        UpdateActiveTimeFrame();
    }

    void UpdateActiveTimeFrame()
    {
        for (int i = 0; i < timeframes.Count; i++)
        {
            timeframes[i].SetActive(i == currentFrameIndex);
        }
    }

    bool IsRightGestureDetected()
    {
        // Replace `Button.One` with the correct Oculus right-hand button/gesture reference
        return OVRInput.GetDown(OVRInput.Button.One);
    }

    bool IsLeftGestureDetected()
    {
        // Replace `Button.Two` with the correct Oculus left-hand button/gesture reference
        return OVRInput.GetDown(OVRInput.Button.Two);
    }
}
