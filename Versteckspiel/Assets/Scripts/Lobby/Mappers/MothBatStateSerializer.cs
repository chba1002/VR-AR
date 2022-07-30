using System;
using Assets.Scripts.Shared.Managers;
using Moth.Scripts.Lobby.Types;

namespace Assets.Scripts.Lobby.Mappers
{
    class MothBatStateSerializer
    {
        public static string Serialize(int mothBatType, bool isSelected, int lastMothBatType = 0)
        {
            // ToDo: Handle exception

            var state = new PlayerMothBatState()
            {
                MothBatType = mothBatType,
                IsSelected = isSelected,
                LastMothBatType = lastMothBatType
            };

            return (Serializer.ObjectToString(state));
        }

        public static PlayerMothBatState Deserialize(string serializedObject)
        {
            // ToDo: Handle exception
            try
            {
                var deserialziedObject = Serializer.StringToObject(serializedObject);
                return deserialziedObject as PlayerMothBatState;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
