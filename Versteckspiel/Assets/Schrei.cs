using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Schrei : MonoBehaviour
{
    public ParticleSystem Wellen;

    // Update is called once per frame
    void Update()
    {
       if (Input.GetKeyUp(KeyCode.Space))
            {
            Wellen.Play();
            }
    }
}
