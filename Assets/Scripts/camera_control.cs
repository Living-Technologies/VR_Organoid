using UnityEngine;
using System.Collections;

public class FlyCamera : MonoBehaviour 
{
    float mainSpeed = 50.0f; // regular speed
    float shiftAdd = 250.0f; // additional speed when Shift is held
    float maxShift = 1000.0f; // maximum speed with Shift
    float camSens = 0.25f; // mouse sensitivity
    private Vector3 lastMouse = new Vector3(255, 255, 255); // initial middle screen position
    private float totalRun = 1.0f;
    
    private float yaw; // horizontal rotation angle
    private float pitch; // vertical rotation angle
    
    void Start()
    {
        // Initialize rotation angles from the current camera rotation
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }
    
    void Update () 
    {
        // Mouse rotation control
        Vector3 mouseDelta = Input.mousePosition - lastMouse;
        lastMouse = Input.mousePosition;

        // Adjust yaw and pitch based on mouse movement
        yaw += mouseDelta.x * camSens;
        pitch -= mouseDelta.y * camSens; // Invert Y-axis for natural camera control

        // Apply rotation as a quaternion to avoid gimbal lock
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Keyboard movement control
        Vector3 moveDirection = GetBaseInput();
        if (moveDirection.sqrMagnitude > 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                totalRun += Time.deltaTime; // Increase speed over time when Shift is held
                moveDirection *= totalRun * shiftAdd;
                moveDirection = Vector3.ClampMagnitude(moveDirection, maxShift); // Limit to maxShift
            }
            else
            {
                totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f); // Reset speed over time
                moveDirection *= mainSpeed;
            }

            moveDirection *= Time.deltaTime;

            // Move on XZ axis only when Space is held
            if (Input.GetKey(KeyCode.Space))
            {
                transform.Translate(moveDirection.x, 0, moveDirection.z, Space.Self);
            }
            else
            {
                transform.Translate(moveDirection, Space.Self);
            }
        }
    }

    // Retrieves base movement direction based on WASD keys
    private Vector3 GetBaseInput() 
    {
        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) direction += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) direction += Vector3.back;
        if (Input.GetKey(KeyCode.A)) direction += Vector3.left;
        if (Input.GetKey(KeyCode.D)) direction += Vector3.right;
        return direction;
    }
}
