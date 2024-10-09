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

    private TMP_Text m_label;

    private void Start()
    {
        Initialize();
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

            ContentSizeFitter contentSize;
            if (TryGetComponent(out contentSize) is false)
            {
                contentSize = gameObject.AddComponent<ContentSizeFitter>();
            }
            contentSize.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSize.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }

    private void Initialize()
    {
        LoadComponents();
        m_label.fontSize = m_cellSize;
        m_label.characterSpacing = HORIZONTAL_SPACING;
        m_label.lineSpacing = VERTICAL_SPACING;
        m_label.alignment = TextAlignmentOptions.MidlineGeoAligned;
        GenerateGrid();
    }

    private void OnValidate()
    {
        LoadComponents();
        m_label.fontSize = m_cellSize;
        GenerateGrid();
    }

    private void Reset()
    {
        Initialize();
    }
}
