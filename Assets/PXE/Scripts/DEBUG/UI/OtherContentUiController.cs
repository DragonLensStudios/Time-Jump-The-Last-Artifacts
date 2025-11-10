using PXE.Core.Enums;
using PXE.Core.Levels.Messaging.Messages;
using PXE.Core.Messaging;
using PXE.Core.Objects;

namespace PXE.DEBUG.UI
{
    public class OtherContentUiController : ObjectController
    {
        /// <summary>
        ///  Executes the OtherContentUiController method and calls level reset message to reset the level.
        /// </summary>
        public virtual void ResetLevel()
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelResetMessage());
            DebugManager.Instance.ShowDebugMenu(false);
        }
        
    }
}