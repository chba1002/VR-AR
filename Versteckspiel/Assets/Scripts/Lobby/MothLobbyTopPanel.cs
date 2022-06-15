using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace Moth.Scripts.Lobby
{
    public class MothLobbyTopPanel : MonoBehaviour
    {
        private readonly string connectionStatusMessage = "Verbindungsstatus: ";

        [Header("UI References")]
        public Text ConnectionStatusText;

        #region UNITY

        public void Update()
        {
            ConnectionStatusText.text = connectionStatusMessage + PhotonNetwork.NetworkClientState;
        }

        #endregion
    }
}