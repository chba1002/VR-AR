using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Assets.Scripts.Shared.Managers;

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

        private GameObject playerListEntryPrefab;
        private PlayerDataProvider playerDataProvider;

        /// <summary>
        /// Create a new instance of PlayerListManager.
        /// </summary>
        /// <param name="instantiate">Unity method instantiate.</param>
        /// <param name="destroy">Unity method destroy.</param>
        public PlayerListManager(GameObject mothPlayerListEntries, GameObject playerListEntryPrefab,  Func<GameObject, GameObject> instantiate, Action<GameObject> destroy)
        {
            this.playerListEntries = new Dictionary<int, GameObject>();
            this.playerDataProvider = new PlayerDataProvider();
            this.mothPlayerListEntries = mothPlayerListEntries;
            this.playerListEntryPrefab = playerListEntryPrefab;
            Instantiate = instantiate;
            Destroy = destroy;
        }

        internal void ClearPlayerListEntries()
        {
            PlayerListEntries.Values.ToList().ForEach(entry => Destroy(entry.gameObject));
            playerListEntries.Clear();
            playerListEntries = null;
        }

        internal void Remove(int actorNumber)
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

        internal GameObject Create(Photon.Realtime.Player p)
        {
            GameObject entry = Instantiate(playerListEntryPrefab);
            entry.transform.SetParent(mothPlayerListEntries.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<MothPlayerListEntry>().Initialize(p.ActorNumber, p.NickName);

            if(p?.ActorNumber == null)
            {
                Debug.LogWarning("Actor numer isnt set.");
            }

            PlayerListEntries.Add(p.ActorNumber, entry);

            var playerData = playerDataProvider.Provide(p);
            if (playerData.PlayerIsReady.HasValue)
            {
                SetPlayerReadyInUi(playerData);
            }

            return entry;
        }

        /// <summary>
        /// Set player ready in Ui
        /// </summary>
        /// <param name="playerReady"></param>
        /// <param name="targetPlayerActorNumber"></param>
        public void SetPlayerReadyInUi(PlayerData playerData)
        {
            if (!playerData.PlayerIsReady.HasValue)
            {
                Debug.Log("Cannot SetPlayerReadyInUi. PlayerIsReady has no value.");
                return;
            }

            CurrentMothPlayerListEntries
                .Where(predicate: m => m.PlayerActorNumber == playerData.ActorNumber)
                .FirstOrDefault()
                ?.SetPlayerReadyInUi(playerData.PlayerIsReady.Value);
        }

        public bool AllPlayersAreReady => CurrentMothPlayerListEntries.All(m => m.IsReady);

        internal bool CheckAllPlayersAreReady(Photon.Realtime.Player[] playerList, bool isMasterClient)
        {
            if (!isMasterClient) return false;

            foreach (Photon.Realtime.Player p in playerList)
            {
                if (p.CustomProperties.TryGetValue(MothGame.PLAYER_READY, out object isPlayerReady))
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