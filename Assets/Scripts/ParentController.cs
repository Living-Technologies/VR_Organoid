using UnityEngine;
using System.Collections.Generic;

public class ParentController : MonoBehaviour
{
    public List<GameObject> otherParentObjects;  // List of all parent objects (timeframes)
    public float transparentAlpha = 0.3f; // The alpha value for making objects transparent

    public string activeChildName = "";  // Track the active child's name

    void Start()
    {
        // Ensure each child has a ChildClick script
        foreach (Transform child in transform)
        {
            var childClickScript = child.gameObject.AddComponent<ChildClick>();
            childClickScript.parentController = this;
        }
    }

    public void HandleChildClick(GameObject clickedChild)
    {
        // Get the name of the clicked child
        string clickedChildName = clickedChild.name;

        // If the clicked child is already active, no need to do anything
        if (clickedChildName == activeChildName)
        {
            return;
        }

        // Deactivate all children in all timeframes (but keep them opaque)
        DeactivateAllChildrenInAllTimeframes();

        // Activate the clicked child in all timeframes
        ActivateChildInAllTimeframes(clickedChildName);

        // Update the active child name
        activeChildName = clickedChildName;
    }

    private void DeactivateAllChildrenInAllTimeframes()
    {
        // Deactivate all children in this parent object
        DeactivateAllChildren(transform);

        // Deactivate all children in each parent object in otherParentObjects
        foreach (GameObject parent in otherParentObjects)
        {
            DeactivateAllChildren(parent.transform);
        }
    }

    private void DeactivateAllChildren(Transform parentTransform)
    {
        foreach (Transform child in parentTransform)
        {
            // Instead of deactivating, make the child transparent
            SetObjectTransparency(child.gameObject, transparentAlpha); 
        }
    }

    private void ActivateChildInAllTimeframes(string childName)
    {
        // Activate in the current parent
        ActivateChild(transform, childName);

        // Activate in each parent object in otherParentObjects
        foreach (GameObject parent in otherParentObjects)
        {
            ActivateChild(parent.transform, childName);
        }
    }

    private void ActivateChild(Transform parentTransform, string childName)
    {
        Transform child = parentTransform.Find(childName);
        if (child != null)
        {
            child.gameObject.SetActive(true);
            SetObjectTransparency(child.gameObject, 1.0f); // Make it fully opaque
        }
    }

    private void SetObjectTransparency(GameObject obj, float alpha)
    {
        // Get all renderers (useful if object has multiple parts)
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            // Ensure the material supports transparency
            Material material = renderer.material;
            Color color = material.color;
            color.a = alpha;
            material.color = color;

            // Set the material's rendering mode to transparent if it isn't already
            material.SetFloat("_Mode", 3); // 3 is for transparent in Unity's Standard Shader
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0); // Disable Z-Write
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000; // Set the render queue to render transparent last

            // Set ZTest to Always to avoid depth issues with transparency
            material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always); // Render transparency without depth checks
        }
    }
}
