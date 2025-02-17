using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Profiling;
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
        for (int i = 0; i < rows.Length; i++)
        {
            for (int j = 1; j <= columns; j++)
            {
                GameObject button = Instantiate(buttonPrefab, gridParent);
                string position = $"{rows[i]}{j}";

                // Set button text
                TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null) buttonText.text = position;

                // Add listener properly using a captured variable
                Button btnComponent = button.GetComponent<Button>();
                if (btnComponent != null)
                {
                    btnComponent.onClick.AddListener(() => OnCellClicked(position));
                }
            }
        }
    }

    void OnCellClicked(string position)
    {
        Debug.Log("Clicked: " + position);
        if (selectionText != null)
        {
            selectionText.text = "Selected: " + position;
        }
    }
}
