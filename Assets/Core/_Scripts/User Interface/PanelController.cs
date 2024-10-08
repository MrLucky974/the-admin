using UnityEngine;

public class PanelController : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] m_panels;

    private void Start()
    {
        ShowPanel(0);
    }

    public void ShowPanel(int index)
    {
        HideAllPanels(); // Hide all before showing the selected panel
        if (index >= 0 && index < m_panels.Length)
        {
            m_panels[index].alpha = 1f; // Show the selected panel
        }
    }

    private void HideAllPanels()
    {
        foreach (var panel in m_panels)
        {
            panel.alpha = 0f; // Hide each panel
        }
    }
}
