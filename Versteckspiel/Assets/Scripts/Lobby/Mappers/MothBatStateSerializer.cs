using System;
using System.IO;
using Moth.Scripts.Lobby.Types;
using System.Runtime.Serialization.Formatters.Binary;

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

            return (ObjectToString(state));
        }

        public static PlayerMothBatState Deserialize(string serializedObject)
        {
            // ToDo: Handle exception
            try
            {
                var deserialziedObject = StringToObject(serializedObject);
                return deserialziedObject as PlayerMothBatState;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string ObjectToString(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, obj);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private static object StringToObject(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
            {
                ms.Write(bytes, 0, bytes.Length);
                ms.Position = 0;
                return new BinaryFormatter().Deserialize(ms);
            }
        }
    }
}
