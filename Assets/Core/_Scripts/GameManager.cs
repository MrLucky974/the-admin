using LuckiusDev.Utils;
using System;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
[DisallowMultipleComponent]
public class GameManager : Singleton<GameManager>
{
    public static readonly Color RED = new Color(1f, 0f, 0.2256851f);
    public static readonly Color ORANGE = new Color(1f, 0.7206769f, 0f);
    public static readonly Color GREEN = new Color(0.2617465f, 1f, 0f);

    [SerializeField] private TimeManager m_timeManager;

    [Space]

    [SerializeField] private CommandSystem m_commandSystem;
    [SerializeField] private CommandLogManager m_commandLogManager;

    [Space]

    [SerializeField] private ResourceHandler m_resourceHandler;
    [SerializeField] private ReputationHandler m_reputationHandler;

    [Space]

    [SerializeField] private ExplorationSystem m_explorationSystem;
    [SerializeField] private VillagerManager m_villagerManager;

    [Space]

    [SerializeField] private ModalBox m_modalBox;
    private PlayerInputActions m_inputActions;

    public static System.Random RNG = new System.Random();

    private void Start()
    {
        // TODO : Apply seed
        int seed = 0;
        RNG = new System.Random(seed);

        m_inputActions = new PlayerInputActions();
        m_inputActions.Enable();

        #region Initialize Commands
        m_commandSystem.AddCommand(new CommandDefinition<Action>("clear", () =>
        {
            m_commandLogManager.Clear();
            SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
        }));

        m_commandSystem.AddCommand(new CommandDefinition<Action<int>>("gettime", (int a) =>
        {
            m_commandLogManager.AddLog($"day {m_timeManager.GetCurrentDay() + 1} of week {m_timeManager.GetCurrentWeek()}");
            SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
        }));

        m_commandSystem.AddCommand(new CommandDefinition<Action>("help", () =>
        {
            m_commandLogManager.AddLog("Commands: ", GameManager.ORANGE);
            foreach (var element in m_commandSystem.GetCommandHelp())
            {
                var text = $"- {element.identifier}: {element.description}";
                m_commandLogManager.AddLog(text, GameManager.ORANGE, format: false);
            }
            SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
        }));

#if UNITY_EDITOR

        m_commandSystem.AddCommand(new CommandDefinition<Action<int>>("addmeds", (int amount) =>
        {
            m_resourceHandler.AddMeds(amount);
            m_commandLogManager.AddLog($"Added {amount} meds", GameManager.ORANGE);
            SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
        }));

        m_commandSystem.AddCommand(new CommandDefinition<Action<int>>("addrations", (int amount) =>
        {
            m_resourceHandler.AddRations(amount);
            m_commandLogManager.AddLog($"Added {amount} rations", GameManager.ORANGE);
            SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
        }));

        m_commandSystem.AddCommand(new CommandDefinition<Action<int>>("addscraps", (int amount) =>
        {
            m_resourceHandler.AddScraps(amount);
            m_commandLogManager.AddLog($"Added {amount} scraps", GameManager.ORANGE);
            SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
        }));

        m_commandSystem.AddCommand(new CommandDefinition<Action<int, int, int>>("slider", (int min, int max, int amount) =>
        {
            var slider = JUtils.GenerateTextSlider(amount, min, max, 8);
            m_commandLogManager.AddLog($"{slider}", GameManager.ORANGE);
            SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
        }));

        m_commandSystem.AddCommand(new CommandDefinition<Action<bool>>("cursor", (bool enabled) =>
        {
            if (enabled)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            m_commandLogManager.AddLog($"Cursor: {enabled}", GameManager.ORANGE);
            SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
        }));

        m_commandSystem.AddCommand(new CommandDefinition<Action<int>>("addreputation", (int value) =>
        {
            if (value < 0)
            {
                m_commandLogManager.AddLog($"warning: value is negative", GameManager.RED);
                value = Mathf.Abs(value);
            }

            m_reputationHandler.IncreaseReputation(value);
            m_commandLogManager.AddLog($"reputation: reputation value set to {m_reputationHandler.Reputation}", GameManager.ORANGE);
            SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
        }));

        m_commandSystem.AddCommand(new CommandDefinition<Action<int>>("removereputation", (int value) =>
        {
            if (value < 0)
            {
                m_commandLogManager.AddLog($"warning: value is negative", GameManager.RED);
                value = Mathf.Abs(value);
            }

            m_reputationHandler.DecreaseReputation(value);
            m_commandLogManager.AddLog($"reputation: reputation value set to {m_reputationHandler.Reputation}", GameManager.ORANGE);
            SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
        }));

        m_commandSystem.AddCommand(new CommandDefinition<Action<int>>("setreputation", (int value) =>
        {
            if (value < ReputationHandler.MIN_REPUTATION || value > ReputationHandler.MAX_REPUTATION)
            {
                m_commandLogManager.AddLog($"error: value out of range", GameManager.RED);
                SoundManager.PlaySound(SoundType.ERROR);
                return;
            }

            m_reputationHandler.SetReputation(value);
            m_commandLogManager.AddLog($"reputation: reputation value set to {m_reputationHandler.Reputation}", GameManager.ORANGE);
            SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
        }));

#endif
        #endregion

        // Lock player from using the cursor
#if UNITY_EDITOR

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

#else

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

#endif

        // Initialize components
        m_timeManager.Initialize();
        m_resourceHandler.Initialize();
        m_reputationHandler.Initialize();
        m_explorationSystem.Initialize();
        m_villagerManager.Initialize();
    }

    private void Update()
    {
        var deltaTime = Time.deltaTime;
        m_timeManager.UpdateTime(deltaTime);
    }

    private void OnDestroy()
    {
        m_inputActions.Dispose();
    }

    public TimeManager GetTimeManager() => m_timeManager;
    public CommandSystem GetCommands() => m_commandSystem;
    public CommandLogManager GetCommandLog() => m_commandLogManager;
    public PlayerInputActions GetInputActions() => m_inputActions;
    public ResourceHandler GetResourceHandler() => m_resourceHandler;
    public ReputationHandler GetReputationHandler() => m_reputationHandler;
    public ExplorationSystem GetExplorer() => m_explorationSystem;
    public ModalBox GetModal() => m_modalBox;
    public VillagerManager GetVillagerManager() => m_villagerManager;
}
