using System;

namespace PXE.Core.Variables
{
    /// <summary>
    /// Represents a generic variable with a name and a value.
    /// </summary>
    [Serializable]
    public class Variable<T> : IVariable<T> where T : IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// The name of the variable.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value of the variable.
        /// </summary>
        public T Value { get; set; }
        
        /// <summary>
        /// Gets the value of the Variable
        /// </summary>
        /// <returns><see cref="object"/></returns>
        public object GetValue()
        {
            return Value;
        }

        /// <summary>
        /// Initializes a new instance of the Variable class with the specified name and value.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The value of the variable.</param>
        public Variable(string name, T value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Compares the current variable with another variable.
        /// </summary>
        /// <param name="other">The variable to compare with.</param>
        /// <returns>An integer that indicates whether the current variable is less than, equal to, or greater than the other variable.</returns>
        public int CompareTo(IVariable<T> other)
        {
            if (other == null)
            {
                return 1;
            }

            return Value.CompareTo(other.Value);
        }

        /// <summary>
        /// Determines whether the current variable is equal to another variable.
        /// </summary>
        /// <param name="other">The variable to compare with.</param>
        /// <returns>True if the variables are equal; otherwise, false.</returns>
        public bool Equals(IVariable<T> other)
        {
            if (other == null)
            {
                return false;
            }

            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Determines whether the current variable is less than or equal to another variable.
        /// </summary>
        /// <param name="other">The variable to compare with.</param>
        /// <returns>True if the current variable is less than or equal to the other variable; otherwise, false.</returns>
        public bool LessThanOrEqual(IVariable<T> other)
        {
            if (other == null)
            {
                return false;
            }

            return Value.CompareTo(other.Value) <= 0;
        }

        /// <summary>
        /// Determines whether the current variable is greater than another variable.
        /// </summary>
        /// <param name="other">The variable to compare with.</param>
        /// <returns>True if the current variable is greater than the other variable; otherwise, false.</returns>
        public bool GreaterThan(IVariable<T> other)
        {
            if (other == null)
            {
                return false;
            }

            return Value.CompareTo(other.Value) > 0;
        }

        /// <summary>
        /// Determines whether the current variable is greater than or equal to another variable.
        /// </summary>
        /// <param name="other">The variable to compare with.</param>
        /// <returns>True if the current variable is greater than or equal to the other variable; otherwise, false.</returns>
        public bool GreaterThanOrEqual(IVariable<T> other)
        {
            if (other == null)
            {
                return false;
            }

            return Value.CompareTo(other.Value) >= 0;
        }

        /// <summary>
        /// Determines whether the current variable is less than another variable.
        /// </summary>
        /// <param name="other">The variable to compare with.</param>
        /// <returns>True if the current variable is less than the other variable; otherwise, false.</returns>
        public bool LessThan(IVariable<T> other)
        {
            if (other == null)
            {
                return false;
            }

            return Value.CompareTo(other.Value) < 0;
        }

        /// <summary>
        /// Determines whether the current variable is equal to another variable.
        /// </summary>
        /// <param name="other">The variable to compare with.</param>
        /// <returns>True if the variables are equal; otherwise, false.</returns>
        public bool EqualTo(IVariable<T> other)
        {
            if (other == null)
            {
                return false;
            }

            return Value.CompareTo(other.Value) == 0;
        }
        
        /// <summary>
        /// Determines whether the current variable is greater than or equal to the other variable.
        /// </summary>
        /// <param name="other">The variable to compare with.</param>
        /// <returns>True if the current variable is greater than or equal to the other variable; otherwise, false.</returns>
        public bool GreaterThanOrEqualTo(IVariable<T> other)
        {
            if (other == null)
            {
                return false;
            }

            return Value.CompareTo(other.Value) >= 0;
        }

        /// <summary>
        /// Determines whether the current variable is less than or equal to the other variable.
        /// </summary>
        /// <param name="other">The variable to compare with.</param>
        /// <returns>True if the current variable is less than or equal to the other variable; otherwise, false.</returns>
        public bool LessThanOrEqualTo(IVariable<T> other)
        {
            if (other == null)
            {
                return false;
            }

            return Value.CompareTo(other.Value) <= 0;
        }

        /// <summary>
        /// Determines whether the current variable is not equal to the other variable.
        /// </summary>
        /// <param name="other">The variable to compare with.</param>
        /// <returns>True if the variables are not equal; otherwise, false.</returns>
        public bool NotEqualTo(IVariable<T> other)
        {
            if (other == null)
            {
                return false;
            }

            return Value.CompareTo(other.Value) != 0;
        }
        
        /// <summary>
        /// Compares the current variable with another variable.
        /// </summary>
        /// <param name="other">The variable to compare with.</param>
        /// <returns>An integer that indicates whether the current variable is less than, equal to, or greater than the other variable.</returns>
        public int CompareTo(Variable<T> other)
        {
            if (other == null)
            {
                return 1;
            }

            return Value.CompareTo(other.Value);
        }

        /// <summary>
        /// Determines whether the current variable is equal to another variable.
        /// </summary>
        /// <param name="other">The variable to compare with.</param>
        /// <returns>True if the variables are equal; otherwise, false.</returns>
        public bool Equals(Variable<T> other)
        {
            if (other == null)
            {
                return false;
            }

            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Returns the value as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}

