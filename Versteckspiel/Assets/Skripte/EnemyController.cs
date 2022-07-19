using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Experimental.VFX;

public class EnemyController : MonoBehaviour
{
    public bool aufloesenAktiv;
    public float verbleibendeAufloesedauerInSekunden = 0;

    public GameObject mothMesh;
    public Material dissolveShaderMaterial;
    private Material material;

    // Start is called before the first frame update
    void Start()
    {
        // Erstelle neues Material auf basis des Prefab Materials, damit jeder Enemy eigenes Material hat.
        var dissolverShader = new Material(dissolveShaderMaterial);

        // Weise neu erstelltes Material dem Spinnen Mesh zu.
        mothMesh.GetComponent<Renderer>().sharedMaterial = dissolverShader;

        // Speichere das Material in der variable 'material' zwischen.
        material = mothMesh.GetComponent<Renderer>().sharedMaterial;
    }
    void Update()
    {
        if (aufloesenAktiv)
        {
            verbleibendeAufloesedauerInSekunden -= Time.deltaTime;
            var dissolverValue = 1 - verbleibendeAufloesedauerInSekunden / 2;
            //Debug.Log("DissolveValue: " + dissolverValue + "(" + verbleibendeAufloesedauerInSekunden + "");

            material.SetFloat("_Dissolve", dissolverValue);

            if (verbleibendeAufloesedauerInSekunden <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Aufloesen()
    {
        aufloesenAktiv = true;
        verbleibendeAufloesedauerInSekunden = 2;
    }
}
