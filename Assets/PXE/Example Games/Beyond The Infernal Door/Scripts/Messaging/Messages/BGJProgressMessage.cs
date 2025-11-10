namespace PXE.Example_Games.Beyond_The_Infernal_Door.Scripts.Messaging.Messages
{
    public struct BGJProgressMessage
    {
        public bool IsGameOver;
        
        public BGJProgressMessage(bool isGameOver)
        {
            IsGameOver = isGameOver;
        }
    }
}