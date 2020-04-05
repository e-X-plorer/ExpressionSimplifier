using System;

namespace CalculatorSimplifier
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter your expression:");
            var input = Console.ReadLine();
            Console.WriteLine("Do you want to apply division? (type \"Y\" if you agree)");
            var applyDivision = Console.ReadLine() == "Y";
            Console.WriteLine("Do you want to apply rounding? (type \"Y\" if you agree)");
            var applyRounding = Console.ReadLine() == "Y";
            Console.WriteLine("Result: " +
                              ExpressionSimplifier.Simplify(new SequenceProcessor(input), applyDivision, applyRounding));
        }
    }
}
