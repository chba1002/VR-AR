using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Pun.UtilityScripts;

namespace Moth.Scripts.Lobby
{
    public class MothPlayerListEntry : MonoBehaviour
    {
        [Header("UI References")]
        public Text PlayerNameText;

        public Image PlayerColorImage;
       // public Button PlayerReadyButton;
        public Image PlayerReadyImage;

        public int PlayerActorNumber {get; private set;}
        public bool IsReady => isPlayerReady;
        private bool isPlayerReady;

        #region UNITY

        public void OnEnable()
        {
            PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
        }


        public void OnDisable()
        {
            PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
        }

        #endregion

        /// <summary>
        /// Initializes a new MothPlayerListEntry.
        /// </summary>
        /// <param name="playerId">Player actor number.</param>
        /// <param name="playerName">Name of the player.</param>
        public void Initialize(int playerId, string playerName)
        {
            PlayerActorNumber = playerId;
            PlayerNameText.text = playerName;
        }

        private void OnPlayerNumberingChanged()
        {
            foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == PlayerActorNumber)
                {
                    PlayerColorImage.color = MothGame.GetColor(p.GetPlayerNumber());
                }
            }
        }

        /// <summary>
        /// Set selected player ready and visualize it.
        /// </summary>
        /// <param name="ready">Indicates whether the player is ready.</param>
        public void SetPlayerReadyInUi(bool ready){
            var playerTypeString = PhotonNetwork.LocalPlayer.ActorNumber == PlayerActorNumber ? "Local" : "Remote";
            var isReadyString = ready ? "ist bereit" : "ist nicht mehr bereit";
            Debug.Log($"{playerTypeString}: UI - Spieler {PlayerActorNumber} {isReadyString}.");

            isPlayerReady = ready;
            //PlayerReadyImage.enabled = ready;
        }
    }
}