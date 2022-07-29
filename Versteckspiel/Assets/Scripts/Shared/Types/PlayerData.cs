using Moth.Scripts.Lobby.Types;

namespace Assets.Scripts.Shared.Types
{
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
