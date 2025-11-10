using System.Linq;
using System.Text;
using PXE.Core.Debug_Console.Scripts;
using PXE.Core.Enums;
using PXE.Core.Game.Managers;
using PXE.Core.Levels.Managers;
using PXE.Core.Levels.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.State_System;
using PXE.Core.State_System.Messaging.Messages;

namespace PXE.Core.Commands
{
    public class LevelCommands
    {
        [ConsoleMethod( "level.reset", "Resets Current Level" ), UnityEngine.Scripting.Preserve]
        public static string ResetLevel()
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelResetMessage());
            return "Level Reset";
        }
        
        [ConsoleMethod( "level.loadbyname", "Loads a level by name" ), UnityEngine.Scripting.Preserve]
        public static string LoadLevel(string levelName)
        {
            var level = LevelManager.Instance.Levels.FirstOrDefault(x => x.Name.Equals(levelName));
            if (level == null) return $"Level: {levelName} not found";
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(level.ID, level.Name, LevelState.Loading, level.PlayerSpawnPosition));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(GameManager.Instance.GetStateByType<GamePlayingState>()));
            return $"Level: {levelName} loaded successfully";
        }
        
        [ConsoleMethod( "level.loadbyindex", "Loads a level by index" ), UnityEngine.Scripting.Preserve]
        public static string LoadLevel(int index)
        {
            if (index > LevelManager.Instance.Levels.Count - 1) return "Level Index is out of bounds";
            var level = LevelManager.Instance.Levels[index];
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(level.ID, level.Name, LevelState.Loading, level.PlayerSpawnPosition));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(GameManager.Instance.GetStateByType<GamePlayingState>()));
            return $"Level: {level.Name} loaded successfully";
        }
        
        [ConsoleMethod( "level.getlevelnames", "Gets level names" ), UnityEngine.Scripting.Preserve]
        public static string GetLevelNames()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var level in LevelManager.Instance.Levels)
            {
                sb.AppendLine($"Level: {level.Name}");
            }
            return sb.ToString();
        }
    }
}