using System;
using System.Reflection;

namespace PXE.Core.Utilities.Containers
{
    public class FieldContainer
    {
        public object Instance { get; set; }
        public FieldInfo Field { get; set; }

        public object GetValue()
        {
            return Field.GetValue(Instance);
        }

        public void SetValue(object value)
        {
            Field.SetValue(Instance, value);
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
                return "Failed to retrieve field value";
            }
        }
    }
}

