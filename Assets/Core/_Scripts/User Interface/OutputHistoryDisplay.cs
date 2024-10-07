using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class OutputHistoryDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text m_historyTextField;

    private CommandLogManager m_commandLog;

    private void Start()
    {
        m_commandLog = GameManager.Instance.GetCommandLog();
        m_commandLog.RegisterOnHistoryChanged(OnHistoryChanged);
        m_historyTextField.text = "";
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
        StringBuilder sb = new StringBuilder();
        var history = m_commandLog.GetCommandHistory();
        foreach ( var item in history )
        {
            sb.AppendLine( item.ToString() );
        }
        m_historyTextField.text = sb.ToString();
    }
}
