using UnityEngine;
using System.Collections.Generic;

public class TimeFrameSwitcher : MonoBehaviour
{
    public List<GameObject> timeframes;  // List of all timeframe objects
    private int currentFrameIndex = 0;   // Index of the currently active timeframe

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
}
