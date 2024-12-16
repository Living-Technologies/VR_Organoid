using UnityEngine;
using Oculus.Interaction.Input;

public class ObjectScaler : MonoBehaviour
{
    [SerializeField] private GameObject targetObject; // Object to scale
    [SerializeField] private float scaleSpeed = 0.5f; // Scaling speed multiplier
    [SerializeField] private float minScale = 0.1f; // Minimum allowed scale
    [SerializeField] private float maxScale = 3f; // Maximum allowed scale

    [SerializeField] private Hand leftHand; // Reference to left hand
    [SerializeField] private Hand rightHand; // Reference to right hand

    private void Update()
    {
        if (targetObject == null || leftHand == null || rightHand == null) return;

        // Check left-hand pinch (thumb + middle finger)
        if (IsPinching(leftHand))
        {
            ScaleObject(-scaleSpeed * Time.deltaTime);
        }

        // Check right-hand pinch (thumb + middle finger)
        if (IsPinching(rightHand))
        {
            ScaleObject(scaleSpeed * Time.deltaTime);
        }
    }

    private bool IsPinching(Hand hand)
    {
        if (hand == null) return false;

        // Check if thumb and middle finger are pinching
        bool isThumbPinching = hand.GetFingerIsPinching(HandFinger.Thumb);
        bool isMiddlePinching = hand.GetFingerIsPinching(HandFinger.Middle);

        return isThumbPinching && isMiddlePinching;
    }

    private void ScaleObject(float scaleDelta)
    {
        Vector3 currentScale = targetObject.transform.localScale;
        float newScale = Mathf.Clamp(currentScale.x + scaleDelta, minScale, maxScale);
        targetObject.transform.localScale = new Vector3(newScale, newScale, newScale);
    }
}
