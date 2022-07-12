using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class ShootAtMoth : MonoBehaviour
{
    public GameObject EndscreenBat;
    public GameObject EndscreenMoths;
    GameObject[] Prey;  // Anzahl der verbleibenden Motten

    public void ShootMoth()
    {
        //Sobald der Strahl Motte ber�hrt, kann man schie�en. Check
        //Beide Strahlen m�ssen Motte treffen, damit sie stirbt. Sonst passiert nichts.
        //Axis1D.PrimaryIndexTrigger und Axis1D.SecondaryIndexTrigger zum Schie�en. Check
        //Motten brauchen Sterbeanzeige
        //Motten gewinnen, sobald Zeit abl�uft, FM gewinnt, sobald innerhalb der Zeit alle Motten gefroffen wurden, dann jeweiliges Men� anzeigen

        print("Motte getroffen!");
        // Hier Dissolve Shader einf�gen @Chi
        Destroy(this.gameObject); //oder deaktivieren
    }


    public void PlayAgain()
    {
        SceneManager.LoadScene("Moth-LobbyScene");
    }

    public void Update()
    {
        Prey = GameObject.FindGameObjectsWithTag("Moth");

        // Sobald alle Motten abgeschossen wurden , taucht der Endbildschirm auf.
        if (Prey.Length == 0)
        {
            print("Alle Motten getroffen.");
            EndscreenBat.SetActive(true);
            EndscreenMoths.SetActive(false);
        }
    }
}
