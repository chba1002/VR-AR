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

    [SerializeField]
    private OVRManager OVRManager;

    [SerializeField]
    private OVRCameraRig OVRCameraRig;

    [SerializeField]
    private OVRHeadsetEmulator OVRHeadsetEmulator;

    [SerializeField]
    private OVRSystemPerfMetrics OVRSystemPerfMetrics;

    // private Transform BodyRig;


    void Start()
    {
        photonView = GetComponent<PhotonView>();
        XROrigin rig = FindObjectOfType<XROrigin>();
       // BodyRig = rig.transform.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor");

        Debug.Log($"NetworkPlayerScript is executed. Player is mine: {photonView.IsMine}");

        if (photonView.IsMine)
        {
            OVRManager.enabled = true;
            OVRCameraRig.enabled = true;
            OVRHeadsetEmulator.enabled = true;
           // OVRSystemPerfMetrics. = true;
            Body.gameObject.SetActive(false);
        }
        else
        {
            OVRManager.enabled = false;
            OVRCameraRig.enabled = false;
            OVRHeadsetEmulator.enabled = false;
            Body.gameObject.SetActive(true);
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
