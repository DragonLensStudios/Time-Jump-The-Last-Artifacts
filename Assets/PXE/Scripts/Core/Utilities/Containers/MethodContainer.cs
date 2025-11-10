using System;
using System.Reflection;

namespace PXE.Core.Utilities.Containers
{
    public class MethodContainer
    {
        public object Instance { get; set; }
        public MethodInfo Method { get; set; }
        public object[] Parameters { get; set; }

        public override string ToString()
        {
            try
            {
                object result;
                if (Parameters != null && Parameters.Length > 0)
                {
                    result = Method.Invoke(Instance, Parameters);
                }
                else
                {
                    result = Method.Invoke(Instance, null);
                }
                return Convert.ToString(result);
            }
            catch
            {
                // Handle any exceptions that may occur during the method invocation
                return "Method invocation failed";
            }
        }
    }
}

