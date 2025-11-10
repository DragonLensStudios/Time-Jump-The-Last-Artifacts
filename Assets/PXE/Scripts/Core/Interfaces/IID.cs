using PXE.Core.SerializableTypes;

namespace PXE.Core.Interfaces
{
    /// <summary>
    ///  This interface represents the identity of the object.
    /// </summary>
    public interface IID
    {
        /// <summary>
        ///  Gets or sets the name of the object.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the id of the object.
        /// </summary>
        SerializableGuid ID { get; set; }

        /// <summary>
        ///  When Enabled, The ID will be able to be manually set.
        /// </summary>
        bool IsManualID { get; set; }

    }
}