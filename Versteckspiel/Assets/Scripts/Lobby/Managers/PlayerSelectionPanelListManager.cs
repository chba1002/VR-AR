using Assets.Scripts.Shared.Types;
using Moth.Scripts.Lobby.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Lobby.Managers
{
    class PlayerSelectionPanelListManager
    {
        private GameObject playerSelectionPanelElements;

        public List<PlayerSelectionPanel> PlayerSelectionPanels { get; }

        public PlayerSelectionPanelListManager(GameObject playerSelectionPanelElements)
        {
            this.playerSelectionPanelElements = playerSelectionPanelElements;

            PlayerSelectionPanels = new List<PlayerSelectionPanel>();
            foreach (Transform child in playerSelectionPanelElements.transform)
            {
                var playerSelectionPanels = child.GetComponents<PlayerSelectionPanel>().ToList();
                PlayerSelectionPanels.AddRange(playerSelectionPanels);
            }
        }

        internal void SetMothBat(PlayerData playerData, Photon.Realtime.Player optionalPlayer = null)
        {
            var mothBatId = playerData.PlayerMothBatState.MothBatType;
            var lastMothBatId = playerData.PlayerMothBatState.LastMothBatType;
            var active = playerData.PlayerMothBatState.IsSelected;

            var allMothBatIdentifier = string.Join(" ", PlayerSelectionPanels);
            Debug.Log("allMothBatIdentifier: " + allMothBatIdentifier + " active: " + active + " playerData.PlayerName" + playerData.PlayerName);
            Debug.Log("optionalPlayer?.ActorNumber: " + optionalPlayer?.ActorNumber);

            PlayerSelectionPanels
                .Where(p => optionalPlayer?.ActorNumber == p.OptionalPlayerId
                    || p.PlayerName.text == optionalPlayer.NickName // ToDo: This is really ugly .. but it works .. finally
                    || p.MothBatIdentifier == mothBatId 
                    || (lastMothBatId != 0 && mothBatId == 0 && optionalPlayer != null && p.OptionalPlayerId == optionalPlayer.ActorNumber)) // Problem if mothBatId == 0
                .ToList()
                .ForEach(p => p.SetSelected(playerData, optionalPlayer)
            );
        }

        internal void UpdatePlayerIsReady(int actorNumber, bool playerIsReady)
        {
            PlayerSelectionPanels
                .Where(p => p.OptionalPlayerId == actorNumber)
                .ToList()
                .ForEach(p => p.SetReady(playerIsReady)
            );
        }
    }
}
