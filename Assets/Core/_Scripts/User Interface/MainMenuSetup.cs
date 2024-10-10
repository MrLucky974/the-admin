using System.Collections;
using TMPro;
using UnityEngine;

public class MainMenuSetup : MonoBehaviour
{
    private const string PREFIX_FORMAT = "> {0}:";
    private const string LOADING_BAR_FORMAT = "Loading... {0} [{1}]";
    private static readonly char[] LOADING_BAR_CHARACTERS =
    {
        '|',
        '/',
        '—',
        '\\',
    };

    private enum State
    {
        IDENTIFIER,
        SEED,
    }

    [SerializeField] private TMP_Text m_logLabel;
    [SerializeField] private TMP_Text m_prefixLabel;
    [SerializeField] private TMP_InputField m_inputField;

    private State m_state;
    private float m_loadingBarValue;

    private void Start()
    {
        m_logLabel.text = "";
        m_logLabel.text += "The Administrator - v0.1.0a";
        m_logLabel.text += "\nEnter username...";

        m_state = State.IDENTIFIER;
        m_inputField.SetTextWithoutNotify(string.Empty);
        m_prefixLabel.SetText(string.Format(PREFIX_FORMAT, "username"));

        m_inputField.ActivateInputField();
        m_inputField.onSubmit.AddListener(OnTextSubmit);
        m_inputField.onValueChanged.AddListener(OnValueChanged);
    }

    private IEnumerator LoadingScreen()
    {
        string text = "";
        int loadingCharacterIndex = 0;
        var currentLoadingValue = 0f;

        while (currentLoadingValue < m_loadingBarValue)
        {
            loadingCharacterIndex = (int)(Time.time * 4) % LOADING_BAR_CHARACTERS.Length;

            char loadingCharacter = LOADING_BAR_CHARACTERS[loadingCharacterIndex];
            string slider = JUtils.GenerateTextSlider(currentLoadingValue / m_loadingBarValue, 10);
            text = string.Format(LOADING_BAR_FORMAT, slider, loadingCharacter);

            m_logLabel.SetText(text);

            yield return null;
            currentLoadingValue += Time.deltaTime;
        }

        // TODO : Load gameplay scene
    }

    private void OnTextSubmit(string input)
    {
        m_inputField.SetTextWithoutNotify(string.Empty);
        m_inputField.ActivateInputField();

        switch (m_state)
        {
            case State.IDENTIFIER:
                if (string.IsNullOrEmpty(input))
                {
                    SoundManager.PlaySound(SoundType.ERROR);
                    return;
                }

                SoundManager.PlaySound(SoundType.ACTION_CONFIRM);

                GameData.SetIdentifier(input);

                m_logLabel.text += $"\nuser: {input}";
                m_logLabel.text += "\nEnter password (leave empty for random seed)...";

                m_state = State.SEED;
                m_prefixLabel.SetText(string.Format(PREFIX_FORMAT, "password (seed)"));
                break;
            case State.SEED:
                SoundManager.PlaySound(SoundType.ACTION_CONFIRM);

                int seed = 0;
                if (string.IsNullOrEmpty(input))
                {
                    var key = JRandom.GenerateRandomString(m_inputField.characterLimit);
                    Debug.Log($"Generated key: {key}");
                    seed = JUtils.StringToSeedFNV1a(key);
                }
                else
                {
                    seed = JUtils.StringToSeedFNV1a(input);
                }

                Debug.Log(seed);
                GameData.SetSeed(seed);

                SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
                m_loadingBarValue = Random.Range(1f, 2f);
                m_prefixLabel.gameObject.SetActive(false);
                m_inputField.gameObject.SetActive(false);
                StartCoroutine(nameof(LoadingScreen));
                break;
        }
    }

    private void OnValueChanged(string input)
    {
        var rng = GameManager.RNG;
        SoundManager.SetPitch(rng.NextFloat(0.8f, 1.2f));
        SoundManager.PlaySound(SoundType.CHARACTER_TYPE);
    }
}
