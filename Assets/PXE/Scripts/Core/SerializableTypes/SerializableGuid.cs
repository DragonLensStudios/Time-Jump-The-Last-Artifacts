using System;
using UnityEngine;

namespace PXE.Core.SerializableTypes
{
    /// <summary>
    ///  Represents the SerializableGuid.
    /// </summary>
    [Serializable]
    public class SerializableGuid : IEquatable<SerializableGuid>
    {
        [SerializeField]
        protected string guidString;

        /// <summary>
        ///  Gets or sets the Guid of the SerializableGuid.
        /// </summary>
        public Guid Guid
        {
            get { return !string.IsNullOrEmpty(guidString) ? new Guid(guidString) : Guid.Empty; }
            set
            {
                guidString = value.ToString();
            }
        }
        
        
        /// <summary>
        ///  Constructs a new SerializableGuid with the specified Guid.
        /// </summary>
        /// <param name="guid"></param>
        public SerializableGuid(Guid guid)
        {
            Guid = guid;
        }
        
        /// <summary>
        /// Returns whether the SerializableGuids are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as SerializableGuid);
        }
        
        /// <summary>
        ///  Returns whether the SerializableGuids are equal.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(SerializableGuid other)
        {
            return other != null && Guid.Equals(other.Guid);
        }
        
        /// <summary>
        ///  Returns the hash code of the SerializableGuid.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }
        
        /// <summary>
        ///  Returns whether the SerializableGuids are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(SerializableGuid left, SerializableGuid right)
        {
            return left?.Equals(right) ?? ReferenceEquals(right, null);
        }

        /// <summary>
        ///  Returns whether the SerializableGuids are not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(SerializableGuid left, SerializableGuid right)
        {
            return !(left == right);
        }

        /// <summary>
        ///  Returns the string representation of the SerializableGuid.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return guidString;
        }

        /// <summary>
        /// Creates a new SerializableGuid with a unique value.
        /// </summary>
        public static SerializableGuid CreateNew => new(Guid.NewGuid());
        
        public static SerializableGuid Empty => new(Guid.Empty);

        /// <summary>
        ///  Returns whether the SerializableGuid is empty.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static bool IsEmpty(SerializableGuid guid)
        {
            return guid == null || guid.Guid == Guid.Empty;
        }
        
        public static bool IsEmpty(Guid guid)
        {
            return guid == Guid.Empty;
        }
        
        public static bool IsEmpty(string guid)
        {
            return string.IsNullOrEmpty(guid) || guid == Guid.Empty.ToString();
        }
    }
}