using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Profiling;
using System.Reflection;

public class GridManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform gridParent;
    public TextMeshProUGUI selectionText; // Reference to TMP Text

    private char[] rows = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
    private int columns = 12;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        GameObject sceneManagerObject = GameObject.Find("ClickManager");
        TiffReader tiffReader = sceneManagerObject.GetComponent<TiffReader>();

        for (int i = 0; i < rows.Length; i++)
        {
            for (int j = 1; j <= columns; j++)
            {
                GameObject button = Instantiate(buttonPrefab, gridParent);
                string position = $"{rows[i]}{j}";
                button.name = position;
                // Set button text
                Text buttonText = button.GetComponentInChildren<Text>();
                if (buttonText != null) buttonText.text = position;

                // Add listener properly using a captured variable
                Button btnComponent = button.GetComponent<Button>();
                Debug.Log($"Button component: {button.name}");
                if (btnComponent != null)
                {
                    Debug.Log($"Adding listener to button {position}");
                    string capturedPosition = position; // Capture position in a local variable
                    btnComponent.onClick.AddListener(() => OnButtonClick(capturedPosition));
                }

            }
        }
    }

    private void OnButtonClick(string well)
    {
        TiffReader tiffReader = FindObjectOfType<TiffReader>();
        if (tiffReader != null)
        {
            tiffReader.GetTexture(well);
        }
        else
        {
            Debug.LogError("TiffReader not found!");
        }
    }

}
