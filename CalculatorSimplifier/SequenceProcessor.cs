using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CalculatorSimplifier
{
    public class SequenceProcessor
    {
        private readonly string _numberPattern;

        private readonly string _openingBracketSequence;

        private readonly string _closingBracketSequence;

        private int _brackets;

        public SequenceProcessor(string sequence, string numberPattern = "^[0-9a-zA-Z]+$", string openingBracket = "(",
            string closingBracket = ")")
        {
            if (numberPattern.Length == 0 || openingBracket.Length == 0 || closingBracket.Length == 0)
            {
                throw new ArgumentException(
                    "All strings used to create SequenceProcessor instance must contain at least one character.");
            }
            if (Regex.IsMatch(openingBracket, numberPattern) || Regex.IsMatch(closingBracket, numberPattern) ||
                Settings.Keys.Any(key => Regex.IsMatch(key, numberPattern)))
            {
                throw new ArgumentException(
                    "Number pattern is ambiguous as it matches other special character sequences.", nameof(numberPattern));
            }

            _numberPattern = numberPattern;
            _openingBracketSequence = openingBracket;
            _closingBracketSequence = closingBracket;
            CurrentIndex = 0;
            _brackets = 0;
            Sequence = GetPreparedInput(sequence);
        }

        public enum SequenceType
        {
            Number,
            Operation,
            OpeningBracket,
            ClosingBracket,
            Illegal
        }

        public string Sequence { get; }

        public int CurrentIndex { get; private set; }

        public bool BracketsValid => _brackets == 0;

        public IEnumerable<SequenceType> IllegalSequences { get; private set; } = new List<SequenceType>
        {SequenceType.ClosingBracket, SequenceType.Operation, SequenceType.Illegal};

        public SequenceType GetSequenceType(string entry)
        {
            if (Settings.MatchingOperationExists(entry))
            {
                return SequenceType.Operation;
            }

            if (entry == _openingBracketSequence)
            {
                return SequenceType.OpeningBracket;
            }

            if (entry == _closingBracketSequence)
            {
                return SequenceType.ClosingBracket;
            }

            return Regex.IsMatch(entry, _numberPattern) ? SequenceType.Number : SequenceType.Illegal;
        }

        public string ReadNextSequence()
        {
            while (CurrentIndex < Sequence.Length && Sequence[CurrentIndex] == ' ')
            {
                CurrentIndex++;
            }

            var currentSequence = Sequence.Substring(CurrentIndex,
                (Sequence.IndexOf(' ', CurrentIndex) == -1
                    ? Sequence.Length
                    : Sequence.IndexOf(' ', CurrentIndex)) - CurrentIndex);

            CurrentIndex += currentSequence.Length;

            var currentSequenceType = GetSequenceType(currentSequence);
            if (currentSequence.Length > 0 && IllegalSequences.Contains(currentSequenceType))
            {
                throw new ArgumentException("Incorrect expression format.");
            }

            if (currentSequenceType == SequenceType.OpeningBracket)
            {
                _brackets++;
            }
            else if (currentSequenceType == SequenceType.ClosingBracket)
            {
                _brackets--;
            }

            IllegalSequences = GetNextIllegalSequences(currentSequenceType).ToList();

            return currentSequence;
        }

        public void Reset()
        {
            CurrentIndex = 0;
            _brackets = 0;
        }

        private static IEnumerable<SequenceType> GetNextIllegalSequences(SequenceType currentSequenceType)
        {
            return currentSequenceType switch
            {
                SequenceType.Operation => new[]
                    {SequenceType.ClosingBracket, SequenceType.Operation, SequenceType.Illegal},
                SequenceType.Number => new[]
                    {SequenceType.OpeningBracket, SequenceType.Number, SequenceType.Illegal},
                SequenceType.OpeningBracket => new[]
                    {SequenceType.ClosingBracket, SequenceType.Operation, SequenceType.Illegal},
                SequenceType.ClosingBracket => new[]
                    {SequenceType.OpeningBracket, SequenceType.Number, SequenceType.Illegal},
                _ => new[] { SequenceType.Illegal }
            };
        }

        private string GetPreparedInput(string input) =>
            Settings.Keys.Concat(new[] { _openingBracketSequence, _closingBracketSequence })
                .Aggregate(input, (current, s) => current.Replace(s, $" {s} ")).Trim();
    }
}
