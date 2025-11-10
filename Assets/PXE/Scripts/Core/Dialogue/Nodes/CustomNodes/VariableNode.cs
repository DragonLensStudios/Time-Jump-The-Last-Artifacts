using System;
using PXE.Core.Dialogue.xNode.Scripts;
using PXE.Core.Enums;
using PXE.Core.Variables;
using UnityEngine;

namespace PXE.Core.Dialogue.Nodes.CustomNodes
{
    /// <summary>
    /// Represents a variable node in a dialogue graph.
    /// </summary>
    public class VariableNode : BaseNode
    {
        [Input] public Connection Input;
        [Output] public Connection ExitTrue;
        [Output] public Connection ExitFalse;
        public string VariableName;
        public VariableType VariableType;
        public string SerializedVariableValue;
        public Operator OperatorType;

        protected object variableValue;

        /// <summary>
        /// Gets or sets the value of the variable associated with this node.
        /// </summary>
        public object VariableValue
        {
            get
            {
                switch (VariableType)
                {
                    case VariableType.Int:
                        int.TryParse(SerializedVariableValue, out int intValue);
                        return intValue;
                    case VariableType.Long:
                        long.TryParse(SerializedVariableValue, out long longValue);
                        return longValue;
                    case VariableType.Short:
                        short.TryParse(SerializedVariableValue, out short shortValue);
                        return shortValue;
                    case VariableType.Double:
                        double.TryParse(SerializedVariableValue, out double doubleValue);
                        return doubleValue;
                    case VariableType.Decimal:
                        decimal.TryParse(SerializedVariableValue, out decimal decimalValue);
                        return decimalValue;
                    case VariableType.Float:
                        float.TryParse(SerializedVariableValue, out float floatValue);
                        return floatValue;
                    case VariableType.Bool:
                        bool.TryParse(SerializedVariableValue, out bool boolValue);
                        return boolValue;
                    case VariableType.String:
                        return SerializedVariableValue;
                    case VariableType.Vector2:
                        string[] vector2Values = SerializedVariableValue.Split(',');
                        if (vector2Values.Length == 2 && float.TryParse(vector2Values[0], out float x) && float.TryParse(vector2Values[1], out float y))
                        {
                            return new ComparableVector2(new Vector2(x, y));
                        }
                        return null;
                    case VariableType.Vector3:
                        string[] vector3Values = SerializedVariableValue.Split(',');
                        if (vector3Values.Length == 3 && float.TryParse(vector3Values[0], out float x3) && float.TryParse(vector3Values[1], out float y3) && float.TryParse(vector3Values[2], out float z))
                        {
                            return new ComparableVector3(new Vector3(x3, y3, z));
                        }
                        return null;
                    case VariableType.DateTime:
                        DateTime.TryParse(SerializedVariableValue, out DateTime dateTimeValue);
                        return dateTimeValue;
                    default:
                        return null;
                }
            }
            set
            {
                switch (VariableType)
                {
                    case VariableType.Int:
                        SerializedVariableValue = ((int)value).ToString();
                        break;
                    case VariableType.Long:
                        SerializedVariableValue = ((long)value).ToString();
                        break;
                    case VariableType.Short:
                        SerializedVariableValue = ((short)value).ToString();
                        break;
                    case VariableType.Double:
                        SerializedVariableValue = ((double)value).ToString();
                        break;
                    case VariableType.Decimal:
                        SerializedVariableValue = ((decimal)value).ToString();
                        break;
                    case VariableType.Float:
                        SerializedVariableValue = ((float)value).ToString();
                        break;
                    case VariableType.Bool:
                        SerializedVariableValue = ((bool)value).ToString();
                        break;
                    case VariableType.String:
                        SerializedVariableValue = (string)value;
                        break;
                    case VariableType.Vector2:
                        ComparableVector2 vector2Value = (ComparableVector2)value;
                        SerializedVariableValue = $"{vector2Value.Value.x},{vector2Value.Value.y}";
                        break;
                    case VariableType.Vector3:
                        ComparableVector3 vector3Value = (ComparableVector3)value;
                        SerializedVariableValue = $"{vector3Value.Value.x},{vector3Value.Value.y},{vector3Value.Value.z}";
                        break;
                    case VariableType.DateTime:
                        SerializedVariableValue = ((DateTime)value).ToString("o"); // ISO 8601 format
                        break;
                }
            }
        }

        /// <summary>
        /// Returns the value of the specified output port.
        /// </summary>
        /// <param name="port">The output port to retrieve the value from.</param>
        /// <returns>The value of the output port.</returns>
        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}
