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
    [SerializeField] private NarratorSystem m_narratorSystem;

    [Space]

    [SerializeField] private ModalBox m_modalBox;
    [SerializeField] private RoomManager m_roomManager;
    private PlayerInputActions m_inputActions;

    public static System.Random RNG = new System.Random();

    public event Action OnGameFinished;

    private void Start()
    {
        int seed = GameData.Seed;
        RNG = new System.Random(seed);

        m_inputActions = new PlayerInputActions();
        m_inputActions.Enable();
        m_reputationHandler.OnReputationChanged += CheckReputationValue;


        #region Initialize Commands

        m_commandSystem.AddCommand(new CommandDefinition<Action>("clear",
            "Clear the command log",
            () =>
            {
                m_commandLogManager.Clear();
                SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
            })
        );

        m_commandSystem.AddCommand(new CommandDefinition<Action>("gettime",
            "Get the current day and week",
            () =>
            {
                m_commandLogManager.AddLog($"day {m_timeManager.GetCurrentDay() + 1} of week {m_timeManager.GetCurrentWeek()}");
                SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
            })
        );

        m_commandSystem.AddCommand(new CommandDefinition<Action>("help",
            "List all commands",
            () =>
            {
                m_commandLogManager.AddLog("Commands: ", GameManager.ORANGE);
                foreach (var element in m_commandSystem.GetCommandsList())
                {
                    var text = $"- {element.identifier}{element.parameters}{(string.IsNullOrEmpty(element.description) ? "" : $": {element.description}")}";
                    m_commandLogManager.AddLog(text, GameManager.ORANGE, format: false);
                }
                SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
            })
        );

#if UNITY_EDITOR

        m_commandSystem.AddCommand(new CommandDefinition<Action<int>>("addmeds",
            "[EDITOR ONLY] Add <amount> of meds in the stock",
            (int amount) =>
            {
                var trueAmount = Mathf.Min(ResourceHandler.MAX_RESOURCE - m_resourceHandler.Meds, amount);
                m_resourceHandler.AddMeds(amount);
                m_commandLogManager.AddLog($"Added {trueAmount} meds", GameManager.ORANGE);
                SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
            })
        );

        m_commandSystem.AddCommand(new CommandDefinition<Action<int>>("addrations",
            "[EDITOR ONLY] Add <amount> of rations in the stock",
            (int amount) =>
            {
                var trueAmount = Mathf.Min(ResourceHandler.MAX_RESOURCE - m_resourceHandler.Rations, amount);
                m_resourceHandler.AddRations(amount);
                m_commandLogManager.AddLog($"Added {trueAmount} rations", GameManager.ORANGE);
                SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
            })
        );

        m_commandSystem.AddCommand(new CommandDefinition<Action<int>>("addscraps",
            "[EDITOR ONLY] Add <amount> of scraps in the stock",
            (int amount) =>
            {
                var trueAmount = Mathf.Min(ResourceHandler.MAX_RESOURCE - m_resourceHandler.Scraps, amount);
                m_resourceHandler.AddScraps(amount);
                m_commandLogManager.AddLog($"Added {trueAmount} scraps", GameManager.ORANGE);
                SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
            })
        );

        m_commandSystem.AddCommand(new CommandDefinition<Action<int, int, int>>("slider",
            "[EDITOR ONLY] Print a text slider on the console log",
            (int min, int max, int amount) =>
            {
                var slider = JUtils.GenerateTextSlider(amount, min, max, 8);
                m_commandLogManager.AddLog($"{slider}", GameManager.ORANGE);
                SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
            })
        );

        m_commandSystem.AddCommand(new CommandDefinition<Action<bool>>("cursor",
            "[EDITOR ONLY] Hide or display the cursor",
            (bool enabled) =>
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
            })
        );

        m_commandSystem.AddCommand(new CommandDefinition<Action<int>>("addreputation",
            "[EDITOR ONLY] Add reputation (only takes positive values)",
            (int value) =>
            {
                if (value < 0)
                {
                    m_commandLogManager.AddLog($"warning: value is negative", GameManager.RED);
                    value = Mathf.Abs(value);
                }

                m_reputationHandler.IncreaseReputation(value);
                m_commandLogManager.AddLog($"reputation: reputation value set to {m_reputationHandler.Reputation}", GameManager.ORANGE);
                SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
            })
        );

        m_commandSystem.AddCommand(new CommandDefinition<Action<int>>("removereputation",
            "[EDITOR ONLY] Remove reputation (only takes positive values)",
            (int value) =>
            {
                if (value < 0)
                {
                    m_commandLogManager.AddLog($"warning: value is negative", GameManager.RED);
                    value = Mathf.Abs(value);
                }

                m_reputationHandler.DecreaseReputation(value);
                m_commandLogManager.AddLog($"reputation: reputation value set to {m_reputationHandler.Reputation}", GameManager.ORANGE);
                SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
            })
        );

        m_commandSystem.AddCommand(new CommandDefinition<Action<int>>("setreputation",
            "[EDITOR ONLY] Set the reputation (only takes a value between -100 and 100)",
            (int value) =>
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
            })
        );

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
        m_roomManager.Initialize();
        m_narratorSystem.Initialize();
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


    private void DisableAllComponents()
    {
        foreach (Behaviour component in gameObject.GetComponents<Behaviour>())
        {
            if (component is SoundManager || component is AudioSource)
            {
                return;
            }
            component.enabled = false;
        }
    }

    private void CheckReputationValue(int reputation)
    {
        if (reputation == ReputationHandler.MIN_REPUTATION)
        {
            OnGameFinished?.Invoke();
        }
    }

    private void GameFinished()
    {
        OnGameFinished?.Invoke();
        DisableAllComponents();
    }


 

    public TimeManager GetTimeManager() => m_timeManager;
    public CommandSystem GetCommands() => m_commandSystem;
    public CommandLogManager GetCommandLog() => m_commandLogManager;
    public PlayerInputActions GetInputActions() => m_inputActions;
    public ResourceHandler GetResourceHandler() => m_resourceHandler;
    public ReputationHandler GetReputationHandler() => m_reputationHandler;
    public ExplorationSystem GetExplorer() => m_explorationSystem;
    public RoomManager GetRoomManager() => m_roomManager;
    public VillagerManager GetVillagerManager() => m_villagerManager;
    public NarratorSystem GetNarrator() => m_narratorSystem;
    public ModalBox GetModal() => m_modalBox;
}
