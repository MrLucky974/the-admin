using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseCommandLineInputFieldController : MonoBehaviour
{
    private const int MAX_COMMAND_HISTORY = 20;

    [SerializeField] private TMP_InputField m_inputField;
    private CommandSystem m_commandSystem;
    private PlayerInputActions.GameplayActions m_actions;

    private List<string> m_commands = new List<string>();
    private int m_commandIndex = -1;

    public void Start()
    {
        m_actions = GameManager.Instance.GetInputActions().Gameplay;
        m_commandSystem = GameManager.Instance.GetCommands();

        // Grab focus of the input field
        m_inputField.ActivateInputField();

        m_inputField.onSubmit.AddListener(OnCommandSubmit);
        m_inputField.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnDestroy()
    {
        m_inputField.onSubmit.RemoveAllListeners();
    }

    private void Update()
    {
        if (m_actions.PreviousCommand.WasPressedThisFrame())
        {
            if (m_commands.Count > 0)
            {
                m_commandIndex = (m_commandIndex + 1) % m_commands.Count;
                var command = m_commands[^((m_commandIndex % m_commands.Count) + 1)];
                m_inputField.SetTextWithoutNotify(command);
                m_inputField.MoveToEndOfLine(ctrl: false, shift: false);
            }
            m_inputField.ActivateInputField();
        }
    }

    void OnCommandSubmit(string input)
    {
        m_commandSystem.ParseCommand(input);
        m_inputField.SetTextWithoutNotify(string.Empty);
        m_inputField.ActivateInputField();
        m_commands.Add(input);
        if (m_commands.Count > MAX_COMMAND_HISTORY)
        {
            m_commands.RemoveAt(0);
        }
        m_commandIndex = -1;
    }

    void OnValueChanged(string input)
    {
        var rng = GameManager.RNG;
        SoundManager.SetPitch(rng.NextFloat(0.8f, 1.2f));
        SoundManager.PlaySound(SoundType.CHARACTER_TYPE);
        if (string.IsNullOrEmpty(input))
        {
            m_commandIndex = -1;
        }
    }
}
