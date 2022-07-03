using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Moth.Scripts.Lobby.Managers
{
    /// <summary>
    /// Manager to handle playerListEntries.
    /// </summary>
    public class PlayerListManager
    {
        /// <summary>
        /// Get PlayerListEntries.
        /// </summary>
        public Dictionary<int, GameObject> PlayerListEntries => playerListEntries;

        private Func<GameObject, GameObject> Instantiate { get; }
        private Action<GameObject> Destroy { get; }
        private Dictionary<int, GameObject> playerListEntries;
        private readonly GameObject mothPlayerListEntries;

        /// <summary>
        /// Create a new instance of PlayerListManager.
        /// </summary>
        /// <param name="instantiate">Unity method instantiate.</param>
        /// <param name="destroy">Unity method destroy.</param>
        public PlayerListManager(GameObject mothPlayerListEntries, Func<GameObject, GameObject> instantiate, Action<GameObject> destroy)
        {
            this.playerListEntries = new Dictionary<int, GameObject>();
            this.mothPlayerListEntries = mothPlayerListEntries;
            Instantiate = instantiate;
            Destroy = destroy;
        }

        internal void ClearPlayerListEntries()
        {
            playerListEntries.Clear();
            playerListEntries = null;
        }

        internal void RemovePlayerListEntry(int actorNumber, GameObject mothPlayerListEntries)
        {
            GameObject mothPlayerListEntryGo = null;
            foreach (Transform m in mothPlayerListEntries.transform)
            {
                var playerListEntry = m.gameObject.GetComponent<MothPlayerListEntry>();
                if (playerListEntry?.PlayerActorNumber == actorNumber)
                {
                    mothPlayerListEntryGo = m.gameObject;
                }
            }

            if (mothPlayerListEntryGo != null)
            {
                Destroy(mothPlayerListEntryGo);
                playerListEntries.Remove(actorNumber);
            }
            else
            {
                Debug.LogWarning($"Couldn't remove player with actor number '{actorNumber}'");
            }

        }

        internal GameObject InitiatePlayerListEntry(
            Photon.Realtime.Player p,
            GameObject mothPlayerListEntries,
            GameObject playerListEntryPrefab)
        {
            GameObject entry = Instantiate(playerListEntryPrefab);
            entry.transform.SetParent(mothPlayerListEntries.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<MothPlayerListEntry>().Initialize(p.ActorNumber, p.NickName);
            return entry;
        }

        /// <summary>
        /// Set player ready in Ui
        /// </summary>
        /// <param name="playerReady"></param>
        /// <param name="targetPlayerActorNumber"></param>
        public void SetPlayerReadyInUi(bool playerReady, int targetPlayerActorNumber)
        {
            CurrentMothPlayerListEntries
                .Where(predicate: m => m.PlayerActorNumber == targetPlayerActorNumber)
                .FirstOrDefault()
                ?.SetPlayerReadyInUi(playerReady);
        }

        public bool AllPlayersAreReady => CurrentMothPlayerListEntries.All(m => m.IsReady);


        internal bool CheckPlayerIsReady(Photon.Realtime.Player[] playerList)
        {
            foreach (Photon.Realtime.Player p in playerList)
            {
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(MothGame.PLAYER_READY, out isPlayerReady))
                {
                    if (!(bool)isPlayerReady)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
        private List<MothPlayerListEntry> CurrentMothPlayerListEntries
        {
            get
            {
                var entries = new List<MothPlayerListEntry>();
                foreach (Transform child in mothPlayerListEntries.transform)
                {
                    child.GetComponents<MothPlayerListEntry>()
                         .ToList()
                         .ForEach(m => entries.Add(m));
                }

                return entries;
            }
        }
    }
}