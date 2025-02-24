using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;  

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;  // Wait for the scene to load up to 90%
        }

        // Add a delay or display a UI message before switching
        yield return new WaitForSeconds(1f);  // Example delay

        asyncLoad.allowSceneActivation = true;  // Activate the scene
    }

}
