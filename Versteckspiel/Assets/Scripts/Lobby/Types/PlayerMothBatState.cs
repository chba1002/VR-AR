using System;

namespace Moth.Scripts.Lobby.Types
{
    [Serializable]
    public class PlayerMothBatState
    {
        public int MothBatType;
        public bool IsSelected;
        public int LastMothBatType;
    }
}