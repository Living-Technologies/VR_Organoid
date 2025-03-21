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
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone) // Ensure full loading
        {
            if (asyncLoad.progress >= 0.9f)
            {
                // Optional: Add a small delay to ensure stability
                yield return new WaitForSeconds(1f);
                
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
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