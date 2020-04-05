using System.Collections.Generic;
using System.Linq;

namespace CalculatorSimplifier
{
    public static class Settings
    {
        /// <summary>
        /// Maximum digits to leave after decimal point.
        /// </summary>
        public const int PrecisionDigits = 5;

        /// <summary>
        /// Maximum allowed priority for expression block.
        /// </summary>
        public const int MaximumPriority = 100;

        private static readonly (string, Operation)[] KeyOperationPairs =
        {
            ("+", new Addition()),
            ("-", new Subtraction()),
            ("*", new Multiplication()),
            ("/", new Division())
        };

        /// <summary>
        /// Priority of parentheses is always higher than priority of any operation.
        /// </summary>
        public static int ParenthesesPriority { get; } =
            KeyOperationPairs.Select(pair => pair.Item2.Priority).Max() + 1;

        public static IEnumerable<string> Keys { get; } = KeyOperationPairs.Select(pair => pair.Item1);

        public static bool MatchingOperationExists(string key) =>
            KeyOperationPairs.Any(pair => pair.Item1 == key);

        public static Operation GetOperation(string key) =>
            (from pair in KeyOperationPairs where pair.Item1 == key select pair.Item2)
            .FirstOrDefault();

        public static string GetKeyFromOperation(Operation operation) =>
            (from pair in KeyOperationPairs where pair.Item2.Equals(operation) select pair.Item1)
            .FirstOrDefault();
    }
}
