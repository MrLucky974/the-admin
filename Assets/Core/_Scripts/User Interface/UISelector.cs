using UnityEngine;
using UnityEngine.EventSystems;

public class UISelector : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private GameObject m_lastSelected;

    private void Start()
    {
        m_lastSelected = EventSystem.current.currentSelectedGameObject;
    }

    void Update()
    {
        // If there is no selected game object, reselect the last known selected UI element
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(m_lastSelected);
        }
        else
        {
            // Update the last known selected UI element
            m_lastSelected = EventSystem.current.currentSelectedGameObject;
        }
    }
}
