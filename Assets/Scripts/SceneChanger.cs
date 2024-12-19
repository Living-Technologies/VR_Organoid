using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    public string sceneName;  // The name of the scene to load
    public Slider progressBar;  // Reference to the UI slider for loading progress

    public void ChangeScene()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);  // Load scene asynchronously
        asyncLoad.allowSceneActivation = false;  // Optional: Prevent automatic scene activation

        while (!asyncLoad.isDone)
        {
            progressBar.value = asyncLoad.progress;  // Update the slider with progress

            if (asyncLoad.progress >= 0.9f)  // Optional: Once loading is complete
            {
                asyncLoad.allowSceneActivation = true;  // Activate the scene
            }

            yield return null;  // Wait for the next frame
        }
    }
}
