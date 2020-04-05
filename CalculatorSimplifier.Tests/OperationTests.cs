using System;
using Xunit;

namespace CalculatorSimplifier.Tests
{
    public class OperationTests
    {
        [Theory]
        [InlineData(1, 0, 1)]
        [InlineData(10, 20, 30)]
        [InlineData(123.456, -123.456, 0)]
        [InlineData(double.NaN, 1, double.NaN)]
        public void AdditionCalculate_VariousInput(double lhs, double rhs, double expected)
        {
            Assert.Equal(expected, new Addition().Calculate(lhs, rhs), Settings.PrecisionDigits);
        }

        [Theory]
        [InlineData(1, 0, 1)]
        [InlineData(10, 20, -10)]
        [InlineData(123.456, 123.456, 0)]
        [InlineData(123.456, -234.567, 358.023)]
        [InlineData(double.NaN, 1, double.NaN)]
        public void SubtractionCalculate_VariousInput(double lhs, double rhs, double expected)
        {
            Assert.Equal(expected, new Subtraction().Calculate(lhs, rhs), Settings.PrecisionDigits);
        }

        [Theory]
        [InlineData(1, 0, 0)]
        [InlineData(2, 1, 2)]
        [InlineData(4, 8, 32)]
        [InlineData(-8, 3.5, -28)]
        [InlineData(double.NaN, 2, double.NaN)]
        [InlineData(double.NaN, 0, 0)]
        public void MultiplicationCalculate_VariousInput(double lhs, double rhs, double expected)
        {
            Assert.Equal(expected, new Multiplication().Calculate(lhs, rhs), Settings.PrecisionDigits);
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(2, 1, 2)]
        [InlineData(4, 8, 0.5)]
        [InlineData(-8, 5, -1.6)]
        [InlineData(0, 10, 0)]
        [InlineData(double.NaN, 2, double.NaN)]
        [InlineData(2, double.NaN, double.NaN)]
        public void DivisionCalculate_VariousInput_Valid(double lhs, double rhs, double expected)
        {
            Assert.Equal(expected, new Division().Calculate(lhs, rhs), Settings.PrecisionDigits);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(double.NaN, 0)]
        public void DivisionCalculate_VariousInput_Invalid(double lhs, double rhs)
        {
            Assert.Throws<DivideByZeroException>(() => new Division().Calculate(lhs, rhs));
        }
    }
}
