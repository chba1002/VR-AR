using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    public Animator myAnimator;
    public Renderer myRenderer;

    public Animator targetAnimator;
    public GameObject targetObject;

    public float time;
    public float intensity;
    public float pow;
    public float timeMax = 45;

    public bool active;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (time > 0)
        {
            time--;
            active = true;
            intensity = (time / timeMax) * 10 * pow;
            UpdateRenderer();
        }
        else
        {
            active = false;
            intensity = 0;
        }
    }

    void UpdateRenderer()
    {
        myRenderer.material.SetFloat("_Intensity", intensity);
        myRenderer.material.SetFloat("_MKGlowPower", intensity);
    }
    public void Activate()
    {
        active = true;
        transform.position = targetObject.transform.position;
        transform.localScale = targetObject.transform.lossyScale;
        transform.rotation = targetObject.transform.rotation;
        myAnimator.Play(targetAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, targetAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);

        foreach (AnimatorControllerParameter param in targetAnimator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Float)
            {
                myAnimator.SetFloat(param.name, targetAnimator.GetFloat(param.name));
            }

            if (param.type == AnimatorControllerParameterType.Int)
            {
                myAnimator.SetInteger(param.name, targetAnimator.GetInteger(param.name));
            }
        }

        myAnimator.speed = 0;
        time = timeMax + 1;
        Update();
    }
}
