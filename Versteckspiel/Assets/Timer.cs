using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float timeValue = 300;
    public Text timerText;
    public GameObject EndscreenBat;
    public GameObject EndscreenMoths;

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
            EndscreenBat.SetActive(false);
            EndscreenMoths.SetActive(true);
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
