using PXE.Core.SerializableTypes;

namespace PXE.Example_Games.Oceans_Call.Messages
{
    public struct PlayerLifePowerUpMessage
    {
        public SerializableGuid ID { get; }
        public int Lives { get; }

        public PlayerLifePowerUpMessage(SerializableGuid id, int lives)
        {
            ID = id;
            Lives = lives;
        }
    }
}