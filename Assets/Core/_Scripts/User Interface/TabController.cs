using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TabController : MonoBehaviour
{
    [SerializeField] private List<TabOption> m_options;

    [Space]

    [SerializeField] private UnityEvent m_onTabSelected;

    private TabOption m_selectedTab;
    private PlayerInputActions m_inputActions;

    private void Start()
    {
        m_inputActions = GameManager.Instance.GetInputActions();
        var input = m_inputActions.Gameplay;
        input.SwitchTab.performed += SwitchRequested;

        if (m_options.Count > 0)
        {
            Select(0);
        }
    }

    private void OnDestroy()
    {
        var input = m_inputActions.Gameplay;
        input.SwitchTab.performed -= SwitchRequested;
    }

    private void SwitchRequested(InputAction.CallbackContext ctx)
    {
        int index = Mathf.RoundToInt(ctx.ReadValue<float>()) - 1;
        Select(index);
    }

    public void Select(int index)
    {
        Debug.Assert(m_options.Count > 0, "No options linked to the controller.", this);
        Debug.Assert(index >= 0 && index < m_options.Count, "Option index out of range.", this);

        if (m_options[index] == m_selectedTab)
        {
            return;
        }

        for (int i = 0; i < m_options.Count; i++)
        {
            bool isSelected = index == i;
            TabOption option = m_options[i];
            option.Toggle(isSelected);
            if (isSelected) m_selectedTab = option;
        }

        m_onTabSelected?.Invoke();
    }
}
