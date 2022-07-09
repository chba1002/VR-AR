using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ShootAtMoth : MonoBehaviour
{
    public int prey; // Anzahl der verbleibenden Motten
    public GameObject Endscreen;

    public void ShootMoth()
    {
        //Sobald der Strahl Motte berührt, kann man schießen. Check
        //Beide Strahlen müssen Motte treffen, damit sie stirbt. Sonst passiert nichts.
        //Axis1D.PrimaryIndexTrigger und Axis1D.SecondaryIndexTrigger zum Schießen. Check
        //Motten brauchen Sterbeanzeige

        print("Motte getroffen!");
        Destroy(this.gameObject);


        /*// Sobald alle Motten abgeschossen wurden, taucht der Endbildschirm auf.
        if (prey = 0)
        {
            Endscreen.SetActive(true);
        }*/
    }
}
