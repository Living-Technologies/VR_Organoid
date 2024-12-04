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
        if (Input.GetKeyDown(KeyCode.Return) || IsMetaQuestButtonPressed())
        {
            SwitchToNextTimeFrame();
        }
    }

    void SwitchToNextTimeFrame()
    {
        // Deactivate the current timeframe
        timeframes[currentFrameIndex].SetActive(false);

        // Move to the next timeframe in the list
        currentFrameIndex = (currentFrameIndex + 1) % timeframes.Count;

        // Activate the new current timeframe
        UpdateActiveTimeFrame();
    }

    void UpdateActiveTimeFrame()
    {
        // Make only the current timeframe active
        for (int i = 0; i < timeframes.Count; i++)
        {
            timeframes[i].SetActive(i == currentFrameIndex);
        }
    }

    bool IsMetaQuestButtonPressed()
    {
        // Replace `Button.One` with the correct Oculus button reference
        return OVRInput.GetDown(OVRInput.Button.One);
    }
}
