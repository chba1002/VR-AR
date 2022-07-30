using System;

namespace Moth.Scripts.Lobby.Types
{
    [Serializable]
    public class MothBatActionType
    {
        public int ActorNumber;
        public MothBatType MothBatType;
        public AttackType AttackType;
    }
}