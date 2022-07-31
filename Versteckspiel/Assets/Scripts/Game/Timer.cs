using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float timeValue = 300;
    public float possibleTimeReductionInSeconds = 5;
    public bool timeReductionWasExecuted = false;

    [Header("Time Text Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI timerTextBatWins;
    public TextMeshProUGUI timeReductionTime;
    public TextMeshProUGUI timeReductionText;

    [Header("Screens")]
    public GameObject EndscreenBat;
    public GameObject EndscreenMoths;
    public GameObject TimerScreen;
    public GameObject DeathScreen;

    [Header("Sounds")]
    public AudioSource WinningSound;


    private void Awake()
    {
        timeReductionTime.gameObject.SetActive(false);
        timeReductionText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(timeValue == 0)
        {
            return;
        }

        /*
         * ToDo: Hier kann bei bedarf angepast werden, wie schnell die Dauer der Zeitverkürzung ansteigt. 
         */ 
        if (!timeReductionWasExecuted)
        {
            if(timeValue<=35 || possibleTimeReductionInSeconds >= 29)
            {
                possibleTimeReductionInSeconds = 30;
            }
            else
            {
                possibleTimeReductionInSeconds += (Time.deltaTime / timeValue) * 10;
               // Debug.Log(possibleTimeReductionInSeconds);
            }
        }

        timeValue = (timeValue > 0) ? timeValue - Time.deltaTime : 0;

        if (timeValue <= 0 
            || (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)))
        {
            timeValue = 0;
            WinningSound.Play();
            TimerScreen.SetActive(false);
            EndscreenBat.SetActive(false);
            EndscreenMoths.SetActive(true);
            return;
        }

        DisplayTime(timeValue);
        DisplayPossibleTimeReduction(possibleTimeReductionInSeconds);
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

    public void DisplayPossibleTimeReduction(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeReductionTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ExecutePossibleTimeReduction()
    {
        timeValue = timeValue - possibleTimeReductionInSeconds > 0
            ? timeValue - possibleTimeReductionInSeconds
            : 0;
        possibleTimeReductionInSeconds = 0;
        timeReductionWasExecuted = true;
    }

    public void ShowTimeReductionInfo()
    {
        timeReductionTime.gameObject.SetActive(true);
        timeReductionText.gameObject.SetActive(true);
    }

    public void BackToLobby()
    {
        //Am Ende kann man wieder in die Lobby laden
    }
}
