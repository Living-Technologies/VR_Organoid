// using UnityEngine;

// public class SphereRotationController : MonoBehaviour
// {
//     [SerializeField] private Transform targetObject; // The object to control
//     private Vector3 lastHandPosition;

//     void Update()
//     {
//         if (Input.GetMouseButton(0)) // Replace with VR input check
//         {
//             // Get the current hand position (or controller in VR)
//             Vector3 currentHandPosition = Input.mousePosition; // Replace with VR hand/controller position

//             if (lastHandPosition != Vector3.zero)
//             {
//                 // Calculate rotation delta based on hand movement
//                 Vector3 delta = currentHandPosition - lastHandPosition;
//                 float rotationX = delta.y; // Vertical movement rotates around X-axis
//                 float rotationY = -delta.x; // Horizontal movement rotates around Y-axis

//                 // Apply rotation to the sphere
//                 transform.Rotate(rotationX, rotationY, 0, Space.World);

//                 // Map the sphere's rotation to the target object
//                 if (targetObject != null)
//                 {
//                     targetObject.rotation = transform.rotation;//                 }
//             }

//             lastHandPosition = currentHandPosition;
//         }
//         else
//         {
//             lastHandPosition = Vector3.zero; // Reset when not interacting
//         }
//     }
// }
using UnityEngine;

public class RotationGizmoWithGLCurvedAxes : MonoBehaviour
{
    public Transform targetObject1; // First object to rotate
    public Transform targetObject2; // Second object to rotate
    public float gizmoRadius = 1f; // Radius of the rotation sphere
    public float rotationSpeed = 2f; // Speed of rotation
    public int segments = 64; // Number of segments for smooth arcs

    private bool isDragging = false;
    private Vector3 dragStart;

    private Material lineMaterial;

    void Start()
    {
        // Create material for lines if it doesn't exist
        CreateLineMaterial();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragStart = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging && targetObject1 != null && targetObject2 != null)
        {
            Vector3 dragDelta = Input.mousePosition - dragStart;
            float rotationX = dragDelta.y * rotationSpeed * Time.deltaTime; // Vertical movement
            float rotationY = -dragDelta.x * rotationSpeed * Time.deltaTime; // Horizontal movement

            // Apply rotation to both targets
            targetObject1.Rotate(Vector3.up, rotationY, Space.World);
            targetObject1.Rotate(Vector3.right, rotationX, Space.World);

            targetObject2.Rotate(Vector3.up, rotationY, Space.World);
            targetObject2.Rotate(Vector3.right, rotationX, Space.World);

            dragStart = Input.mousePosition; // Reset drag start
        }
    }

    void OnDrawGizmos()
    {
        // Only draw in the editor using Gizmos, not GL lines
        if (!Application.isPlaying)
        {
            DrawCurvedAxes();
        }
    }

    // Draw the curved axes using GL
    private void DrawCurvedAxes()
    {
        if (lineMaterial == null)
        {
            CreateLineMaterial();
        }

        lineMaterial.SetPass(0);

        // Draw X-axis arc (red)
        DrawArc(transform.position, transform.right, transform.up, gizmoRadius, segments, Color.red);

        // Draw Y-axis arc (green)
        DrawArc(transform.position, transform.up, transform.forward, gizmoRadius, segments, Color.green);

        // Draw Z-axis arc (blue)
        DrawArc(transform.position, transform.forward, transform.right, gizmoRadius, segments, Color.blue);
    }

    // Helper to draw arc
    private void DrawArc(Vector3 center, Vector3 normal, Vector3 tangent, float radius, int segments, Color color)
    {
        GL.Begin(GL.LINES);
        GL.Color(color);

        Vector3 previousPoint = center + (tangent * radius); // Starting point of the arc

        for (int i = 1; i <= segments; i++)
        {
            float angle = (360f / segments) * i;
            Quaternion rotation = Quaternion.AngleAxis(angle, normal); // Rotate around the normal
            Vector3 nextPoint = center + (rotation * (tangent * radius)); // Next point on the arc

            GL.Vertex(previousPoint); // Start of the line
            GL.Vertex(nextPoint); // End of the line

            previousPoint = nextPoint; // Update for the next segment
        }

        GL.End();
    }

    // Create a simple unlit material for drawing
    private void CreateLineMaterial()
    {
        if (lineMaterial == null)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    // Make sure GL lines are drawn in the proper rendering phase
    void OnRenderObject()
    {
        // Only draw in runtime
        if (Application.isPlaying)
        {
            DrawCurvedAxes(); // Ensure lines render during runtime
        }
    }
}
