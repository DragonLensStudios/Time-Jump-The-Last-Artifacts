namespace PXE.Core.Achievements.Messaging.Messages
{
    public struct AchievementMenuMessage
    {
        public bool ShowMenu { get; }
        
        public AchievementMenuMessage(bool showMenu)
        {
            ShowMenu = showMenu;
        }
        
    }
}