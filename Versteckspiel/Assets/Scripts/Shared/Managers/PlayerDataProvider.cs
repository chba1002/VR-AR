using Assets.Scripts.Lobby.Mappers;
using Moth.Scripts;
using Moth.Scripts.Lobby.Types;
using System;
using UnityEngine;

namespace Assets.Scripts.Shared.Managers
{
    public class PlayerDataProvider
    {
        public PlayerData Provide(Photon.Realtime.Player player)
        {
            bool? _playerIsReady = null;
            PlayerMothBatState _playerMothBatState = null;
            string _playerName = null;

            if (player.CustomProperties.TryGetValue(MothGame.PLAYER_READY, out object isPlayerReady))
            {
                _playerIsReady = isPlayerReady as bool?;
            }

            if (player.CustomProperties.TryGetValue(MothGame.PLAYER_MOTH_BAT_STATE, out object playerMothBatStateObject))
            {
                _playerMothBatState = MothBatStateSerializer.Deserialize(((string)playerMothBatStateObject));
            }

            if (player.CustomProperties.TryGetValue(MothGame.PLAYER_NAME, out object playerNameObject))
            {
                _playerName =((string)playerNameObject);
                Debug.Log($"TryGetValue PLAYER_NAME: {(string)playerNameObject}");
            }

            return new PlayerData(_playerIsReady, _playerMothBatState, _playerName, player.ActorNumber);
        }

        public PlayerData Provide(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            bool? _playerIsReady = null;
            PlayerMothBatState _playerMothBatState = null;
            string _playerName = null;

            if (changedProps.TryGetValue(MothGame.PLAYER_READY, out object isPlayerReady))
            {
                _playerIsReady = isPlayerReady as bool?;
            }

            if (changedProps.TryGetValue(MothGame.PLAYER_MOTH_BAT_STATE, out object playerMothBatStateObject))
            {
                _playerMothBatState = MothBatStateSerializer.Deserialize(((string)playerMothBatStateObject));

                if (_playerMothBatState == null)
                {
                    Debug.LogError($"PlayerMothBatState konnte nicht geparsed werden.");
                }
            }

            if (changedProps.TryGetValue(MothGame.PLAYER_NAME, out object playerNameObject))
            {
                _playerName = ((string)playerNameObject);
                Debug.Log($"TryGetValue PLAYER_NAME (changedProps): {(string)playerNameObject}");
            }

            return new PlayerData(_playerIsReady, _playerMothBatState, _playerName, targetPlayer.ActorNumber);
        }
    }

    public class PlayerData
    {
        public PlayerData(bool? playerIsReady, PlayerMothBatState playerMothBatState, string playerName, int actorNumber)
        {
            PlayerIsReady = playerIsReady;
            PlayerMothBatState = playerMothBatState;
            PlayerName = playerName;
            ActorNumber = actorNumber;
        }

        public bool? PlayerIsReady { get; private set; }
        public PlayerMothBatState PlayerMothBatState { get; private set; }
        public string PlayerName { get; private set; }
        public int ActorNumber { get; private set; }

        internal void SetPlayerMothBatState(PlayerMothBatState playerMothBatState)
        {
            PlayerMothBatState = playerMothBatState;
        }
    }
}
