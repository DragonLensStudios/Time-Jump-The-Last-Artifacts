using PXE.Core.Enums;

namespace PXE.Core.Achievements.Messaging.Messages
{
    public struct AchievementMessage
    {
        public string AchievementKey { get; }
        
        public AchievementOperator Operator { get; }

        public float Progress { get; }
        

/// <summary>
/// Executes the AchievementMessage method.
/// Handles the AchievementMessage functionality.
/// </summary>
        public AchievementMessage(string achievementKey, AchievementOperator op, float progress = 0f)
        {
            AchievementKey = achievementKey;
            Progress = progress;
            Operator = op;
        }
    }
}