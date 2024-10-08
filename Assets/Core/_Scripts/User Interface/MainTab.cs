using System;
using UnityEngine;

public class MainTab : MonoBehaviour
{
    [SerializeField] private PanelController m_panelController;

    private void Start()
    {
        var commandSystem = GameManager.Instance.GetCommands();

        commandSystem.AddCommand(new CommandDefinition<Action>("stock", () =>
        {
            m_panelController.ShowPanel((int)MainPanelIndex.STOCK);
        }));
    }

    public PanelController GetPanelController() => m_panelController;
}
