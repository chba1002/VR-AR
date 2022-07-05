using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkPlayer : MonoBehaviour
{
    public Transform Body;
    private PhotonView photonView;

    private Transform BodyRig;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        XRRig rig = FindObjectOfType<XRRig>();
        Body = rig.transform.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor");
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine)
        {
            Body.gameObject.SetActive(false);
            MapPosition(Body, BodyRig);
        }
    }

    void MapPosition(Transform target,Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }
}
