using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class EndSreen : MonoBehaviour
{

    [SerializeField] Button m_restartButton;
    [SerializeField] Button m_quitButton;
    [SerializeField] TextMeshProUGUI m_daysLabel;

    GameManager m_gm;
    TimeManager m_timeManager;


    private void Awake()
    {
        m_gm = FindObjectOfType<GameManager>();
        m_gm.OnGameFinished += ShowGameOverPanel;
        m_timeManager = FindObjectOfType<TimeManager>();
        gameObject.SetActive(false);
    }



    void ShowGameOverPanel()
    {
        Debug.Log("CACA");
        gameObject.SetActive(true);
        StartCoroutine(SelectButtonTimer(1, m_restartButton));
        DisplayRecap();
    }



    IEnumerator SelectButtonTimer(float time, Button button)
    {
        yield return new WaitForSeconds(time);
        SelectButton(button);
    }


    void DisplayRecap()
    {
        int dayNum = CalculateTotalDays(m_timeManager.GetCurrentDay(), m_timeManager.GetCurrentWeek());
        float[] realTime = ConvertSecToMinutes(dayNum * TimeManager.DAY_IN_SECONDS);
        DisplayRecapLabel(m_daysLabel, $"- the colony survived {dayNum} days => {realTime[0]}:{realTime[1]}");

    }

    float[] ConvertSecToMinutes(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int remainingSeconds = Mathf.FloorToInt(seconds % 60f);
        float[] result = new float[2];
        result[0] = minutes;
        result[1] = remainingSeconds;
        return result;

    }

    int CalculateTotalDays(int startDay, int weeks)
    {
        int daysFromWeeks = weeks * 7;
        int totalDays = startDay + daysFromWeeks;
        return totalDays;
    }


    public void RestartButtonPressed()
    {
        SceneManager.LoadScene(1);
        SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
    }

    public void QuitButtonPressed()
    {
        SceneManager.LoadScene(0);
        SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
    }

    void DisplayRecapLabel(TextMeshProUGUI label, string text)
    {
        label.text = text;
    }

    void SelectButton(Button button)
    {
        button.Select();
    }


}
