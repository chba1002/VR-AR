using Moth.Scripts.Lobby.Types;

namespace Assets.Scripts.Shared.Types
{
    public class PlayerData
    {
        public PlayerData(
            bool? playerIsReady, 
            PlayerMothBatState playerMothBatState, string playerName, int actorNumber, bool? isAlive)
        {
            PlayerIsReady = playerIsReady;
            PlayerMothBatState = playerMothBatState;
            PlayerName = playerName;
            ActorNumber = actorNumber;
            IsAlive = isAlive;
        }

        public bool? PlayerIsReady { get; private set; }
        public PlayerMothBatState PlayerMothBatState { get; private set; }
        public string PlayerName { get; private set; }
        public int ActorNumber { get; private set; }
        public bool? IsAlive { get; }

        internal void SetPlayerMothBatState(PlayerMothBatState playerMothBatState)
        {
            PlayerMothBatState = playerMothBatState;
        }
    }
}
