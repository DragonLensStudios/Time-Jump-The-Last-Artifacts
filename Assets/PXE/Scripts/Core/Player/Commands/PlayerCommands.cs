using PXE.Core.Debug_Console.Scripts;
using PXE.Core.Enums;
using PXE.Core.Messaging;
using PXE.Core.Messaging.Messages;
using PXE.Core.Player.Managers;
using UnityEngine;

namespace PXE.Core.Player.Commands
{
    public class PlayerCommands
    {
        [ConsoleMethod( "godmode", "Sets GodMode" ), UnityEngine.Scripting.Preserve]
        public static void ToggleGodMode(bool enabled)
        {
			MessageSystem.MessageManager.SendImmediate(MessageChannels.Player, new GodModeMessage(enabled));
        }
        
        [ConsoleMethod( "player.setposition", "Sets Player Position" ), UnityEngine.Scripting.Preserve]
        public static void SetPlayerPosition(Vector3 position)
        {
            var player = PlayerManager.Instance.Player;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Object, new TransformPositionMessage(player.ID, position, Quaternion.identity));
        }
        
        [ConsoleMethod( "player.getposition", "Gets Player Position" ), UnityEngine.Scripting.Preserve]
        public static string GetPlayerPosition()
        {
            var player = PlayerManager.Instance.Player;
            if (player == null) return "No Player Found";
            var position = player.transform.position;
            return $"{player.Name} Position: X:{position.x}, Y:{position.y}, Z:{position.z}";
        }
    }
}