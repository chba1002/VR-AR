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
    public float verbleibendeAufloesedauerInSekunden = 2;

    public GameObject mothMesh1;
    public GameObject mothMesh2;
    public GameObject mothMesh3;
    public GameObject mothMesh4;
    public GameObject mothMesh5;
    public Material dissolveShaderMaterial;
    private Material material;

    // Start is called before the first frame update
    void Start()
    {
        // Erstelle neues Material auf basis des Prefab Materials, damit jeder Enemy eigenes Material hat.
        var dissolverShader = new Material(dissolveShaderMaterial);

        // Weise neu erstelltes Material dem Moth Mesh zu.
        mothMesh1.GetComponent<Renderer>().sharedMaterial = dissolverShader;
        mothMesh2.GetComponent<Renderer>().sharedMaterial = dissolverShader;
        mothMesh3.GetComponent<Renderer>().sharedMaterial = dissolverShader;
        mothMesh4.GetComponent<Renderer>().sharedMaterial = dissolverShader;
        mothMesh5.GetComponent<Renderer>().sharedMaterial = dissolverShader;

        // Speichere das Material in der variable 'material' zwischen.
        material = mothMesh1.GetComponent<Renderer>().sharedMaterial;
        material = mothMesh2.GetComponent<Renderer>().sharedMaterial;
        material = mothMesh3.GetComponent<Renderer>().sharedMaterial;
        material = mothMesh4.GetComponent<Renderer>().sharedMaterial;
        material = mothMesh5.GetComponent<Renderer>().sharedMaterial;
    }

        public void ShootMoth()
    {
        //Sobald der Strahl Motte berührt, kann man schießen. Check
        //Beide Strahlen müssen Motte treffen, damit sie stirbt. Sonst passiert nichts.
        //Axis1D.PrimaryIndexTrigger und Axis1D.SecondaryIndexTrigger zum Schießen. Check
        //Motten brauchen Sterbeanzeige
        //Motten gewinnen, sobald Zeit abläuft, FM gewinnt, sobald innerhalb der Zeit alle Motten gefroffen wurden, dann jeweiliges Menü anzeigen

        print("Motte getroffen!");

        aufloesenAktiv = true;
        var dissolverValue = 1 - verbleibendeAufloesedauerInSekunden / 2;
        Debug.Log("DissolveValue: " + dissolverValue + "(" + verbleibendeAufloesedauerInSekunden + "");
        material.SetFloat("_Dissolve", dissolverValue);

        verbleibendeAufloesedauerInSekunden -= Time.deltaTime;

        if (verbleibendeAufloesedauerInSekunden == 0f)
        {
            Destroy(this.gameObject); //oder deaktivieren
        }

    }



    public void Update()
    {
        Prey = GameObject.FindGameObjectsWithTag("Moth");

        // Sobald alle Motten abgeschossen wurden , taucht der Endbildschirm auf.
        if (Prey.Length == 0)
        {
            print("Alle Motten getroffen.");
            WinningSound.Play();
            EndscreenBat.SetActive(true);
            EndscreenMoths.SetActive(false);
        }

        string StartTime = mytimerscript.timeValue.ToString("f0");
        TimePlayed.text = StartTime;

    }
}