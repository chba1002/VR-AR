using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using UnityEngine.XR;

public class NetworkPlayer : MonoBehaviour
{
    public Transform Body;
    public Transform BodyRig_CenterEyeAnchor;
    private PhotonView photonView;

    // private Transform BodyRig;


    void Start()
    {
        photonView = GetComponent<PhotonView>();
        XROrigin rig = FindObjectOfType<XROrigin>();
       // BodyRig = rig.transform.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor");

        Debug.Log($"NetworkPlayerScript is executed. Player is mine: {photonView.IsMine}");

        if (photonView.IsMine)
        {
            Body.gameObject.SetActive(false);
        }
    }


    private void Update()
    {
        if (photonView.IsMine)
        {
            MapPosition(Body, BodyRig_CenterEyeAnchor);
        }
    }

    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }
}
