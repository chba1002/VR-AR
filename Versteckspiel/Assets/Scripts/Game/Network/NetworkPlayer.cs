using UnityEngine;
using Photon.Pun;
using Unity.XR.CoreUtils;
using UnityEngine.Rendering.PostProcessing;

public class NetworkPlayer : MonoBehaviour
{
    public Transform Body;
    private Transform BodyRig;
    [SerializeField] private Transform MothBatModal;
    private PhotonView photonView;

    [Header("OVRCameraRig")]
    [SerializeField] private OVRManager OVRManager;
    [SerializeField] private OVRCameraRig OVRCameraRig;
    [SerializeField] private OVRHeadsetEmulator OVRHeadsetEmulator;
    [SerializeField] private AudioListener AudioListener;
    [SerializeField] private AudioSource AudioSource;

    [Header("CenterEyeAnchor")]
    [SerializeField] private PostProcessLayer CenterEyeAnchor_PostProcessLayer;
    [SerializeField] private AudioSource CenterEyeAnchor_AudioSource;
    [SerializeField] private Camera CenterEyeAnchor_Camera;

    [Header("HandAnchors")]
    [SerializeField] private GameObject LeftHandAnchor;
    [SerializeField] private GameObject RightHandAnchor;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        XROrigin rig = FindObjectOfType<XROrigin>();
        BodyRig = rig.transform.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor");

        Debug.Log($"NetworkPlayerScript is executed. Player is mine: {photonView.IsMine}");

        if (photonView.IsMine)
        {

            // HandAnchors
            LeftHandAnchor.SetActive(true);
            RightHandAnchor.SetActive(true);

            // OVRManager
            OVRManager.enabled = true;
            OVRCameraRig.enabled = true;
            OVRHeadsetEmulator.enabled = true;
            AudioListener.enabled = true;
            AudioSource.enabled = true;

            // CenterEyeAnchor
            CenterEyeAnchor_Camera.enabled = true;
            CenterEyeAnchor_AudioSource.enabled = true;
            CenterEyeAnchor_PostProcessLayer.enabled = true;

            MothBatModal.gameObject.SetActive(false);
        }
        else
        {
            // CenterEyeAnchor
            Destroy(CenterEyeAnchor_PostProcessLayer);
            Destroy(CenterEyeAnchor_AudioSource);
            Destroy(CenterEyeAnchor_Camera);

            // OVRManager
            Destroy(OVRManager);
            Destroy(OVRCameraRig);
            Destroy(OVRHeadsetEmulator);
            Destroy(AudioListener);
            Destroy(AudioSource);

            // HandAnchors
            LeftHandAnchor.SetActive(false);
            RightHandAnchor.SetActive(false);

            MothBatModal.gameObject.SetActive(true);
        }
    }


    private void Update()
    {
        if (photonView.IsMine)
        {
            MapPosition(Body, BodyRig);
        }
    }

    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }
}
