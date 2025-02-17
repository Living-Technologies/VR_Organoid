using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform gridParent;
    public TextMeshProUGUI selectionText;
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
                button.GetComponentInChildren<Text>().text = position;
                button.GetComponent<Button>().onClick.AddListener(() => OnCellClicked(position));
            }
        }
    }

    void OnCellClicked(string position)
    {
        if (selectionText != null)
        {
            selectionText.text = "Selected: " + position;
        }
    }
}
