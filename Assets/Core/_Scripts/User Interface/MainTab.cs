using System;
using System.Collections;
using System.Collections.Generic;
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

        // TODO : Merge with villager command
        commandSystem.AddCommand(new CommandDefinition<Action<string>>("checkup", (string identifier) =>
        {
            m_panelController.ShowPanel((int)MainPanelIndex.CHECKUP);
        }));
    }

    public PanelController GetPanelController() => m_panelController;
}
