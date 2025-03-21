using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private OVRHand leftHand;
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);  // Load scene asynchronously
        asyncLoad.allowSceneActivation = false;  // Optional: Prevent automatic scene activation

        while (!asyncLoad.isDone)
        {

            if (asyncLoad.progress >= 0.9f)  // Optional: Once loading is complete
            {
                asyncLoad.allowSceneActivation = true;  // Activate the scene
            }

            yield return null;  // Wait for the next frame
        }
    }

    private void Update()
    {
        if (leftHand.GetFingerIsPinching(OVRHand.HandFinger.Middle))  // Check if the index finger is pinching
        {
            ChangeScene("intro-scene");  // Change scene
        }
    }
}