using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using static UnityEngine.Rendering.DebugUI;

public class AsciiSlider : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_textMeshPro;
    [SerializeField] protected int m_value = 5;
    [SerializeField] protected int m_minValue = 0;
    [SerializeField] protected int m_maxValue = 100;

    protected void Start()
    {

       m_textMeshPro = gameObject.GetComponent<TextMeshProUGUI>();
       m_textMeshPro.color = GameManager.GREEN;
       if (!m_textMeshPro){
            Debug.LogError("no text mesh pro component"+this.gameObject);
       }
       m_value = m_maxValue;
       SetSlider(m_value);
    }

    protected void SetSlider(int value)
    {
        m_textMeshPro.text = "";
        m_textMeshPro.text += "[";
        for (int i = 0; i < m_maxValue; i++)
        {
            if( i < m_value){
                m_textMeshPro.text += "■";
            }
            else{
                m_textMeshPro.text += "□";
            }
        }
        m_textMeshPro.text += "]";
    }
}
