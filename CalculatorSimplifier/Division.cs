using System;

namespace CalculatorSimplifier
{
    public class Division : Operation
    {
        public override int Priority => 1;

        public override Operation ReverseOperation => Settings.GetOperation("*");

        public override double Calculate(double leftOperand, double rightOperand)
        {
            if (Math.Abs(rightOperand) < Math.Pow(0.1, Settings.PrecisionDigits))
            {
                throw new DivideByZeroException("An attempt to divide by zero is spotted! Expression could not be simplified.");
            }

            return leftOperand / rightOperand;
        }
    }
}