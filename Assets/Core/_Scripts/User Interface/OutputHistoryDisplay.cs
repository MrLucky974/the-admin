using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutputHistoryDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text m_historyTextField;
    [SerializeField] private ScrollRect m_scrollRect;

    private CommandLogManager m_commandLog;
    PlayerInputActions.GameplayActions m_actions;

    private void Start()
    {
        m_actions = GameManager.Instance.GetInputActions().Gameplay;

        m_commandLog = GameManager.Instance.GetCommandLog();
        m_commandLog.RegisterOnHistoryChanged(OnHistoryChanged);

        //m_historyTextField.text = "";
        OnHistoryChanged();
    }

    private void Update()
    {
        var deltaTime = Time.deltaTime;
        var currentValue = m_scrollRect.verticalNormalizedPosition;
        var input = m_actions.Scroll.ReadValue<float>();
        currentValue -= input * m_scrollRect.scrollSensitivity * deltaTime;
        m_scrollRect.verticalNormalizedPosition = Mathf.Clamp01(currentValue);
    }

    private void OnDestroy()
    {
        if (m_commandLog)
        {
            m_commandLog.UnregisterOnHistoryChanged(OnHistoryChanged);
        }
    }

    private void OnHistoryChanged()
    {
        var history = m_commandLog.GetCommandHistory();

        StringBuilder sb = new StringBuilder();
        foreach (var item in history)
        {
            sb.AppendLine(item);
        }

        m_historyTextField.SetText(sb.ToString());
        m_scrollRect.verticalNormalizedPosition = 0f;
    }
}
