using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuBtn : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    // Start is called before the first frame update
    [SerializeField] private Button m_button;
    [SerializeField] private TextMeshProUGUI m_label;
    string m_baseText; 

    public void OnSelect(BaseEventData eventData)
    {
        Selected();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        DeSelected();
    }

    void Awake()
    {
        m_button = GetComponent<Button>();
        m_label = GetComponentInChildren<TextMeshProUGUI>();
        m_baseText = m_label.text;
    }


    void Selected()
    {
        String text = m_label.text;
        m_label.text = "";
        m_label.text = "> " + text;
    }

    void DeSelected()
    {
        m_label.text = m_baseText;
    }
}
