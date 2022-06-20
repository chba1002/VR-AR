using UnityEngine;
using System.Collections.Generic;
using System;

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

        /// <summary>
        /// Create a new instance of PlayerListManager.
        /// </summary>
        /// <param name="instantiate">Unity method instantiate.</param>
        /// <param name="destroy">Unity method destroy.</param>
        public PlayerListManager(Func<GameObject, GameObject> instantiate, Action<GameObject> destroy)
        {
            this.playerListEntries = new Dictionary<int, GameObject>();
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
    }

}