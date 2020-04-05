using System.Linq;
using Xunit;

namespace CalculatorSimplifier.Tests
{
    public class NumberTests
    {
        [Fact]
        public void Init_WithDouble_IsNotVariable()
        {
            var number = new Number(123);

            Assert.False(number.IsVariable);
            Assert.False(number.ContainsDivision);
            Assert.True(number.IsAtomic);
            Assert.Equal(number.Priority, IExpression.AtomicPriority);
            Assert.Equal(number.Nodes, Enumerable.Empty<IExpression>());
        }

        [Fact]
        public void Init_WithString_IsVariable()
        {
            var number = new Number("var");

            Assert.True(number.IsVariable);
            Assert.False(number.ContainsDivision);
            Assert.True(number.IsAtomic);
            Assert.Equal(IExpression.AtomicPriority, number.Priority);
            Assert.Equal(Enumerable.Empty<IExpression>(), number.Nodes);
        }

        [Fact]
        public void GetNumericValue_IsNotVariable_ValidNumberAlways()
        {
            var number = new Number(123);

            Assert.Equal(123, number.GetNumericValue(true), Settings.PrecisionDigits);
            Assert.Equal(123, number.GetNumericValue(false), Settings.PrecisionDigits);
        }

        [Fact]
        public void GetNumericValue_IsVariable_NaNAlways()
        {
            var number = new Number("var");

            Assert.Equal(double.NaN, number.GetNumericValue(true));
            Assert.Equal(double.NaN, number.GetNumericValue(false));
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void SimplifiedRepresentation_IsNotVariable_ValidAlways(bool applyDivision, bool applyRounding)
        {
            var number = new Number(123);

            Assert.Equal("123", number.SimplifiedRepresentation(applyDivision, applyRounding));
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void SimplifiedRepresentation_IsVariable_ValidAlways(bool applyDivision, bool applyRounding)
        {
            var number = new Number("var");

            Assert.Equal("var", number.SimplifiedRepresentation(applyDivision, applyRounding));
        }
    }
}
