using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class NetworkPlayer_Test : MonoBehaviour
{
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    private PhotonView photonView;

    private Transform headRig;
    private Transform leftHandRig;
    private Transform rightHandRig;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        XROrigin rig = FindObjectOfType<XROrigin>();
        headRig = rig.transform.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor");
        leftHandRig = rig.transform.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor");
        rightHandRig = rig.transform.Find("OVRCameraRig/TrackingSpace/RightHandAnchor");
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            //head.gameObject.SetActive(false);
            //rightHand.gameObject.SetActive(false);
            //leftHand.gameObject.SetActive(false);

            MapPosition(head, headRig);
            MapPosition(leftHand, leftHandRig);
            MapPosition(rightHand, rightHandRig);
        }
    }

    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }
}
