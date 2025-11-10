namespace PXE.Example_Games.Oceans_Call.Messages
{
    public struct PlayerInfoMessage
    {
        public int Lives { get; }
        public float MetersTraveled { get; }

        public PlayerInfoMessage(int lives, float metersTraveled)
        {
            Lives = lives;
            MetersTraveled = metersTraveled;
        }
    }
}