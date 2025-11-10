using System;
using System.Collections.Generic;

namespace PXE.Core.Variables
{
    /// <summary>
    /// A container class for storing and managing variables of type T.
    /// </summary>
    /// <typeparam name="T">The type of variables stored in the container. Must implement <see cref="IComparable{T}"/> and <see cref="IEquatable{T}"/>.</typeparam>
    [Serializable]
    public class VariableContainer<T> where T : IComparable<T>, IEquatable<T>
    {
        private List<IVariable<T>> _variables = new List<IVariable<T>>();

        /// <summary>
        /// The list of variables stored in the container.
        /// </summary>
        public List<IVariable<T>> Variables
        {
            get => _variables;
            set => _variables = value;
        }
        
        /// <summary>
        /// Gets or sets the variable with the specified name.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        /// <returns>The variable with the specified name.</returns>
        public IVariable<T> this[string variableName]
        {
            get
            {
                IVariable<T> variable = _variables.Find(x => x.Name == variableName);
                if (variable != null)
                    return variable;

                return default;
            }
            set
            {
                IVariable<T> variable = _variables.Find(x => x.Name == variableName);
                if (variable != null)
                {
                    variable.Value = value.Value;
                }
                else
                {
                    Variable<T> newVariable = new Variable<T>(variableName, value.Value);
                    _variables.Add(newVariable);
                }
            }
        }
        
        /// <summary>
        /// Default constructor for the VariableContainer class.
        /// </summary>
        public VariableContainer()
        {
            _variables = new List<IVariable<T>>();
        }

        /// <summary>
        /// Gets the variable with the specified name.
        /// </summary>
        /// <param name="variableName">The name of the variable to retrieve.</param>
        /// <returns>The variable with the specified name, or null if not found.</returns>
        public IVariable<V> GetVariable<V>(string variableName)
        {
            return (IVariable<V>)_variables.Find(x => x.Name == variableName);
        }

        /// <summary>
        /// Adds a new variable to the container with the specified name and value.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The value of the variable.</param>
        public void AddVariable(string name, T value)
        {
            Variable<T> variable = new Variable<T>(name, value);
            _variables.Add(variable);
        }

        /// <summary>
        /// Removes the variable with the specified name from the container.
        /// </summary>
        /// <param name="name">The name of the variable to remove.</param>
        public void RemoveVariable(string name)
        {
            IVariable<T> variable = _variables.Find(x => x.Name == name);
            if (variable != null)
                _variables.Remove(variable);
        }

        /// <summary>
        /// Gets the serialized values of the variables in the container.
        /// </summary>
        /// <returns>A list of serialized values of the variables.</returns>
        public List<SerializedVariable<T>> GetSerializedValues()
        {
            List<SerializedVariable<T>> serializedValues = new List<SerializedVariable<T>>();

            foreach (Variable<T> variable in Variables)
            {
                serializedValues.Add(new SerializedVariable<T>(variable.Name, variable.Value));
            }

            return serializedValues;
        }

        /// <summary>
        /// Sets the values of the variables in the container using the serialized values.
        /// </summary>
        /// <param name="serializedValues">The list of serialized values of the variables.</param>
        public void SetSerializedValues(List<T> serializedValues)
        {
            _variables.Clear();
            foreach (T value in serializedValues)
            {
                Variable<T> variable = new Variable<T>(string.Empty, value);
                _variables.Add(variable);
            }
        }
        
    }
}