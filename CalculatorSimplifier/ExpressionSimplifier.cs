using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace CalculatorSimplifier
{
    public static class ExpressionSimplifier
    {
        public static string Simplify(SequenceProcessor sequenceProcessor, bool applyDivision, bool applyRounding)
        {
            if (sequenceProcessor == null) throw new ArgumentNullException(nameof(sequenceProcessor));

            if (sequenceProcessor.Sequence.Length == 0)
            {
                Console.WriteLine("Empty string is not an expression.");
                return "<Error>";
            }

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            sequenceProcessor.Reset();
            IExpression result;
            try
            {
                result = ParseExpression(sequenceProcessor);
                sequenceProcessor.Reset();
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message + "\nCould not simplify an expression");
                return "<Error>";
            }

            sequenceProcessor.Reset();
            result.MergeChildren();
            return result.SimplifiedRepresentation(applyDivision, applyRounding);
        }

        private static ExpressionBlock ParseExpression(SequenceProcessor sequenceProcessor, int startingPriority = 0)
        {
            var currentPriority = startingPriority;
            ExpressionBlock currentExpressionBlock = null;
            IExpression currentNumber = null;
            Operation currentOperation = null;

            while (!ReadNextSequence(ref currentPriority, ref currentExpressionBlock,
                ref currentNumber, ref currentOperation, sequenceProcessor)) {}

            while (currentExpressionBlock.Parent != null)
            {
                currentExpressionBlock = currentExpressionBlock.Parent;
            }

            return currentExpressionBlock;
        }

        private static bool ReadNextSequence(ref int currentPriority, ref ExpressionBlock currentExpressionBlock,
            ref IExpression currentNumber, ref Operation currentOperation, SequenceProcessor sequenceProcessor)
        {
            var currentSequence = sequenceProcessor.ReadNextSequence();
            var isFinalSequence = false;

            ExpressionBlock newExpressionBlock;
            switch (sequenceProcessor.GetSequenceType(currentSequence))
            {
                case SequenceProcessor.SequenceType.Number:
                    currentNumber = double.TryParse(currentSequence, out var result)
                        ? new Number(result)
                        : new Number(currentSequence);

                    break;
                case SequenceProcessor.SequenceType.Operation:
                    currentOperation = Settings.GetOperation(currentSequence);
                    if (currentExpressionBlock == null)
                    {
                        currentExpressionBlock = new ExpressionBlock(currentPriority + currentOperation.Priority);
                        currentExpressionBlock.AppendNode(currentNumber);
                        currentExpressionBlock.AppendOperation(currentOperation);
                    }
                    else if (currentPriority + currentOperation.Priority > currentExpressionBlock.Priority)
                    {
                        newExpressionBlock = new ExpressionBlock(currentPriority + currentOperation.Priority);
                        newExpressionBlock.AppendNode(currentNumber);
                        newExpressionBlock.AppendOperation(currentOperation);
                        currentExpressionBlock.AppendNode(newExpressionBlock);
                        currentExpressionBlock = newExpressionBlock;
                    }
                    else if (currentPriority + currentOperation.Priority < currentExpressionBlock.Priority)
                    {
                        if (currentExpressionBlock.Parent != null)
                        {
                            currentExpressionBlock.AppendNode(currentNumber);
                            currentExpressionBlock = currentExpressionBlock.Parent;
                            currentExpressionBlock.AppendOperation(currentOperation);
                        }
                        else
                        {
                            newExpressionBlock = new ExpressionBlock(currentPriority + currentOperation.Priority);
                            currentExpressionBlock.AppendNode(currentNumber);
                            newExpressionBlock.AppendNode(currentExpressionBlock);
                            newExpressionBlock.AppendOperation(currentOperation);
                            currentExpressionBlock = newExpressionBlock;
                        }
                    }
                    else
                    {
                        currentExpressionBlock.AppendNode(currentNumber);
                        currentExpressionBlock.AppendOperation(currentOperation);
                    }

                    break;
                case SequenceProcessor.SequenceType.OpeningBracket:
                    newExpressionBlock =
                        ParseExpression(sequenceProcessor, currentPriority + Settings.ParenthesesPriority);

                    currentNumber = newExpressionBlock;
                    break;
                case SequenceProcessor.SequenceType.ClosingBracket:
                    isFinalSequence = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currentSequence),
                        "Sequence of this type cannot be processed.");
            }

            isFinalSequence = isFinalSequence || sequenceProcessor.CurrentIndex == sequenceProcessor.Sequence.Length;

            if (isFinalSequence)
            {
                currentExpressionBlock ??= new ExpressionBlock(currentNumber.IsAtomic
                    ? currentPriority
                    : currentNumber.Priority);
                if (!currentExpressionBlock.IsFilled)
                {
                    currentExpressionBlock.AppendNode(currentNumber);
                }
            }
            return isFinalSequence;
        }
    }
}