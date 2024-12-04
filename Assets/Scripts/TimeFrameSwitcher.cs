using UnityEngine;
using UnityEngine.UI; // For UI Text component
using System.Collections.Generic;

public class TimeFrameSwitcher : MonoBehaviour
{
    public List<GameObject> timeframes;  // List of all timeframe objects
    public Text modelText;               // Reference to a Text UI element to display the model name
    public Text volumeText;              // Reference to a Text UI element to display the volume
    public ParentController parentController; // Reference to ParentController to access active child info
    private int currentFrameIndex = 0;   // Index of the currently active timeframe

    void Start()
    {
        // Set only the first timeframe to active, hide the rest
        UpdateModelText(timeframes[currentFrameIndex].name);
        UpdateActiveTimeFrame();
    }

    void Update()
    {
        // Switch timeframes on Enter key press
        if (Input.GetKeyDown(KeyCode.Return))
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
        // Make only the current timeframe active and update the model name
        for (int i = 0; i < timeframes.Count; i++)
        {
            timeframes[i].SetActive(i == currentFrameIndex);
        }

        UpdateModelText(timeframes[currentFrameIndex].name);

        // Call to update volume info when the timeframe switches
        UpdateVolumeInfo(timeframes[currentFrameIndex]);
    }

    void UpdateModelText(string modelName)
    {
        Debug.Log($"Model name: {modelName}".GetType());
        if (modelText != null)
        {
            modelText.text = $"Model name: {modelName}";
            Debug.Log("Updated UI text to: " + modelText.text);
        }
        else
        {
            Debug.LogWarning("Model Text UI is not assigned.");
        }
    }

    void UpdateVolumeInfo(GameObject currentFrame)
    {
        // Ensure volumeText is assigned
        if (volumeText == null)
        {
            Debug.LogError("Volume Text UI is not assigned.");
            return;
        }

        // Get the active child from the current timeframe
        Transform activeChild = currentFrame.transform.Find(parentController.activeChildName);

        if (activeChild != null)
        {
            // Get the ChildClick script from the active child
            ChildClick childClickScript = activeChild.GetComponent<ChildClick>();
            if (childClickScript != null)
            {
                // Call CalculateMeshVolume using the ChildClick class directly
                float volume = ChildClick.CalculateMeshVolume(activeChild.gameObject);
                
                // Get the nucleus name
                string nucleusName = activeChild.gameObject.name;

                // Display both the nucleus name and volume with 3 decimal points
                volumeText.text = $"Nucleus: {nucleusName}\nVolume: {volume:F3}";
                Debug.Log("Updated Nucleus Name: " + nucleusName + " Volume: " + volume);
            }
        }
        else
        {
            Debug.LogWarning("Active child not found in current frame.");
        }
    }
}
