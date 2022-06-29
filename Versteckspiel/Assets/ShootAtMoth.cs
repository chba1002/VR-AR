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
        //Sobald der Strahl Motte berührt, kann man schießen.
        //Beide Strahlen müssen Motte treffen, damit sie stirbt. Sonst passiert nichts.
        //Axis1D.PrimaryIndexTrigger und Axis1D.SecondaryIndexTrigger zum Schießen.
        //

        RaycastHit hit;

        if(Physics.Raycast(raycastOrigin.position, raycastOrigin.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, InteractionLayerMask))
        {
            print("Treffer!");
            Destroy(this.gameObject);
        }
    }
}
