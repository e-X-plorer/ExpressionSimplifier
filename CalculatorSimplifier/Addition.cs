namespace CalculatorSimplifier
{
    public class Addition : Operation
    {
        public override int Priority => 0;
        public override Operation ReverseOperation => Settings.GetOperation("-");

        public override double Calculate(double leftOperand, double rightOperand) => leftOperand + rightOperand;
    }
}
