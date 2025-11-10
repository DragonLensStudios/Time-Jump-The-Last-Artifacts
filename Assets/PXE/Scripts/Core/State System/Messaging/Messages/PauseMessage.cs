using JetBrains.Annotations;
using PXE.Core.Interfaces;

namespace PXE.Core.State_System.Messaging.Messages
{
    public struct PauseMessage
    {
        [CanBeNull] public IObject Source { get; }
        public bool IsPaused { get; }
        public bool ShowPauseMenu { get;  }
        
        public PauseMessage([CanBeNull] IObject source, bool isPaused, bool showPauseMenu = false)
        {
            Source = source;
            IsPaused = isPaused;
            ShowPauseMenu = showPauseMenu;
        }
    }
}