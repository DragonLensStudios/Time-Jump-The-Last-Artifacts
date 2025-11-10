using JetBrains.Annotations;
using PXE.Core.Enums;

namespace PXE.Core.UI.Messaging.Messages
{
    public struct PageMessage
    {
        [CanBeNull] public Page Page { get; }
        public PageOperation PageOperation { get; }

        public PageMessage(PageOperation pageOperation, [CanBeNull] Page page = null)
        {
            PageOperation = pageOperation;
            Page = page;
        }
        
    }
}