using System;
using UnityEngine;

namespace PXE.Core.Variables
{
    //TODO: Convert to a struct and set fields to be properties with backing fields.
    [Serializable]
    public class ComparableVector2 : IComparable<ComparableVector2>, IEquatable<ComparableVector2>
    {
        [SerializeField]
        private float x;
        [SerializeField]
        private float y;

        public float X
        {
            get => x;
            set => x = value;
        }

        public float Y
        {
            get => y;
            set => y = value;
        }

        public Vector2 Value
        {
            get => new Vector2(x, y);
            set
            {
                x = value.x;
                y = value.y;
            }
        }

        public ComparableVector2(Vector2 value)
        {
            x = value.x;
            y = value.y;
        }

        public int CompareTo(ComparableVector2 other)
        {
            return Value.sqrMagnitude.CompareTo(other.Value.sqrMagnitude);
        }

        public bool Equals(ComparableVector2 other)
        {
            return Value == other.Value;
        }

        // Addition operator
        public static ComparableVector2 operator +(ComparableVector2 a, ComparableVector2 b)
        {
            return new ComparableVector2(a.Value + b.Value);
        }

        // Subtraction operator
        public static ComparableVector2 operator -(ComparableVector2 a, ComparableVector2 b)
        {
            return new ComparableVector2(a.Value - b.Value);
        }

        // Element-wise multiplication operator
        public static ComparableVector2 operator *(ComparableVector2 a, ComparableVector2 b)
        {
            return new ComparableVector2(new Vector2(a.x * b.x, a.y * b.y));
        }

        // Element-wise division operator
        public static ComparableVector2 operator /(ComparableVector2 a, ComparableVector2 b)
        {
            return new ComparableVector2(new Vector2(a.x / b.x, a.y / b.y));
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }
}