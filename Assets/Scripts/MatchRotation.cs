using UnityEngine;

public class RotationController : MonoBehaviour
{
    public Transform controlledObject; // Object that follows this rotation

    void Update()
    {
        if (controlledObject != null)
        {
            controlledObject.rotation = transform.rotation;
        }
    }
}
