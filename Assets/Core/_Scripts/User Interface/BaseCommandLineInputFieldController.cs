using TMPro;
using UnityEngine;

public class BaseCommandLineInputFieldController : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_inputField;
    private CommandSystem m_commandSystem;

    public void Start()
    {
        m_commandSystem = GameManager.Instance.GetCommands();

        // Grab focus of the input field
        m_inputField.ActivateInputField();

        m_inputField.onSubmit.AddListener(OnCommandSubmit);
    }

    private void OnDestroy()
    {
        m_inputField.onSubmit.RemoveAllListeners();
    }

    void OnCommandSubmit(string input)
    {
        m_commandSystem.ParseCommand(input);
        m_inputField.text = "";
        m_inputField.ActivateInputField();
    }
}
