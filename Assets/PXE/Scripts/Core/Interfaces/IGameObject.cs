using PXE.Core.Enums;
using UnityEngine;

namespace PXE.Core.Interfaces
{
    /// <summary>
    ///  This interface represents the game object.
    /// </summary>
    public interface IGameObject : IObject, IGameObjectIdentity, IInitializable
    {
        /// <summary>
        /// Gets the game object.
        /// </summary>
        GameObject gameObject { get ; }
        
        /// <summary>
        ///  Gets the transform.
        /// </summary>
        Transform transform { get; }
        
        /// <summary>
        ///  Gets or sets the active state of the game object.
        /// </summary>
        bool IsActive { get; set; }

        void SetObjectActive(bool active);
        void UpdateActive();
        string SetGameObjectName();

        void UpdateComponentActive(Component comp);
        // bool IsControllerActive { get; set; }
        
        // InheritBoolType ControllerActiveType { get; set; }
        ActiveType ActiveType { get; set; }
        ChildrenActiveType ComponentActiveType { get; set; }

    }
}