using Assets.Scripts.Lobby.Mappers;
using Assets.Scripts.Shared.Types;
using Moth.Scripts;
using Moth.Scripts.Lobby.Types;
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
            bool? _playerIsAlive = null;

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
                _playerName = ((string)playerNameObject);
                Debug.Log($"TryGetValue PLAYER_NAME: {(string)playerNameObject}");
            }

            if (player.CustomProperties.TryGetValue(MothGame.PLAYER_IS_ALIVE, out object playerIsAlive))
            {
                _playerIsAlive = playerIsAlive as bool?;
            }


            return new PlayerData(_playerIsReady, _playerMothBatState, _playerName, player.ActorNumber, _playerIsAlive);
        }

        public PlayerData Provide(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            bool? _playerIsReady = null;
            PlayerMothBatState _playerMothBatState = null;
            string _playerName = null;
            bool? _playerIsAlive = null;

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

            if (changedProps.TryGetValue(MothGame.PLAYER_IS_ALIVE, out object playerIsAlive))
            {
                _playerIsAlive = playerIsAlive as bool?;
            }

            return new PlayerData(_playerIsReady, _playerMothBatState, _playerName, targetPlayer.ActorNumber, _playerIsAlive);
        }

        public MothBatActionType TryProvideMothBatActionType(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            MothBatActionType mothBatActionType = null;

            if (changedProps.TryGetValue(MothGame.PLAYER_MOTH_BAT_ACTION_TYPE, out object serializedPlayerMothBatActionType))
            {

                var deserializedValue = Serializer.StringToObject(serializedPlayerMothBatActionType as string);

                if(deserializedValue != null)
                {
                    mothBatActionType = deserializedValue as MothBatActionType;
                }
            }

            return mothBatActionType;
        }

        public bool? TryProvidePlayerIsAlive(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            bool? _playerIsAlive = null;

            if (changedProps.TryGetValue(MothGame.PLAYER_IS_ALIVE, out object playerIsAlive))
            {
                _playerIsAlive = playerIsAlive as bool?;
            }
            return _playerIsAlive;
        }

        public bool? TryProvidePlayerIsInvulnerable(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            bool? _playerIsInvulnerable = null;

            if (changedProps.TryGetValue(MothGame.PLAYER_MOTH_IS_INVULNERABLE, out object playerIsInvulnerable))
            {
                _playerIsInvulnerable = playerIsInvulnerable as bool?;
            }
            return _playerIsInvulnerable;
        }
    }
}
