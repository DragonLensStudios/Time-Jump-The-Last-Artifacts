using System;
using System.Collections.Generic;
using PXE.Core.ScriptableObjects;
using UnityEngine;

namespace PXE.Core.Variables
{
    //TODO: Convert to a struct? and set fields to be properties with backing fields.
    /// <summary>
    /// Represents a ScriptableObject that serves as a container for different types of variables.
    /// </summary>
    [CreateAssetMenu(fileName = "New Variables", menuName = "PXE/Variables", order = 0)]
    public class VariablesObject : ScriptableObjectController, ISerializationCallbackReceiver
    {
        
        [SerializeField] private List<SerializedVariable<int>> serializedIntVariables = new();
        [SerializeField] private List<SerializedVariable<long>> serializedLongVariables = new();
        [SerializeField] private List<SerializedVariable<short>> serializedShortVariables = new();
        [SerializeField] private List<SerializedVariable<double>> serializedDoubleVariables = new();
        [SerializeField] private List<SerializedVariable<decimal>> serializedDecimalVariables = new();
        [SerializeField] private List<SerializedVariable<float>> serializedFloatVariables = new();
        [SerializeField] private List<SerializedVariable<bool>> serializedBoolVariables = new();
        [SerializeField] private List<SerializedVariable<string>> serializedStringVariables = new();
        [SerializeField] private List<SerializedVariable<ComparableVector2>> serializedVector2Variables = new();
        [SerializeField] private List<SerializedVariable<ComparableVector3>> serializedVector3Variables = new();
        [SerializeField] private List<SerializedVariable<DateTime>> serializedDateTimeVariables = new();
        
        
        private VariableContainer<int> _intVariables = new();
        private VariableContainer<long> _longVariables = new();
        private VariableContainer<short> _shortVariables = new();
        private VariableContainer<double> _doubleVariables = new();
        private VariableContainer<decimal> _decimalVariables = new();
        private VariableContainer<float> _floatVariables = new();
        private VariableContainer<bool> _boolVariables = new();
        private VariableContainer<string> _stringVariables = new();
        private VariableContainer<ComparableVector2> _vector2Variables = new();
        private VariableContainer<ComparableVector3> _vector3Variables = new();
        private VariableContainer<DateTime> _dateTimeVariables = new();

        [SerializeField] private string variableType;
        private List<string> _availableTypes;

        /// <summary>
        /// The container for integer variables.
        /// </summary>
        public VariableContainer<int> IntVariables => _intVariables;

        /// <summary>
        /// The container for long variables.
        /// </summary>
        public VariableContainer<long> LongVariables => _longVariables;

        /// <summary>
        /// The container for short variables.
        /// </summary>
        public VariableContainer<short> ShortVariables => _shortVariables;

        /// <summary>
        /// The container for double variables.
        /// </summary>
        public VariableContainer<double> DoubleVariables => _doubleVariables;

        /// <summary>
        /// The container for decimal variables.
        /// </summary>
        public VariableContainer<decimal> DecimalVariables => _decimalVariables;

        /// <summary>
        /// The container for float variables.
        /// </summary>
        public VariableContainer<float> FloatVariables => _floatVariables;

        /// <summary>
        /// The container for boolean variables.
        /// </summary>
        public VariableContainer<bool> BoolVariables => _boolVariables;

        /// <summary>
        /// The container for string variables.
        /// </summary>
        public VariableContainer<string> StringVariables => _stringVariables;
        public VariableContainer<ComparableVector2> Vector2Variables => _vector2Variables;
        public VariableContainer<ComparableVector3> Vector3Variables => _vector3Variables;

        /// <summary>
        /// The container for DateTime variables.
        /// </summary>
        public VariableContainer<DateTime> DateTimeVariables => _dateTimeVariables;

        /// <summary>
        /// The type of variables stored in the container.
        /// </summary>
        public string VariableType
        {
            get => variableType;
            set => variableType = value;
        }
        

        /// <summary>
        /// Indexer to access variables of type int by name.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        /// <returns>The value of the variable.</returns>
        public IVariable this[string variableName]
        {
            get
            {                
                return (((((((((_intVariables.Variables.Find(v => v.Name == variableName) ?? 
                             (IVariable)_longVariables.Variables.Find(v => v.Name == variableName)) ??
                                        _shortVariables.Variables.Find(v => v.Name == variableName)) ??
                                        _doubleVariables.Variables.Find(v => v.Name == variableName)) ??
                                        _decimalVariables.Variables.Find(v => v.Name == variableName)) ??
                                        _floatVariables.Variables.Find(v => v.Name == variableName)) ??
                                        _boolVariables.Variables.Find(v => v.Name == variableName)) ??
                                        _stringVariables.Variables.Find(v => v.Name == variableName)) ??
                                        _vector2Variables.Variables.Find(v => v.Name == variableName)) ??
                                        _vector3Variables.Variables.Find(v => v.Name == variableName)) ??
                                        _dateTimeVariables.Variables.Find(v => v.Name == variableName);;
            }
            set
            {
                switch (value)
                {
                    case IVariable<int> intValue:
                        SetValue(_intVariables.Variables, variableName, intValue.Value);
                        break;
                    case IVariable<long> longValue:
                        SetValue(_longVariables.Variables, variableName, longValue.Value);
                        break;
                    case IVariable<short> shortValue:
                        SetValue(_shortVariables.Variables, variableName, shortValue.Value);
                        break;
                    case IVariable<double> doubleValue:
                        SetValue(_doubleVariables.Variables, variableName, doubleValue.Value);
                        break;
                    case IVariable<decimal> decimalValue:
                        SetValue(_decimalVariables.Variables, variableName, decimalValue.Value);
                        break;
                    case IVariable<float> floatValue:
                        SetValue(_floatVariables.Variables, variableName, floatValue.Value);
                        break;
                    case IVariable<bool> boolValue:
                        SetValue(_boolVariables.Variables, variableName, boolValue.Value);
                        break;
                    case IVariable<string> stringValue:
                        SetValue(_stringVariables.Variables, variableName, stringValue.Value);
                        break;
                    case IVariable<ComparableVector2> vector2Value:
                        SetValue(_vector2Variables.Variables, variableName, vector2Value.Value);
                        break;
                    case IVariable<ComparableVector3> vector3Value:
                        SetValue(_vector3Variables.Variables, variableName, vector3Value.Value);
                        break;
                    case IVariable<DateTime> dateTimeValue:
                        SetValue(_dateTimeVariables.Variables, variableName, dateTimeValue.Value);
                        break;
                    default:
                        Debug.LogWarning($"Failed to set variable '{variableName}': Incompatible value type");
                        break;
                }
            }
        }
        public virtual void SetValue<T>(List<IVariable<T>> variableList, string variableName, T value) where T : IComparable<T>, IEquatable<T>
        {
            IVariable<T> variable = variableList.Find(v => v.Name == variableName);
            if (variable != null)
            {
                variable.Value = value;
            }
            else
            {
                variableList.Add(new Variable<T>(variableName, value));
            }
        }

