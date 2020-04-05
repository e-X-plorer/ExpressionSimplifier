using System.Collections.Generic;

namespace CalculatorSimplifier
{
    /// <summary>
    /// Base class for expression tree nodes.
    /// </summary>
    public interface IExpression
    {
        const int AtomicPriority = int.MaxValue;

        /// <summary>
        /// True if this expression an atomic entity.
        /// </summary>
        bool IsAtomic { get; }

        /// <summary>
        /// True if this expression is a variable or contains it.
        /// </summary>
        bool IsVariable { get; }

        /// <summary>
        /// True if this expression contains division.
        /// </summary>
        bool ContainsDivision { get; }

        ExpressionBlock Parent { get; set; }

        /// <summary>
        /// Priority of this expression based on its operations and parentheses.
        /// </summary>
        int Priority { get; }

        IEnumerable<IExpression> Nodes { get; }

        string SimplifiedRepresentation(bool applyDivision, bool applyRounding);

        /// <param name="applyDivision">if division should be applied and calculated.</param>
        /// <returns>Value of this expression excluding variable parts.</returns>
        double GetNumericValue(bool applyDivision);

        void MergeChildren();
    }
}
