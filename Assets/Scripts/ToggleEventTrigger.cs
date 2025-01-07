using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ToggleEventTrigger : MonoBehaviour
{
    public Toggle toggle; // The toggle you are using
    public UnityEvent onToggleActivated; // The event to trigger when activated

    private bool previousState = false;

    void Start()
    {
        // Initialize previous state with the toggle's initial state
        previousState = toggle.isOn;

        // Add listener to handle toggle changes
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        // Check if the toggle was just activated (false to true)
        if (!previousState && isOn)
        {
            // Trigger the event
            onToggleActivated.Invoke();
        }

        // Update previous state
        previousState = isOn;
    }
}
