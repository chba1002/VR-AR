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
        //Sobald der Strahl Motte berührt, kann man schießen. Check
        //Beide Strahlen müssen Motte treffen, damit sie stirbt. Sonst passiert nichts.
        //Axis1D.PrimaryIndexTrigger und Axis1D.SecondaryIndexTrigger zum Schießen. Check
        //Motten brauchen Sterbeanzeige
        //Motten gewinnen, sobald Zeit abläuft, FM gewinnt, sobald innerhalb der Zeit alle Motten gefroffen wurden, dann jeweiliges Menü anzeigen

        print("Motte getroffen!");
        // Hier Dissolve Shader einfügen @Chi
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
