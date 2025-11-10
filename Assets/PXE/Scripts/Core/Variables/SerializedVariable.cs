namespace PXE.Core.Variables
{
    //TODO: Convert to a struct? and set fields to be properties with backing fields.
    [System.Serializable]
    public class SerializedVariable<T>
    {
        public string Name;
        public T Value;

        public SerializedVariable(string name, T value)
        {
            Name = name;
            Value = value;
        }
    }
}