using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Experimental.VFX;

public enum EnemyType {  SmallEnemy, BigEnemy }

public class EnemyController : MonoBehaviour
{
    Animator Animator;
    public bool aufloesenAktiv;
    public float verbleibendeAufloesedauerInSekunden = 0;
    public bool angriffAktiv;
    //public bool StrikeLightning = false;
    public float verbleibendeAngriffsdauerInSekunden = 0;
    public bool hatBereitsAngegriffen;
    private schachfigur schachfigur;
    public int AnzahlDerAngriffe;

    public EnemyType EnemyType = EnemyType.SmallEnemy;

    public GameObject spiderMesh;
    public Material dissolveShaderMaterial;
    public VisualEffect Blitz;
    //private Material dissolverShader;
    private Material material;
    private SoundController soundController;

    // Start is called before the first frame update
    void Start()
    {
        Blitz.enabled = false;
        AnzahlDerAngriffe = 0;
        angriffAktiv = false;
        Animator = GetComponent<Animator>();

        var schachfiguren = FindObjectsOfType<schachfigur>();

        schachfigur = schachfiguren[0];

        // Erstelle neues Material auf basis des Prefab Materials, damit jeder Enemy eigenes Material hat.
        var dissolverShader = new Material(dissolveShaderMaterial);

        // Weise neu erstelltes Material dem Spinnen Mesh zu.
        spiderMesh.GetComponent<Renderer>().sharedMaterial = dissolverShader;

        // Speichere das Material in der variable 'material' zwischen.
        material = spiderMesh.GetComponent<Renderer>().sharedMaterial;
        soundController = FindObjectsOfType<SoundController>()[0];
    }
    void Update()
    {
        if (angriffAktiv)
        {
            verbleibendeAngriffsdauerInSekunden -= Time.deltaTime;

            if (verbleibendeAngriffsdauerInSekunden <= 0)
            {
                angriffAktiv = false;
                hatBereitsAngegriffen = true;
            }
        }

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

    public bool EnemyIsDead()
    {
        return Animator.GetCurrentAnimatorStateInfo(0).IsName("dead");
    }

    public void Attack()
    {
        angriffAktiv = true;
        verbleibendeAngriffsdauerInSekunden = 2;
        Animator.SetTrigger("AttackTrigger");

        AnzahlDerAngriffe = AnzahlDerAngriffe + 1;

        switch (EnemyType)
        {
            case EnemyType.SmallEnemy:
                soundController.PlaySpinneGreiftAn();
                break;
            case EnemyType.BigEnemy:
                soundController.PlayBigEnemyAttackSound();
                break;
        }
    }

    public void Aufloesen()
    {
        // Blitz auslösen
        Blitz.enabled = true;
        Blitz.Play();
        // Kristall wird abgezogen
        Energy.RemoveEnergy();
        aufloesenAktiv = true;
        verbleibendeAufloesedauerInSekunden = 2;


        switch (EnemyType)
        {
            case EnemyType.SmallEnemy:
                soundController.PlaySpinneBesiegt();
                break;
            case EnemyType.BigEnemy:
                soundController.PlayBigEnemyBesiegtSound();
                break;
        }

    }

    /// <summary>
    /// Wird in Animationsfenster aufgerufen, wenn Gegner Spieler berührt.
    /// </summary>
    public void ZeitpunktBeruehrungSpieler()
    {
        schachfigur.ReceiveDamage();
        // Blitzattacke der Spinne schlägt ein
    }

    public void ReceiveDamage()
    {
        Animator.SetTrigger("ReceiveDamageTrigger");
    }
}
