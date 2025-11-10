namespace PXE.Core.Enums
{
    /// <summary>
    /// Represents mathematical operators that can be used in variable operations.
    /// </summary>
    public enum Operator
    {
        Add,                // Addition operator (+)
        Subtract,           // Subtraction operator (-)
        Multiply,           // Multiplication operator (*)
        Divide,             // Division operator (/)
        Set,                // Assignment operator (=)
        EqualTo,            // Equality comparison operator (==)
        NotEqualTo,         // Inequality comparison operator (!=)
        GreaterThan,        // Greater than comparison operator (>)
        GreaterThanOrEqual, // Greater than or equal to comparison operator (>=)
        LessThan,           // Less than comparison operator (<)
        LessThanOrEqual     // Less than or equal to comparison operator (<=)
    }
}