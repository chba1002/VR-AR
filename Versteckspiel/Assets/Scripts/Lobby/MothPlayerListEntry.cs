// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerListEntry.cs" company="Exit Games GmbH">
//   Part of: Asteroid Demo,
// </copyright>
// <summary>
//  Player List Entry
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
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
        private bool isPlayerReady;

        #region UNITY

        public void OnEnable()
        {
            PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
        }

        public void Start()
        {

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
        public void SetPlayerReady(bool ready){
            Debug.Log($"SetPlayer {PlayerActorNumber} ready: {ready}");
            isPlayerReady = ready;
            PlayerReadyImage.enabled = ready;
        }
    }
}