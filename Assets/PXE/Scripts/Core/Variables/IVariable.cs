using System;

namespace PXE.Core.Variables
{
    public interface IVariable
    {
        string Name { get; set; }
    }
    public interface IVariable<T> : IVariable, IComparable<IVariable<T>>, IEquatable<IVariable<T>>
    {
        T Value { get; set; }
        bool LessThan(IVariable<T> other);
        bool LessThanOrEqual(IVariable<T> other);
        bool GreaterThan(IVariable<T> other);
        bool GreaterThanOrEqual(IVariable<T> other);
    }
}

