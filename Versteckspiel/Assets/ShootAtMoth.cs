using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class ShootAtMoth : MonoBehaviour
{
    public float playTime;
    public GameObject EndscreenBat;
    public GameObject EndscreenMoths;

    public int Prey; // Anzahl der verbleibenden Motten

    public void Start()
    {
        Prey = 5;
        playTime = Time.deltaTime;
    }
    public void ShootMoth()
    {
        //Sobald der Strahl Motte ber�hrt, kann man schie�en. Check
        //Beide Strahlen m�ssen Motte treffen, damit sie stirbt. Sonst passiert nichts.
        //Axis1D.PrimaryIndexTrigger und Axis1D.SecondaryIndexTrigger zum Schie�en. Check
        //Motten brauchen Sterbeanzeige
        //Motten gewinnen, sobald Zeit abl�uft, FM gewinnt, sobald innerhalb der Zeit alle Motten gefroffen wurden, dann jeweiliges Men� anzeigen

        print("Motte getroffen!");
        Destroy(this.gameObject);
        Prey = Prey - 1;
    }


    public void PlayAgain()
    {
        SceneManager.LoadScene("Moth-LobbyScene");
    }

    public void Update()
    {
        
        // Sobald alle Motten abgeschossen wurden bzw. die Zeit abgelaufen ist, taucht der Endbildschirm auf.
        if (Prey == 0)
        {
            EndscreenBat.SetActive(true);
            EndscreenMoths.SetActive(false);
        }

        if (playTime == 0)
        {
            EndscreenBat.SetActive(false);
            EndscreenMoths.SetActive(true);
        }
    }
}
