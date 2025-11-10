namespace PXE.Core.Interfaces
{
    public interface IInitializable
    {
        bool IsInitialized { get; }
        void Initialize();
    }
}