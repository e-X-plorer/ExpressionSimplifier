using System;
using Xunit;

namespace CalculatorSimplifier.Tests
{
    public class SettingsTests
    {
        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        [InlineData("*")]
        [InlineData("/")]
        public void MatchingOperationExists_AllSupportedOperations_True(string identifier)
        {
            Assert.True(Settings.MatchingOperationExists(identifier));
        }

        [Theory]
        [InlineData("^")]
        [InlineData("(")]
        [InlineData(" ")]
        [InlineData("++")]
        public void MatchingOperationExists_UnsupportedOperations_False(string identifier)
        {
            Assert.False(Settings.MatchingOperationExists(identifier));
        }

        [Theory]
        [InlineData("+", typeof(Addition))]
        [InlineData("-", typeof(Subtraction))]
        [InlineData("*", typeof(Multiplication))]
        [InlineData("/", typeof(Division))]
        [InlineData("^", null)]
        public void GetOperation_AllSupportedOperations_OperationType(string key, Type expected)
        {
            Assert.Equal(expected, Settings.GetOperation(key)?.GetType());
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        [InlineData("*")]
        [InlineData("/")]
        public void GetKeyFromOperation_SupportedOperations_Key(string key)
        {
            Assert.Equal(key, Settings.GetKeyFromOperation(Settings.GetOperation(key)));
        }
    }
}
