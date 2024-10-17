using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Text))]
public class TextGrid : MonoBehaviour
{
    private const float VERTICAL_SPACING = -60f;
    private const float HORIZONTAL_SPACING = -5f;
    private const char CELL_CHAR = '□';

    [SerializeField, Min(2)] private int m_gridSize = 5;
    [SerializeField, Min(1)] private int m_cellSize = 250;
    [SerializeField] private TMP_FontAsset m_font;
    [SerializeField] private Color m_defaultColor;

    private TMP_Text m_label;
    private ContentSizeFitter m_sizeFitter;
    private GridLayoutGroup m_grid;
    private TMP_Text[] m_cells;

    private void Start()
    {
        LoadComponents();
    }

    public string GetCell(int x, int y)
    {
        int index = y * m_gridSize + x;

        // Prevent the code from executing if the index given is outside the maximum possible range of values
        if (index < 0 || index >= (m_gridSize * m_gridSize))
            return null;

        // Initialize cells array if it's null or not matching the expected size
        if (m_cells == null || m_cells.Length != m_gridSize * m_gridSize)
        {
            m_cells = new TMP_Text[m_gridSize * m_gridSize];
        }

        if (m_cells[index] == null)
            return null;

        return m_cells[index].text;
    }

    public void SetCell(int x, int y, string text)
    {
        int index = y * m_gridSize + x;

        // Prevent the code from executing if the index given is outside the maximum possible range of values
        if (index < 0 || index >= (m_gridSize * m_gridSize))
            return;

        // Initialize cells array if it's null or not matching the expected size
        if (m_cells == null || m_cells.Length != m_gridSize * m_gridSize)
        {
            m_cells = new TMP_Text[m_gridSize * m_gridSize];
        }

        // Create any missing cells up to the specified index
        for (int i = 0; i <= index; i++)
        {
            // If the cell at the index is null, create a new GameObject and add a TextMeshProUGUI component
            if (m_cells[i] == null)
            {
                GameObject cellObject = new GameObject($"Cell_{i / m_gridSize}_{i % m_gridSize}");
                cellObject.transform.SetParent(m_grid.transform, false);
                TMP_Text cellText = cellObject.AddComponent<TextMeshProUGUI>();

                // Set default font size and style for the new cell
                cellText.fontSize = 18;
                cellText.alignment = TextAlignmentOptions.Center;
                cellText.font = m_font;
                cellText.color = m_defaultColor;
                m_cells[i] = cellText;

#if UNITY_EDITOR
                cellObject.hideFlags = HideFlags.NotEditable;
#endif
            }
        }

        // Get the text component at the index and set the text
        m_cells[index].text = text;
    }

    private void GenerateGrid()
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < m_gridSize; i++)
        {
            string line = "";
            for (int j = 0; j < m_gridSize; j++)
            {
                line += CELL_CHAR;
            }
            sb.AppendLine(line);
        }

        m_label.text = sb.ToString();
    }

    private void LoadComponents()
    {
        if (m_label == null && TryGetComponent(out m_label) is false)
        {
            m_label = gameObject.AddComponent<TMP_Text>();
        }

        if (m_sizeFitter == null && TryGetComponent(out m_sizeFitter) is false)
        {
            m_sizeFitter = gameObject.AddComponent<ContentSizeFitter>();
        }

        if (m_grid == null && TryGetComponent(out m_grid) is false)
        {
            m_grid = gameObject.AddComponent<GridLayoutGroup>();
        }

#if UNITY_EDITOR

        m_label.hideFlags = HideFlags.NotEditable;
        m_grid.hideFlags = HideFlags.NotEditable;
        m_sizeFitter.hideFlags = HideFlags.NotEditable;

#endif
    }

    private void Initialize()
    {
        LoadComponents();
        m_label.fontSize = m_cellSize;
        m_label.characterSpacing = HORIZONTAL_SPACING;
        m_label.lineSpacing = VERTICAL_SPACING;
        m_label.alignment = TextAlignmentOptions.MidlineGeoAligned;
        m_sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        m_sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        int gridCellSize = (m_cellSize / 2) + 15;
        m_grid.cellSize = Vector2.one * gridCellSize;
        m_grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        m_grid.constraintCount = 5;
        m_grid.padding.top = 64;
        GenerateGrid();
    }

    private void OnValidate()
    {
        LoadComponents();
        m_label.fontSize = m_cellSize;
        int gridCellSize = (m_cellSize / 2) + 15;
        m_grid.cellSize = Vector2.one * gridCellSize;
        m_grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        m_grid.constraintCount = 5;
        m_grid.padding.top = 64;
        GenerateGrid();
    }

    private void Reset()
    {
        Initialize();
    }
}
