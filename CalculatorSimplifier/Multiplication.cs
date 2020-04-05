using System;

namespace CalculatorSimplifier
{
    public class Multiplication : Operation
    {
        public override int Priority => 1;

        public override Operation ReverseOperation => Settings.GetOperation("/");

        public override double Calculate(double leftOperand, double rightOperand)
        {
            if (Math.Abs(leftOperand) < Math.Pow(0.1, Settings.PrecisionDigits) ||
                Math.Abs(rightOperand) < Math.Pow(0.1, Settings.PrecisionDigits))
            {
                return 0;
            }
            return leftOperand * rightOperand;
        }
    }
}
