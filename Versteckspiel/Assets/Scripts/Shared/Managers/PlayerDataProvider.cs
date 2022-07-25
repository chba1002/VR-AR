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

            if (player.CustomProperties.TryGetValue(MothGame.PLAYER_READY, out object isPlayerReady))
            {
                _playerIsReady = isPlayerReady as bool?;
            }

            if (player.CustomProperties.TryGetValue(MothGame.PLAYER_MOTH_BAT_STATE, out object playerMothBatStateObject))
            {
                _playerMothBatState = MothBatStateSerializer.Deserialize(((string)playerMothBatStateObject));
            }

            return new PlayerData(_playerIsReady, _playerMothBatState, player.ActorNumber);
        }

        public PlayerData Provide(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            bool? _playerIsReady = null;
            PlayerMothBatState _playerMothBatState = null;

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

            return new PlayerData(_playerIsReady, _playerMothBatState, targetPlayer.ActorNumber);
        }
    }

    public class PlayerData
    {
        public PlayerData(bool? playerIsReady, PlayerMothBatState playerMothBatState, int actorNumber)
        {
            PlayerIsReady = playerIsReady;
            PlayerMothBatState = playerMothBatState;
            ActorNumber = actorNumber;
        }

        public bool? PlayerIsReady { get; private set; }
        public PlayerMothBatState PlayerMothBatState { get; private set; }
        public int ActorNumber { get; private set; }

    }
}
