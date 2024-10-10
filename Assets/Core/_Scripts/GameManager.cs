using LuckiusDev.Utils;
using System;
using System.Text;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
[DisallowMultipleComponent]
public class GameManager : Singleton<GameManager>
{
    public static readonly Color RED = new Color(1f, 0f, 0.2256851f);
    public static readonly Color ORANGE = new Color(1f, 0.7206769f, 0f);
    public static readonly Color GREEN = new Color(0.2617465f, 1f, 0f);

    [SerializeField] private TimeManager m_timeManager;
    [SerializeField] private CommandSystem m_commandSystem;
    [SerializeField] private CommandLogManager m_commandLogManager;
    [SerializeField] private ResourceHandler m_resourceHandler;
    [SerializeField] private ExplorationSystem m_explorationSystem;
    [SerializeField] private ModalBox m_modalBox;
    [SerializeField] private VillagerManager m_villagerManager;
    [SerializeField] private RoomManager m_roomManager;
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
        m_commandSystem.AddCommand(new CommandDefinition<Action<int>>("gettime", (int a) =>
        {
            m_commandLogManager.AddLog($"day {m_timeManager.GetCurrentDay() + 1} of week {m_timeManager.GetCurrentWeek()}");
        }));

        m_commandSystem.AddCommand(new CommandDefinition<Action>("help", () =>
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Commands: ");
            foreach (var element in m_commandSystem.GetCommandHelp())
            {
                sb.AppendLine($"- {element.identifier}: {element.description}");
            }
            m_commandLogManager.AddLog(sb.ToString(), GameManager.ORANGE);
        }));
        m_commandSystem.AddCommand(new CommandDefinition<Action<String, String>>("repair", (String villagerId, String roomId) =>
        {
            m_villagerManager.SendVillagerRepairRoom(villagerId, roomId);
        }));

#if UNITY_EDITOR

        m_commandSystem.AddCommand(new CommandDefinition<Action<int>>("addmeds", (int amount) =>
        {
            m_resourceHandler.AddMeds(amount);
            m_commandLogManager.AddLog($"Added {amount} meds", GameManager.ORANGE);
        }));

        m_commandSystem.AddCommand(new CommandDefinition<Action<int>>("addrations", (int amount) =>
        {
            m_resourceHandler.AddRations(amount);
            m_commandLogManager.AddLog($"Added {amount} rations", GameManager.ORANGE);
        }));

        m_commandSystem.AddCommand(new CommandDefinition<Action<int>>("addscraps", (int amount) =>
        {
            m_resourceHandler.AddScraps(amount);
            m_commandLogManager.AddLog($"Added {amount} scraps", GameManager.ORANGE);
        }));

        m_commandSystem.AddCommand(new CommandDefinition<Action<int, int, int>>("slider", (int min, int max, int amount) =>
        {
            var slider = JUtils.GenerateTextSlider(amount, min, max, 8);
            m_commandLogManager.AddLog($"{slider}", GameManager.ORANGE);
        }));

        m_commandSystem.AddCommand(new CommandDefinition<Action<String>>("upgrade", (String roomId) =>
        {
            m_roomManager.UpgradeRoom(roomId);
        }));

 

#endif
        #endregion

        // Lock player from using the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize components
        m_timeManager.Initialize();
        m_resourceHandler.Initialize();
        m_explorationSystem.Initialize();
        m_villagerManager.Initialize();
        m_roomManager.Initialize();
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
    public ExplorationSystem GetExplorer() => m_explorationSystem;
    public RoomManager GetRoomManager() => m_roomManager;
    public ModalBox GetModal() => m_modalBox;
    public VillagerManager GetVillagerManager() => m_villagerManager;
}
