using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ModalBox : MonoBehaviour
{
    [SerializeField] private RectTransform m_modalBoxTransform;
    private GameObject m_modalBox;

    [Space]

    [SerializeField] private TMP_Text m_headerLabel;
    [SerializeField] private TMP_Text m_bodyLabel;
    [SerializeField] private RectTransform m_yesButtonTransform;
    private GameObject m_yesButton;
    [SerializeField] private RectTransform m_noButtonTransform;
    private GameObject m_noButton;

    [Space]

    [Header("Open Animation")]
    [SerializeField] private AnimationCurve m_xAxisOpenAnimationCurve;
    [SerializeField] private AnimationCurve m_yAxisOpenAnimationCurve;
    [SerializeField] private float m_openAnimationDuration = 1f;

    [Header("Close Animation")]
    [SerializeField] private AnimationCurve m_xAxisCloseAnimationCurve;
    [SerializeField] private AnimationCurve m_yAxisCloseAnimationCurve;
    [SerializeField] private float m_closeAnimationDuration = 1f;

    private Action<ModalBox> m_confirmAction;
    private Action<ModalBox> m_dismissAction;
    private bool m_interactable = false;

    private PlayerInputActions.GameplayActions m_actions;

    private void Start()
    {
        m_headerLabel.text = string.Empty;
        m_bodyLabel.text = string.Empty;
        m_confirmAction = null;
        m_dismissAction = null;

        m_yesButton = m_yesButtonTransform.gameObject;
        m_noButton = m_noButtonTransform.gameObject;
        m_noButton.SetActive(false);

        m_modalBox = m_modalBoxTransform.gameObject;
        m_modalBox.SetActive(false);

        m_actions = GameManager.Instance.GetInputActions().Gameplay;
        m_actions.Accept.performed += OnAccept;
        m_actions.Dismiss.performed += OnDismiss;
    }

    private void OnDestroy()
    {
        m_actions.Accept.performed -= OnAccept;
        m_actions.Dismiss.performed -= OnDismiss;
    }

    private void OnAccept(InputAction.CallbackContext ctx)
    {
        if (!m_interactable) return;
        m_confirmAction?.Invoke(this);
    }

    private void OnDismiss(InputAction.CallbackContext ctx)
    {
        if (!m_interactable) return;
        m_dismissAction?.Invoke(this);
    }

    public ModalBox Init(string header, Action<ModalBox> action)
    {
        var modal = SetConfirmAction(action);
        modal = SetHeader(header);
        return modal;
    }

    public ModalBox SetConfirmAction(Action<ModalBox> action)
    {
        m_confirmAction = action;
        return this;
    }

    public ModalBox SetDismissAction(Action<ModalBox> action)
    {
        m_dismissAction = action;
        m_noButton.SetActive(true);
        return this;
    }

    public ModalBox SetHeader(string header)
    {
        m_headerLabel.text = header;
        return this;
    }

    public ModalBox SetBody(string body)
    {
        m_bodyLabel.text = body;
        return this;
    }

    public void Open()
    {
        if (m_confirmAction == null)
        {
            Debug.Log("Could not open modal box, no confirm action has been set!");
            return;
        }

        if (string.IsNullOrEmpty(m_headerLabel.text))
        {
            Debug.Log("Could not open modal box, no header has been set!");
            return;
        }

        // Show modal box.
        StartCoroutine(nameof(OpenModal));
    }

    public void Close()
    {
        // Hide modal box.
        StartCoroutine(nameof(CloseModal));
    }

    private IEnumerator OpenModal()
    {
        m_modalBox.SetActive(true);

        float elapsedTime = 0f;
        while (elapsedTime < m_openAnimationDuration)
        {
            float delta = elapsedTime / m_openAnimationDuration;
            float xScale = m_xAxisOpenAnimationCurve.Evaluate(delta);
            float yScale = m_yAxisOpenAnimationCurve.Evaluate(delta);

            m_modalBoxTransform.localScale = new Vector3(xScale, yScale, 1f);

            yield return null;
            elapsedTime += Time.deltaTime;
        }

        m_modalBoxTransform.localScale = Vector3.one;
        m_interactable = true;
    }

    private IEnumerator CloseModal()
    {
        m_interactable = false;

        float elapsedTime = 0f;
        while (elapsedTime < m_closeAnimationDuration)
        {
            float delta = elapsedTime / m_closeAnimationDuration;
            float xScale = m_xAxisCloseAnimationCurve.Evaluate(delta);
            float yScale = m_yAxisCloseAnimationCurve.Evaluate(delta);

            m_modalBoxTransform.localScale = new Vector3(xScale, yScale, 1f);

            yield return null;
            elapsedTime += Time.deltaTime;
        }

        m_modalBoxTransform.localScale = new Vector3(0f, 0f, 1f);
        m_modalBox.SetActive(false);

        // Clear everything.
        m_headerLabel.text = string.Empty;
        m_bodyLabel.text = string.Empty;
        m_confirmAction = null;
        m_dismissAction = null;
        m_noButton.SetActive(false);
    }
}
