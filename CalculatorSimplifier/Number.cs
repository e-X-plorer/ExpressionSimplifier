﻿using System.Collections.Generic;
using System.Linq;

namespace CalculatorSimplifier
{
    public class Number : IExpression
    {
        private readonly double _number;

        private readonly string _name;

        public Number(string name)
        {
            IsVariable = true;
            this._name = name;
        }

        public Number(double value)
        {
            IsVariable = false;
            _number = value;
        }

        public ExpressionBlock Parent { get; set; }

        public bool IsAtomic => true;

        public bool IsVariable { get; }

        public bool ContainsDivision => false;

        public int Priority => IExpression.AtomicPriority;

        public IEnumerable<IExpression> Nodes { get; } = Enumerable.Empty<IExpression>();

        public string SimplifiedRepresentation(bool applyDivision, bool applyRounding) =>
            IsVariable ? _name : _number.ToString();

        public double GetNumericValue(bool applyDivision) => IsVariable ? double.NaN : _number;

        public void MergeChildren() {}

        public override string ToString()
        {
            return IsVariable ? _name : _number.ToString();
        }
    }
}
