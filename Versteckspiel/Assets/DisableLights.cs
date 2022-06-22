using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableLights : MonoBehaviour
{
    [SerializeField] GameObject[] ObjectsToHide;

    public void OnPreCull()
    {
        foreach (GameObject obj in ObjectsToHide)
        {
            // Licht ausschalten, bevor die Kamera rendert.
            obj.SetActive(false);
        }
    }

    public void OnPostRender()
    {
        foreach (GameObject obj in ObjectsToHide)
        {
            // Licht für andere Kameras wieder anschalten.
            obj.SetActive(true);
        }
    }
}
