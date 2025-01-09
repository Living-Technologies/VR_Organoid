using UnityEngine;

public class ScaleSetter : MonoBehaviour
{
    public void ChangeScale(float scaleValue)
    {
        // Apply scaleValue to the GameObject
        transform.localScale = Vector3.one * scaleValue;
    }
}
