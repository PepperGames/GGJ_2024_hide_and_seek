using UnityEngine;
using TMPro;
using Photon.Pun;

public class GameTimerMaster : MonoBehaviourPunCallbacks
{
    public TeamStatusUI teamStatusUI;
    public TMP_Text timerText; // Текстовое поле для отображения таймера
    public float remainingTime = 150f; // 2 минуты и 30 секунд
    
    private bool timerIsRunning = false;

    private void Start()
    {
        StartTimer();
    }

    private void Update()
    {
        if (timerIsRunning)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                UpdateTimerDisplay();
            }
            else
            {
                remainingTime = 0;
                timerIsRunning = false;
                OnTimerEnd();
            }
        }
    }

    public void StartTimer()
    {
        timerIsRunning = true;
    }

    private void UpdateTimerDisplay()
    {
        if (remainingTime <= 0)
        {
            timerText.text = "00:00";
            return;
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void OnTimerEnd()
    {
        teamStatusUI.timeIsOut = true;
        teamStatusUI.UpdateTeamCounts();
    }
}