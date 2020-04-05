using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalculatorSimplifier
{
    public class ExpressionBlock : IExpression
    {
        private readonly List<Operation> _operations = new List<Operation>();

        private readonly List<IExpression> _nodes = new List<IExpression>();

        private readonly double _defaultValue;

        private readonly Operation _defaultOperation;

        public ExpressionBlock(int priority)
        {
            if (priority > Settings.MaximumPriority)
            {
                throw new ArgumentOutOfRangeException(nameof(priority),
                    "Cannot initialize expression block with priority this high. Try removing parentheses from initial string.");
            }
            switch (priority % Settings.ParenthesesPriority)
            {
                case 0:
                    _defaultValue = 0;
                    _defaultOperation = Settings.GetOperation("+");
                    break;
                case 1:
                    _defaultValue = 1;
                    _defaultOperation = Settings.GetOperation("*");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(priority), "Priority value passed is not supported.");
            }

            Priority = priority;
            _operations.Add(_defaultOperation);
        }

        public ExpressionBlock(int priority, IEnumerable<IExpression> nodes, IEnumerable<Operation> operations) :
            this(priority)
        {
            if (nodes == null) throw new ArgumentNullException(nameof(nodes));
            if (operations == null) throw new ArgumentNullException(nameof(operations));

            if (nodes.Count() != operations.Count() + 1)
            {
                throw new ArgumentException("");
            }
            _nodes.AddRange(nodes);
            _operations.AddRange(operations);
        }

        public bool IsAtomic => false;

        public bool IsVariable { get; private set; }

        public bool ContainsDivision { get; private set; }

        public ExpressionBlock Parent { get; set; }

        public int Priority { get; }

        public IEnumerable<IExpression> Nodes => _nodes;

        public IEnumerable<Operation> Operations => _operations;

        public string SimplifiedRepresentation(bool applyDivision, bool applyRounding)
        {
            var result = new StringBuilder();
            var position = 0;
            var emptyHead = true;
            string valueToInsert;
            for (var i = 0; i < _nodes.Count; i++)
            {
                if (_nodes[i].IsVariable || !applyDivision &&
                    (_nodes[i].ContainsDivision || _operations[i].GetType() == typeof(Division)))
                {
                    valueToInsert = ' ' + Settings.GetKeyFromOperation(_operations[i]) + ' ' +
                                    _nodes[i].SimplifiedRepresentation(applyDivision, applyRounding);
                    if (emptyHead && _operations[i].Equals(_defaultOperation))
                    {
                        result.Insert(0, valueToInsert);
                        emptyHead = false;
                        position = result.Length;
                        continue;
                    }
                    result.Append(' ' + Settings.GetKeyFromOperation(_operations[i]) + ' ' +
                                  _nodes[i].SimplifiedRepresentation(applyDivision, applyRounding));
                }
            }

            if (emptyHead || Math.Abs(GetNumericValue(applyDivision) - _defaultValue) > Math.Pow(0.1, Settings.PrecisionDigits))
            {
                var numericValue = applyRounding
                    ? Math.Round(GetNumericValue(applyDivision), Settings.PrecisionDigits, MidpointRounding.AwayFromZero)
                    : GetNumericValue(applyDivision);
                if (numericValue >= 0)
                {
                    valueToInsert = ' ' + Settings.GetKeyFromOperation(_defaultOperation) + ' ' + numericValue;
                }
                else if (_defaultOperation.Priority > 0 || Parent != null && Parent._defaultOperation.Priority > 0 && emptyHead)
                { 
                    valueToInsert = ' ' + Settings.GetKeyFromOperation(_defaultOperation) + " (" + numericValue + ')';
                }
                else if (!emptyHead)
                { 
                    valueToInsert = ' ' + Settings.GetKeyFromOperation(_defaultOperation.ReverseOperation) + ' ' +
                                    (-numericValue);
                }
                else
                { 
                    valueToInsert = ' ' + Settings.GetKeyFromOperation(_defaultOperation) + ' ' + numericValue;
                }

                result.Insert(position, valueToInsert);
            }

            result.Remove(0, 3);

            if (Parent != null && (IsVariable || !applyDivision && ContainsDivision) &&
                _defaultOperation.Priority <= Parent._defaultOperation.Priority && Priority > Parent.Priority)
            {
                result.Insert(0, '(');
                result.Append(')');
            }

            return result.ToString();
        }

        public double GetNumericValue(bool applyDivision)
        {
            var result = _defaultValue;
            var hasValue = false;

            for (var i = 0; i < _operations.Count; i++)
            {
                if (_nodes[i].IsVariable ||
                    !applyDivision && (_nodes[i].ContainsDivision || _operations[i].GetType() == typeof(Division)) ||
                    Math.Abs(_nodes[i].GetNumericValue(applyDivision) - _defaultValue) <
                    Math.Pow(0.1, Settings.PrecisionDigits))
                {
                    continue;
                }
                result = _operations[i].Calculate(result, _nodes[i].GetNumericValue(applyDivision));
                hasValue = true;
            }

            return hasValue ? result : _defaultValue;
        }

        public void MergeChildren()
        {
            foreach (var node in _nodes)
            {
                node.MergeChildren();
            }

            for (var i = 0; i < _nodes.Count; i++)
            {
                if (_nodes[i].Nodes.Count() == 1)
                {
                    _nodes[i] = _nodes[i].Nodes.ElementAt(0);
                    _nodes[i].Parent = this;
                }
            }
        }

        public bool IsFilled => _nodes.Count == _operations.Count;

        public void AppendNode(IExpression expression)
        {
            if (IsFilled)
            {
                throw new InvalidOperationException(
                    "Appending a node to filled expression is forbidden. Append an operation first.");
            }

            if (expression == this)
            {
                throw new InvalidOperationException("Appending a node to itself is forbidden.");
            }

            if (expression.Parent != null)
            {
                throw new InvalidOperationException("Cannot append a node which already has a parent.");
            }

            _nodes.Add(expression);
            expression.Parent = this;
            if (expression.IsVariable)
            {
                IsVariable = true;
            }

            if (expression.ContainsDivision)
            {
                ContainsDivision = true;
            }
        }

        public void AppendOperation(Operation operation)
        {
            if (_nodes.Count < _operations.Count)
            {
                throw new InvalidOperationException(
                    "Expression is not filled. Append a node before appending an operation.");
            }

            if (operation.GetType() == typeof(Division))
            {
                ContainsDivision = true;
            }
            _operations.Add(operation);
        }
    }
}
