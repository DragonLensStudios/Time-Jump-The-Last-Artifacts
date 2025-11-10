namespace PXE.Example_Games.Oceans_Call.Messages
{
    public struct LetterMessage
    {
        public string Message { get; }
        public float TimeToDisplay { get; }

        public LetterMessage(string message, float timeToDisplay = 10f)
        {
            Message = message;
            TimeToDisplay = timeToDisplay;
        }
    }
}