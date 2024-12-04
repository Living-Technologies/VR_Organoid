using UnityEngine;

public class ScaleChildren : MonoBehaviour
{
    public float scaleFactor = 1.5f; // Scale factor for duplication
    public Material newMaterial;    // New material to assign to original objects

    void Start()
    {
        if (newMaterial == null)
        {
            Debug.LogError("Please assign a new material in the inspector.");
            return;
        }

        // Create a separate list to iterate over to avoid hierarchy modification issues
        Transform[] originalChildren = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            originalChildren[i] = transform.GetChild(i);
        }

        foreach (Transform child in originalChildren)
        {
            // Duplicate child object
            GameObject duplicatedChild = Instantiate(child.gameObject, child.position, child.rotation);
            duplicatedChild.transform.parent = transform;

            // Scale the duplicate around its center
            duplicatedChild.transform.localScale = child.localScale * scaleFactor;

            // Assign new material to the original child
            MeshRenderer renderer = child.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = newMaterial;
            }
        }
    }
}
