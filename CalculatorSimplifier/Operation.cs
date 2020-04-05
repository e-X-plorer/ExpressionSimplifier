using System;

namespace CalculatorSimplifier
{
    public abstract class Operation : IEquatable<Operation>
    {
        public abstract int Priority { get; }

        public abstract Operation ReverseOperation { get; }

        public abstract double Calculate(double leftOperand, double rightOperand);

        public bool Equals(Operation other) => other != null && GetType() == other.GetType();
    }
}