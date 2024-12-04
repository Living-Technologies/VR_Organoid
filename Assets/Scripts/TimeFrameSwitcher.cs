using UnityEngine;
using UnityEngine.UI; // For Slider and UI components
using System.Collections.Generic;

public class TimeFrameSwitcher : MonoBehaviour
{
    public List<GameObject> timeframes;  // List of all timeframe objects
    public Text modelText;               // Reference to a Text UI element to display the model name
    public Text volumeText;              // Reference to a Text UI element to display the volume
    public Slider timeframeSlider;       // Reference to the UI Slider component
    public ParentController parentController; // Reference to ParentController to access active child info
    private int currentFrameIndex = 0;   // Index of the currently active timeframe

    void Start()
    {
        if (timeframeSlider != null)
        {
            // Dynamically adjust the slider range based on the number of timeframes
            timeframeSlider.minValue = 0;
            timeframeSlider.maxValue = timeframes.Count - 1;

            // Set the initial slider value
            timeframeSlider.value = currentFrameIndex;

            // Add a listener to respond to slider value changes
            timeframeSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        else
        {
            Debug.LogError("Timeframe Slider is not assigned.");
        }

        // Initialize active timeframe
        UpdateActiveTimeFrame();
    }

    void OnSliderValueChanged(float value)
    {
        // Update the current frame index based on the slider's value
        currentFrameIndex = Mathf.RoundToInt(value);

        // Update the active timeframe
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
        Debug.Log($"Model name: {modelName}");
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
