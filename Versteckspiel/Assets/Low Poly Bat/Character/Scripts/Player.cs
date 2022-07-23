using UnityEngine;

public class Player : MonoBehaviour
{
    private bool pcControllIsActive = false;

    [SerializeField]
    private Transform CenterEyeAnchorTransform;

    private Animator anim;
    private CharacterController controller;

    public float speed = 600.0f;
    public float turnSpeed = 400.0f;
    private Vector3 moveDirection = Vector3.zero;
    public float gravity = 20.0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = gameObject.GetComponentInChildren<Animator>();

        if(controller == null)
        {
            Debug.Log("CharacterController on player is inactive.");
        }
        Debug.Log("Start Player Script");
    }

    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            pcControllIsActive = !pcControllIsActive;
            Debug.Log(pcControllIsActive ? "Steuerung für PC aktiv" : "Steuerung für PC inaktiv");
        }

        if (Input.GetKey("w"))
        {
            anim.SetInteger("AnimationPar", 1);
        }
        else
        {
            anim.SetInteger("AnimationPar", 0);
        }

        if (controller?.isGrounded ?? false)
        {
            moveDirection = transform.forward * Input.GetAxis("Vertical") * speed;
        }

        // Information: If controll via pc is neccessary - use this transform input to controll.
        if (pcControllIsActive)
        {
            float turn = Input.GetAxis("Horizontal");
            transform.Rotate(0, turn * turnSpeed * Time.deltaTime, 0);
        }

        this.transform.position = CenterEyeAnchorTransform.position;

        if (controller.enabled)
        {
            controller.Move(moveDirection * Time.deltaTime);
        }
        moveDirection.y -= gravity * Time.deltaTime;
    }
}
