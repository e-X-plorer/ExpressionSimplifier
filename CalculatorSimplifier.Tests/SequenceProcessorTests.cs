using System;
using System.Diagnostics.Contracts;
using Xunit;

namespace CalculatorSimplifier.Tests
{
    public class SequenceProcessorTests
    {
        [Theory]
        [InlineData("123", "^[0-9]+$", "(", ")", SequenceProcessor.SequenceType.Number)]
        [InlineData("AbC", "^[a-zA-Z]+$", "(", ")", SequenceProcessor.SequenceType.Number)]
        [InlineData("1v3", "^[0-9a-z]+$", "(", ")", SequenceProcessor.SequenceType.Number)]
        [InlineData("+", "^[0-9a-zA-Z]+$", "(", ")", SequenceProcessor.SequenceType.Operation)]
        [InlineData("(", "^[0-9a-zA-Z]+$", "(", ")", SequenceProcessor.SequenceType.OpeningBracket)]
        [InlineData("bracket", "^[0-9]+$", "bracket", ")", SequenceProcessor.SequenceType.OpeningBracket)]
        [InlineData(")", "^[0-9a-zA-Z]+$", "(", ")", SequenceProcessor.SequenceType.ClosingBracket)]
        [InlineData("()()()", "^[0-9a-zA-Z]+$", "(", "()()()", SequenceProcessor.SequenceType.ClosingBracket)]
        [InlineData("1.2", "[0-9a-zA-Z]+", "(", ")", SequenceProcessor.SequenceType.Number)]
        [InlineData("1.2", "^[0-9a-zA-Z]+$", "(", ")", SequenceProcessor.SequenceType.Illegal)]
        [InlineData("()", "^[0-9a-zA-Z]+$", "(", ")", SequenceProcessor.SequenceType.Illegal)]
        [InlineData("A", "[0-9a-z]", "(", ")", SequenceProcessor.SequenceType.Illegal)]
        [InlineData("(", "^[0-9a-zA-Z]+$", "[", "]", SequenceProcessor.SequenceType.Illegal)]
        [InlineData("a01", "^[a-z]+$", "(", ")", SequenceProcessor.SequenceType.Illegal)]
        [InlineData("aaa", "^[a-z]$", "(", ")", SequenceProcessor.SequenceType.Illegal)]
        public void GetSequenceType_VariousInput_CorrectType(string input, string numberPattern, string openingBracket, string closingBracket,
            SequenceProcessor.SequenceType expected)
        {
            Assert.Equal(expected,
                new SequenceProcessor("", numberPattern, openingBracket, closingBracket).GetSequenceType(input));
        }

        [Fact]
        public void Init_ValidInput_CheckState()
        {
            var sequenceProcessor = new SequenceProcessor("123abc");

            Assert.Equal("123abc", sequenceProcessor.Sequence);
            Assert.Equal(0, sequenceProcessor.CurrentIndex);
            Assert.True(sequenceProcessor.BracketsValid);
        }

        [Theory]
        [InlineData("", "(", ")")]
        [InlineData("123", "", ")")]
        [InlineData("123", "(", "")]
        [InlineData("[0-9]+", "9", ")")]
        [InlineData("[a-z]+", "(", "abc")]
        [InlineData("^[*+-/]+$", "+-+", "(")]
        public void Init_InvalidInput_Exception(string numberPattern, string openingBracket, string closingBracket)
        {
            Assert.Throws<ArgumentException>(() =>
                new SequenceProcessor("", numberPattern, openingBracket, closingBracket));
        }

        [Fact]
        public void ReadNextSequence_ValidInput_String()
        {
            var sequenceProcessor = new SequenceProcessor("   123   + b +(  44");

            Assert.Equal("123", sequenceProcessor.ReadNextSequence());
            Assert.Equal("+", sequenceProcessor.ReadNextSequence());
            Assert.Equal("b", sequenceProcessor.ReadNextSequence());
            Assert.Equal("+", sequenceProcessor.ReadNextSequence());
            Assert.Equal("(", sequenceProcessor.ReadNextSequence());
            Assert.Equal("44", sequenceProcessor.ReadNextSequence());
        }

        [Theory]
        [InlineData("()")]
        [InlineData("+ 2")]
        [InlineData("(1 * (12 + 123) / )")]
        [InlineData("123 + < + 123")]
        [InlineData("2 (3 + 4)")]
        [InlineData("2 * (3 + 4) 3")]
        [InlineData("(1 + 1) (2 - 2)")]
        public void ReadNextSequence_InvalidInput_Exception(string input)
        {
            Assert.Throws<ArgumentException>(() => ReadFullExpression(new SequenceProcessor(input)));
        }

        [Fact]
        public void Reset_CallReset_StateReset()
        {
            var sequenceProcessor = new SequenceProcessor("(123456");
            sequenceProcessor.ReadNextSequence();
            sequenceProcessor.ReadNextSequence();

            sequenceProcessor.Reset();

            Assert.True(sequenceProcessor.BracketsValid);
            Assert.Equal(0, sequenceProcessor.CurrentIndex);
        }

        private void ReadFullExpression(SequenceProcessor sequenceProcessor)
        {
            while (sequenceProcessor.ReadNextSequence().Length > 0) {}
        }
    }
}
