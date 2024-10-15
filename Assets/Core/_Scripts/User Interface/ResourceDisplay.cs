using System;
using TMPro;
using UnityEngine;

public class ResourceDisplay : MonoBehaviour
{
    private const string RESOURCE_FORMAT = "{0}: [{1}/{2}]";

    [SerializeField] private PageSwitcher m_pages;

    [Space]

    [SerializeField] private TMP_Text m_rationsLabel;
    [SerializeField] private TMP_Text m_medsLabel;
    [SerializeField] private TMP_Text m_scrapsLabel;

    private ResourceHandler m_resourceHandler;

    private void Awake()
    {
        m_resourceHandler = GameManager.Instance.GetResourceHandler();
        m_resourceHandler.OnRationsChanged += UpdateRationLabel;
        m_resourceHandler.OnMedsChanged += UpdateMedsLabel;
        m_resourceHandler.OnScrapsChanged += UpdateScrapsLabel;
    }

    private void Start()
    {
        var commandSystem = GameManager.Instance.GetCommands();
        commandSystem.AddCommand(new CommandDefinition<Action>("stock", "Displays the stock panel on the screen", () =>
        {
            m_pages.Select(StatusPageIndex.STOCK);
            SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
        }));
    }

    private void OnDestroy()
    {
        m_resourceHandler.OnRationsChanged -= UpdateRationLabel;
        m_resourceHandler.OnMedsChanged -= UpdateMedsLabel;
        m_resourceHandler.OnScrapsChanged -= UpdateScrapsLabel;
    }

    private void UpdateScrapsLabel(int amount)
    {
        m_scrapsLabel.text = string.Format(RESOURCE_FORMAT, "Scraps", amount, ResourceHandler.MAX_RESOURCE);
    }

    private void UpdateMedsLabel(int amount)
    {
        m_medsLabel.text = string.Format(RESOURCE_FORMAT, "Meds", amount, ResourceHandler.MAX_RESOURCE);
    }

    private void UpdateRationLabel(int amount)
    {
        m_rationsLabel.text = string.Format(RESOURCE_FORMAT, "Rations", amount, ResourceHandler.MAX_RESOURCE);
    }
}
