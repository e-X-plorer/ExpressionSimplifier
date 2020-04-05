using System.Collections.Generic;

namespace CalculatorSimplifier
{
    public interface IExpression
    {
        const int AtomicPriority = int.MaxValue;

        bool IsAtomic { get; }

        bool IsVariable { get; }

        bool ContainsDivision { get; }

        ExpressionBlock Parent { get; set; }

        int Priority { get; }

        IEnumerable<IExpression> Nodes { get; }

        string SimplifiedRepresentation(bool applyDivision, bool applyRounding);

        double GetNumericValue(bool applyDivision);

        void MergeChildren();
    }
}
