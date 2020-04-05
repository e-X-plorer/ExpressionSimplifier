using System;
using Xunit;

namespace CalculatorSimplifier.Tests
{
    public class ExpressionSimplifierTests
    {
        [Theory]
        [InlineData("(a * (10 + 5)) / ((2 + 2) * 3) + b * (3 + 5) + 2 * 2", "(a * 15) * 0.08333 + 4 + b * 8")]
        [InlineData("1", "1")]
        [InlineData("0", "0")]
        [InlineData("a", "a")]
        [InlineData("((5))", "5")]
        [InlineData("((a)) + ((b))", "a + b")]
        [InlineData("n / 2 + 4", "n * 0.5 + 4")]
        [InlineData("4-x*9-23", "-19 - x * 9")]
        [InlineData("a * 3 + b * 2 + 0", "a * 3 + b * 2")]
        [InlineData("1 * a + b * 1 * 1 + 0", "a + b")]
        [InlineData("(a * 4 + 2) * 2 - 20", "(a * 4 + 2) * 2 - 20")]
        [InlineData("((a * 3 - 2) / 7 + 2 - (b * 10 - 6) / 8)", "(a * 3 - 2) * 0.14286 + 2 - (b * 10 - 6) * 0.125")]
        [InlineData("((( (( a + 3 - 8 )) / ( 1 + 1 / 2 ) )))", "(a - 5) * 0.66667")]
        [InlineData("7 / (11 - 13) / 11 / c * 14", "(-4.45455) / c")]
        [InlineData("5 * 15 / 6 * (9 - 8) + 15", "27.5")]
        [InlineData("8 / (6 + 6) * a * 11 / b - 8 - (11 - c) * (d * 7 * 7)", "a * 7.33333 / b - 8 - (11 - c) * (d * 49)")]
        public void Simplify_ApplyDivisionApplyRounding_CorrectOutput(string input, string expectedOutput)
        {
            Assert.Equal(expectedOutput, ExpressionSimplifier.Simplify(new SequenceProcessor(input), true, true));
        }

        [Theory]
        [InlineData("(a * (10 + 5)) / ((2 + 2) * 3) + b * (3 + 5) + 2 * 2", "(a * 15) / 12 + 4 + b * 8")]
        [InlineData("1", "1")]
        [InlineData("0", "0")]
        [InlineData("a", "a")]
        [InlineData("((a)) + ((b))", "a + b")]
        [InlineData("n / 2 + 4", "n / 2 + 4")]
        [InlineData("4-x*9-23", "-19 - x * 9")]
        [InlineData("a * 3 + b * 2 + 0", "a * 3 + b * 2")]
        [InlineData("1 * a + b * 1 * 1 + 0", "a + b")]
        [InlineData("((a * 3 - 2) / 7 + 2 - (b * 10 - 6) / 8)", "(a * 3 - 2) / 7 + 2 - (b * 10 - 6) / 8")]
        [InlineData("((( (( a + 3 - 8 )) / ( 1 + 1 / 2 ) )))", "(a - 5) / (1 / 2 + 1)")]
        [InlineData("7 / (11 - 13) / 11 / c * 14", "98 / (-2) / 11 / c")]
        [InlineData("5 * 15 / 6 * (9 - 8) + 15", "75 / 6 + 15")]
        [InlineData("8 / (6 + 6) * a * 11 / b - 8 - (11 - c) * (d * 7 * 7)", "a / 12 * 88 / b - 8 - (11 - c) * (d * 49)")]
        public void Simplify_NoDivisionApplyRounding_CorrectOutput(string input, string expectedOutput)
        {
            Assert.Equal(expectedOutput, ExpressionSimplifier.Simplify(new SequenceProcessor(input), false, true));
        }

        [Theory]
        [InlineData("")]
        [InlineData("()")]
        [InlineData("+ 2")]
        [InlineData("(1 * (12 + 123) / )")]
        [InlineData("123 + < + 123")]
        [InlineData("2 (3 + 4)")]
        [InlineData("2 * (3 + 4) 3")]
        [InlineData("(1 + 1) (2 - 2)")]
        [InlineData("((1 + 1 + 1)")]
        public void Simplify_InvalidInput_Exception(string input)
        {
            Assert.Equal("<Error>", ExpressionSimplifier.Simplify(new SequenceProcessor(input), true, true));
        }

        [Fact]
        public void Simplify_SequenceProcessorNull_Exception()
        {
            Assert.Throws<ArgumentNullException>(() => ExpressionSimplifier.Simplify(null, true, true));
        }
    }
}
