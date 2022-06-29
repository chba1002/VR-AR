using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ShootAtMoth : MonoBehaviour
{
    [SerializeField] Transform raycastOrigin;
    [SerializeField] LayerMask InteractionLayerMask;

    public void ShootMoth()
    {
        //Sobald der Strahl Motte ber�hrt, kann man schie�en.
        //Beide Strahlen m�ssen Motte treffen, damit sie stirbt. Sonst passiert nichts.
        //Axis1D.PrimaryIndexTrigger und Axis1D.SecondaryIndexTrigger zum Schie�en.
        //

        RaycastHit hit;

        if(Physics.Raycast(raycastOrigin.position, raycastOrigin.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, InteractionLayerMask))
        {
            print("Treffer!");
            Destroy(this.gameObject);
        }
    }
}
