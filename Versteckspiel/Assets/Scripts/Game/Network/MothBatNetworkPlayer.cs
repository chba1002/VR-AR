using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using Moth.Scripts.Lobby.Managers;
using static OVRInput;

public class MothBatNetworkPlayer : MonoBehaviour
{
    [SerializeField]
    private MothBatType mothBatType;
    
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    private PhotonView photonView;

    private Transform headRig;
    private Transform leftHandRig;
    private Transform rightHandRig;

    private MothGameManager mothGameManager;

    // Start is called before the first frame update
    void Start()
    {
        mothGameManager = GameObject
            .FindGameObjectWithTag(TagProvider.MothGameManager)?
            .GetComponent<MothGameManager>();

        photonView = GetComponent<PhotonView>();
        XROrigin rig = FindObjectOfType<XROrigin>();
        headRig = rig.transform.Find("Camera Offset/Main Camera");
        leftHandRig = rig.transform.Find("Camera Offset/LeftHand Controller");
        rightHandRig = rig.transform.Find("Camera Offset/RightHand Controller");
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
                var actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

            if (Input.GetKeyDown("e"))
            {
                Debug.Log("0");
                mothGameManager.MothBarExecuteInteraction(actorNumber, mothBatType);
            }

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