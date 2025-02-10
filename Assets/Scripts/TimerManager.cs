using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    //Components 
    public Image timerCircle;
    public TextMeshProUGUI timerText;

    //Gameplay variables
    public float maxTime = 30;
    public bool isTimerRunning = false;

    [HideInInspector] public float timeLeft;

    private static TimerManager instance;
    public static TimerManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType(typeof(TimerManager)) as TimerManager;

            return instance;
        }
        set
        {
            instance = value;
        }
    }

    private void Start()
    {
        timeLeft = maxTime;
    }

    void Update()
    {
        TimerAction();
    }

    //Runs the timer and adjusts the timer image accordingly, when it goes to 0, game over event is called.
    private void TimerAction()
    {
        if (isTimerRunning)
        {
            timeLeft = Mathf.Clamp(timeLeft - Time.deltaTime, 0, maxTime);
            timerCircle.fillAmount = Mathf.Lerp(0, 1, timeLeft / maxTime);
            UpdateTimerUI();

            if (timeLeft <= 0)
            {
                GameStateManager.Instance.EndGame();
                isTimerRunning = false;
            }
        }
    }

    //Updates the timer Ui text
    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = timeLeft.ToString("F0");
        }
    }

    public void RestartTimer()
    {
        isTimerRunning = true;
        timeLeft = maxTime;
        UpdateTimerUI();
    }

   
}
