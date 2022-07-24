﻿using System;
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

        internal void SetMothBat(int mothBatId, int lastMothBatId, bool active, int? optionalPlayerId)
        {
            //Debug.Log("UpdatePlayerSelectionPanelsSetMothBat mothBatId:" + mothBatId + " active:" + active + " [PlayerSelectionPanels.length: " + PlayerSelectionPanels.Count + "]");

            var allMothBatIdentifier = string.Join(" ", PlayerSelectionPanels);
            Debug.Log("allMothBatIdentifier: " + allMothBatIdentifier + " active: " + active + " ");

            PlayerSelectionPanels
                .Where(p => p.MothBatIdentifier == mothBatId || (lastMothBatId != 0 && mothBatId == 0 && optionalPlayerId.HasValue && p.OptionalPlayerId == optionalPlayerId.Value)) // Problem if mothBatId == 0
                .ToList()
                .ForEach(p => p.SetSelected(active, optionalPlayerId.HasValue ? optionalPlayerId.Value : null)
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