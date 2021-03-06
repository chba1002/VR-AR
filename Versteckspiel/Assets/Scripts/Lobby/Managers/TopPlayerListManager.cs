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
    public class TopPlayerListManager
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
        public TopPlayerListManager(GameObject topMothPlayerListEntries, GameObject topPlayerListEntryPrefab, Func<GameObject, GameObject> instantiate, Action<GameObject> destroy)
        {
            this.playerListEntries = new Dictionary<int, GameObject>();
            this.playerDataProvider = new PlayerDataProvider();
            this.mothPlayerListEntries = topMothPlayerListEntries;
            this.playerListEntryPrefab = topPlayerListEntryPrefab;
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

        internal void Create(Photon.Realtime.Player p)
        {
            if (PlayerListEntries.ContainsKey(p.ActorNumber)) return; //return null;


            GameObject entry = Instantiate(playerListEntryPrefab);
            entry.transform.SetParent(mothPlayerListEntries.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<MothPlayerListEntry>().Initialize(p.ActorNumber, p.NickName);

            if (p?.ActorNumber == null)
            {
                Debug.LogWarning("Actor numer isnt set.");
            }

            PlayerListEntries.Add(p.ActorNumber, entry);

            var playerData = playerDataProvider.Provide(p);
        }
    }
}