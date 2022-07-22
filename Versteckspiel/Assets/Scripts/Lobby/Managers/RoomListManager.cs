using UnityEngine;
using System.Collections.Generic;
using System;
using Photon.Realtime;

namespace Moth.Scripts.Lobby.Managers
{

    public class RoomListManager
    {
        private Action<GameObject> Destroy { get; }
        private Func<GameObject, GameObject> Instantiate { get; }
        private Dictionary<string, GameObject> roomListEntries;
        private int roomCounter;
        private Dictionary<string, RoomInfo> cachedRoomList;
        private readonly GameObject RoomListEntryPrefab;
        private readonly GameObject RoomListContent;

        public RoomListManager(
            Func<GameObject, GameObject> instantiate,
            Action<GameObject> destroy,
            GameObject roomListEntryPrefab,
            GameObject roomListContent)
        {
            roomCounter = 0;
            cachedRoomList = new Dictionary<string, RoomInfo>();
            roomListEntries = new Dictionary<string, GameObject>();
            Destroy = destroy;
            Instantiate = instantiate;
            RoomListEntryPrefab = roomListEntryPrefab;
            RoomListContent = roomListContent;
        }

        public void ClearCachedRoomList()
        {
            cachedRoomList.Clear();
        }

        internal void ClearRoomListView()
        {
            foreach (GameObject entry in roomListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            roomListEntries.Clear();
        }

        internal void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            foreach (RoomInfo info in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (cachedRoomList.ContainsKey(info.Name))
                    {
                        cachedRoomList.Remove(info.Name);
                    }

                    continue;
                }

                // Update cached room info
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                // Add new room info to cache
                else
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }
        }

        internal void UpdateRoomListView()
        {
            foreach (RoomInfo info in cachedRoomList.Values)
            {
                GameObject entry = Instantiate(RoomListEntryPrefab);
                entry.transform.SetParent(RoomListContent.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<MothRoomListEntry>().Initialize(++roomCounter, info.Name, (byte)info.PlayerCount, info.MaxPlayers);

                roomListEntries.Add(info.Name, entry);
            }
        }

        internal string GenerateRoomName()
        {
            return "Room " + UnityEngine.Random.Range(1000, 10000);
        }

        internal (string, RoomOptions) CreateRoom(string roomName, string maxPlayersString, Func<float, float, float> randomRange)
        {
            roomName = (roomName.Equals(string.Empty)) ? "Room " + randomRange(1000, 10000) : roomName;

            byte maxPlayers;
            byte.TryParse(maxPlayersString, out maxPlayers);
            maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 8);

            RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 10000 };
            return (roomName, options);
        }
    }

}