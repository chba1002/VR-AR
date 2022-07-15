using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class ShootAtMoth : MonoBehaviour
{
    public GameObject EndscreenBat;
    public GameObject EndscreenMoths;
    GameObject[] Prey;  // Anzahl der verbleibenden Motten
    public AudioSource WinningSound;
    public TMP_Text TimePlayed;
    public Timer mytimerscript;

    public bool aufloesenAktiv;
    public float dissolveAmount;

    public GameObject mothMesh;
    public Material dissolveShaderMaterial;
    private Material material;

    // Start is called before the first frame update
    void Start()
    {
        // Erstelle neues Material auf basis des Prefab Materials, damit jeder Enemy eigenes Material hat.
        var dissolverShader = new Material(dissolveShaderMaterial);

        // Weise neu erstelltes Material dem Moth Mesh zu.
        mothMesh.GetComponent<Renderer>().sharedMaterial = dissolverShader;


        // Speichere das Material in der variable 'material' zwischen.
        material = mothMesh.GetComponent<Renderer>().sharedMaterial;


    }

        public void ShootMoth()
    {
        //Sobald der Strahl Motte ber�hrt, kann man schie�en. Check
        //Beide Strahlen m�ssen Motte treffen, damit sie stirbt. Sonst passiert nichts.
        //Axis1D.PrimaryIndexTrigger und Axis1D.SecondaryIndexTrigger zum Schie�en. Check
        //Motten brauchen Sterbeanzeige
        //Motten gewinnen, sobald Zeit abl�uft, FM gewinnt, sobald innerhalb der Zeit alle Motten gefroffen wurden, dann jeweiliges Men� anzeigen

        print("Motte getroffen!");
        aufloesenAktiv = true;
    }



    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            aufloesenAktiv = true;
        }

        if (aufloesenAktiv == true)
        {
            dissolveAmount = Mathf.Clamp01(dissolveAmount + Time.deltaTime);
            material.SetFloat("_Dissolve", dissolveAmount);
        }
        else
        {
            dissolveAmount = Mathf.Clamp01(dissolveAmount - Time.deltaTime);
            material.SetFloat("_Dissolve", dissolveAmount);
        }

        if (dissolveAmount == 1f)
        {
            Destroy(mothMesh.gameObject); //oder deaktivieren
        }

        string StartTime = mytimerscript.timeValue.ToString("f0");
        TimePlayed.text = StartTime;

        
        Prey = GameObject.FindGameObjectsWithTag("Moth");

        // Sobald alle Motten abgeschossen wurden , taucht der Endbildschirm auf.
        if (Prey.Length == 0)
        {
            print("Alle Motten getroffen.");
            WinningSound.Play();
            EndscreenBat.SetActive(true);
            EndscreenMoths.SetActive(false);
        }
    }
}