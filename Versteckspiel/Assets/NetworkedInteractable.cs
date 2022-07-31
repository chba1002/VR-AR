using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkedInteractable : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{
    internal XRBaseInteractable m_InteractableBase;
    internal Photon.Realtime.Player owner;
    internal Rigidbody rigidBody;
    internal XRController interactingController;
    //private bool isBeingHeld;
    public UnityEvent RpcActivate;
    public UnityEvent RpcDeactivate;
    public UnityEvent RpcSelect;
    public bool useGravity = true;
    public UnityEvent RpcDeselect;

    // Start is called before the first frame update
    void Awake()
    {
        m_InteractableBase = GetComponent<XRBaseInteractable>();
        rigidBody = GetComponent<Rigidbody>();
        m_InteractableBase.onSelectEntered.AddListener(OnSelectEnter);
        m_InteractableBase.onSelectExited.AddListener(OnSelectExit);
        m_InteractableBase.onActivate.AddListener(OnActivate);
        m_InteractableBase.onDeactivate.AddListener(OnDeactivate);
    }

    void Update()
    {
        /*
        if (isBeingHeld)
        {
            if (rigidBody)
                rigidBody.isKinematic = true;
            gameObject.layer = LayerMask.NameToLayer("InHand");
        }

        else
        {
            if (rigidBody && useGravity)
                rigidBody.isKinematic = false;
            gameObject.layer = LayerMask.NameToLayer("Interactable");
        }
        */
    }

    void TransferOwnership()
    {
        photonView.RequestOwnership();
    }

    public void OnSelectEnter(XRBaseInteractor obj)
    {

        Debug.Log("INTERACTABLE: OnSelectEnter: " + obj.name);
        interactingController = obj.GetComponent<XRController>();
        print("NetworkedInteractable -> OnSelectEnter");
        if (photonView && PhotonNetwork.InRoom)
        {
            photonView.RPC("RpcOnSelect", RpcTarget.AllBuffered);
            if (photonView.Owner != PhotonNetwork.LocalPlayer)
            {
                TransferOwnership();
            }
        }
        else
        {
            RpcOnSelect();
        }
    }

    public void OnSelectExit(XRBaseInteractor obj)
    {
        if (photonView && PhotonNetwork.InRoom)
        {
            photonView.RPC("RpcOnDeselect", RpcTarget.AllBuffered);
        }
        else
        {
            RpcOnDeselect();
        }
        interactingController = null;
    }

    public void OnDeactivate(XRBaseInteractor obj)
    {
        if (photonView && PhotonNetwork.InRoom)
            photonView.RPC("RpcOnDeactivate", RpcTarget.AllBuffered);
        else
            RpcOnDeactivate();
    }

    public void OnActivate(XRBaseInteractor obj)
    {
        if (photonView && PhotonNetwork.InRoom)
            photonView.RPC("RpcOnActivate", RpcTarget.AllBuffered);
        else
            RpcOnActivate();
    }

    [PunRPC]
    public void RpcOnSelect()
    {
        Debug.Log("INTERACTABLE: RpcOnSelect");
        RpcSelect.Invoke();
    }

    [PunRPC]
    public void RpcOnDeselect()
    {
        Debug.Log("INTERACTABLE: RpcOnDeselect");
        RpcDeselect.Invoke();
    }

    [PunRPC]
    public void RpcOnActivate()
    {
        RpcActivate.Invoke();
    }

    [PunRPC]
    public void RpcOnDeactivate()
    {
        RpcDeactivate.Invoke();
    }


    public void OnOwnershipRequest(PhotonView targetView, Photon.Realtime.Player requestingPlayer)
    {
        Debug.Log("INTERACTABLE: OnOwnershipRequest");
        if (targetView != photonView)
        {
            return;
        }

        print("Ownership Requested for: " + targetView.name + " from " + requestingPlayer.NickName);
        photonView.TransferOwnership(requestingPlayer);
        owner = requestingPlayer;
    }
    

    public void OnOwnershipTransfered(PhotonView targetView, Photon.Realtime.Player previousOwner)
    {
        print("Ownership Transfered. New Owner: " + targetView.Owner.NickName);
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Photon.Realtime.Player senderOfFailedRequest)
    {
        Debug.Log("INTERACTABLE: OnOwnershipTransfered");
    }
}
