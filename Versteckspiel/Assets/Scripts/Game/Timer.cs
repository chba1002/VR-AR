using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public float timeValue = 300;
    public TextMeshProUGUI timerText;
    public GameObject EndscreenBat;
    public GameObject EndscreenMoths;
    public GameObject TimerScreen;
    public GameObject Blocking;
    public GameObject DeathScreen;
    public AudioSource WinningSound;

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
            EndscreenBat.SetActive(false);
            EndscreenMoths.SetActive(true);
            TimerScreen.SetActive(false);
            //Blocking.SetActive(false);
            DeathScreen.SetActive(true);
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
    }
}