        // Helper method to get the value of a variable
        public virtual T GetValue<T>(List<IVariable<T>> variableList, string variableName) where T : IComparable<T>, IEquatable<T>
        {
            IVariable<T> variable = variableList.Find(v => v.Name == variableName);
            if (variable != null)
            {
                return variable.Value;
            }

            Debug.LogWarning($"Failed to find variable '{variableName}'");
            return default;
        }
        
        
        public void OnBeforeSerialize()
        {
            serializedIntVariables = _intVariables.GetSerializedValues();
            serializedLongVariables = _longVariables.GetSerializedValues();
            serializedShortVariables = _shortVariables.GetSerializedValues();
            serializedDoubleVariables = _doubleVariables.GetSerializedValues();
            serializedDecimalVariables = _decimalVariables.GetSerializedValues();
            serializedFloatVariables = _floatVariables.GetSerializedValues();
            serializedBoolVariables = _boolVariables.GetSerializedValues();
            serializedStringVariables = _stringVariables.GetSerializedValues();
            serializedVector2Variables = _vector2Variables.GetSerializedValues();
            serializedVector3Variables = _vector3Variables.GetSerializedValues();
            serializedDateTimeVariables = _dateTimeVariables.GetSerializedValues();
        }

        public void OnAfterDeserialize()
        {
            _intVariables = new VariableContainer<int>();
            foreach (var value in serializedIntVariables)
            {
                _intVariables.Variables.Add(new Variable<int>(value.Name, value.Value));
            }

            _longVariables = new VariableContainer<long>();
            foreach (var value in serializedLongVariables)
            {
                _longVariables.Variables.Add(new Variable<long>(value.Name, value.Value));
            }

            _shortVariables = new VariableContainer<short>();
            foreach (var value in serializedShortVariables)
            {
                _shortVariables.Variables.Add(new Variable<short>(value.Name, value.Value));
            }

            _doubleVariables = new VariableContainer<double>();
            foreach (var value in serializedDoubleVariables)
            {
                _doubleVariables.Variables.Add(new Variable<double>(value.Name, value.Value));
            }

            _decimalVariables = new VariableContainer<decimal>();
            foreach (var value in serializedDecimalVariables)
            {
                _decimalVariables.Variables.Add(new Variable<decimal>(value.Name, value.Value));
            }

            _floatVariables = new VariableContainer<float>();
            foreach (var value in serializedFloatVariables)
            {
                _floatVariables.Variables.Add(new Variable<float>(value.Name, value.Value));
            }

            _boolVariables = new VariableContainer<bool>();
            foreach (var value in serializedBoolVariables)
            {
                _boolVariables.Variables.Add(new Variable<bool>(value.Name, value.Value));
            }

            _stringVariables = new VariableContainer<string>();
            foreach (var value in serializedStringVariables)
            {
                _stringVariables.Variables.Add(new Variable<string>(value.Name, value.Value));
            }

            _vector2Variables = new VariableContainer<ComparableVector2>();
            foreach (var value in serializedVector2Variables)
            {
                _vector2Variables.Variables.Add(new Variable<ComparableVector2>(value.Name, value.Value));
            }
            
            _vector3Variables = new VariableContainer<ComparableVector3>();
            foreach (var value in serializedVector3Variables)
            {
                _vector3Variables.Variables.Add(new Variable<ComparableVector3>(value.Name, value.Value));
            }
            
            _dateTimeVariables = new VariableContainer<DateTime>();
            foreach (var value in serializedDateTimeVariables)
            {
                _dateTimeVariables.Variables.Add(new Variable<DateTime>(value.Name, value.Value));
            }
        }

        public void Reset()
        {
            _intVariables = new VariableContainer<int>();
            _longVariables = new VariableContainer<long>();
            _shortVariables = new VariableContainer<short>();
            _doubleVariables = new VariableContainer<double>();
            _decimalVariables = new VariableContainer<decimal>();
            _floatVariables = new VariableContainer<float>();
            _boolVariables = new VariableContainer<bool>();
            _stringVariables = new VariableContainer<string>();
            _vector2Variables = new VariableContainer<ComparableVector2>();
            _vector3Variables = new VariableContainer<ComparableVector3>();
            _dateTimeVariables = new VariableContainer<DateTime>();
        }
    }
}
