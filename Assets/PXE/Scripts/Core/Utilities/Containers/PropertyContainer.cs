using System;
using System.Reflection;

namespace PXE.Core.Utilities.Containers
{
    public class PropertyContainer
    {
        public object Instance { get; set; }
        public PropertyInfo Property { get; set; }

        public object GetValue()
        {
            return Property.GetValue(Instance);
        }

        public void SetValue(object value)
        {
            Property.SetValue(Instance, value);
        }

        public override string ToString()
        {
            try
            {
                object value = GetValue();
                return Convert.ToString(value);
            }
            catch
            {
                // Handle any exceptions that may occur during the value retrieval
                return "Failed to retrieve property value";
            }
        }
    }
}

