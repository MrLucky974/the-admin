using TMPro;
using UnityEngine;

public class BaseCommandLineInputFieldController : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_inputField;
    private CommandSystem m_commandSystem;
    private PlayerInputActions.GameplayActions m_actions;

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

    void OnCommandSubmit(string input)
    {
        m_commandSystem.ParseCommand(input);
        m_inputField.text = "";
        m_inputField.ActivateInputField();
    }

    void OnValueChanged(string input)
    {
        var rng = GameManager.RNG;
        SoundManager.SetPitch(rng.NextFloat(0.8f, 1.2f));
        SoundManager.PlaySound(SoundType.CHARACTER_TYPE);
    }
}
