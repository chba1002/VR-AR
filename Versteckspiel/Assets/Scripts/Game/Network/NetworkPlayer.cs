using UnityEngine;
using Photon.Pun;

public class NetworkPlayer : MonoBehaviour
{
    public Transform Body;
    private PhotonView photonView;

    // private Transform BodyRig;


    void Start()
    {
        photonView = GetComponent<PhotonView>();
        // XRRig rig = FindObjectOfType<XRRig>();
        //  Body = rig.transform.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor");

        Debug.Log($"NetworkPlayerScript is executed. Player is mine: {photonView.IsMine}");

        if (photonView.IsMine)
        {
            Body.gameObject.SetActive(false);
            //     MapPosition(Body, BodyRig);
        }
    }

    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }
}
