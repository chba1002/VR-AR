using UnityEngine;
using TMPro;

public class ShootAtMoth : MonoBehaviour
{
    public GameObject EndscreenBat;
    public GameObject EndscreenMoths;
    public GameObject TimerScreen;
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
        //Sobald der Strahl Motte berührt, kann man schießen. Check
        //Beide Strahlen müssen Motte treffen, damit sie stirbt. Sonst passiert nichts.
        //Axis1D.PrimaryIndexTrigger und Axis1D.SecondaryIndexTrigger zum Schießen. Check
        //Motten brauchen Sterbeanzeige
        //Motten gewinnen, sobald Zeit abläuft, FM gewinnt, sobald innerhalb der Zeit alle Motten gefroffen wurden, dann jeweiliges Menü anzeigen

        print("Motte getroffen!");
        aufloesenAktiv = true;
        //Destroy(mothMesh.gameObject); //oder deaktivieren

    }



    public void Update()
    {
        Prey = GameObject.FindGameObjectsWithTag("Moth");


        if (Input.GetKeyDown(KeyCode.T))
        {
            aufloesenAktiv = true;
        }

        // Die Motte soll erst als getroffen zählen, wenn beide Strahen auf sie schießen, das unten reicht noch nicht
        if (OVRInput.GetDown(OVRInput.GetButtonDown.PrimaryHandTrigger) && OVRInput.GetDown(OVRInput.GetButtonDown.SecondaryHandTrigger)) 
        {
            print("Two buttons pressed");
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
            Destroy(mothMesh.gameObject); //das Objekt, aus das geschossen wurde
        }


        // Sobald alle Motten abgeschossen wurden , taucht der Endbildschirm auf.
        if (Prey.Length == 0)
        {
            print("Alle Motten getroffen.");
            WinningSound.Play();
            TimerScreen.SetActive(false);
            EndscreenBat.SetActive(true);
            EndscreenMoths.SetActive(false);
        }
    }
}