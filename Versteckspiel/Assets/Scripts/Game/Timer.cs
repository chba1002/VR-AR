using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public float timeValue = 300;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI timerTextBatWins;
    public GameObject EndscreenBat;
    public GameObject EndscreenMoths;
    public GameObject TimerScreen;
    public GameObject DeathScreen;
    public AudioSource WinningSound;


    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DisplayTime(timeValue);

        if (timeValue > 0)
        {
            timeValue -= Time.deltaTime;
        }
        else
        {
            timeValue = 0;
        }
        if (timeValue == 0)
        {
            WinningSound.Play();
            TimerScreen.SetActive(false);
            EndscreenBat.SetActive(false);
            EndscreenMoths.SetActive(true);
        }

        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            WinningSound.Play();
            TimerScreen.SetActive(false); 
            EndscreenBat.SetActive(true);
            EndscreenMoths.SetActive(false);
            return;
        }
    }

    public void DisplayTime(float TimeToDisplay)
    {
        if(TimeToDisplay <0)
        {
            TimeToDisplay = 0;
        }

        float minutes = Mathf.FloorToInt(TimeToDisplay / 60);
        float seconds = Mathf.FloorToInt(TimeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerTextBatWins.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void BackToLobby()
    {
        //Am Ende kann man wieder in die Lobby laden
    }
}
