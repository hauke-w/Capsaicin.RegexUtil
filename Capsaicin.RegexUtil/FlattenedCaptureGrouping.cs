using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    public class FlattenedCaptureGrouping : IReadOnlyCollection<Capture?>, IFlattenedCaptureGrouping5
    {
        internal FlattenedCaptureGrouping(CaptureGroup captureGroup, Capture key, Capture?[] captures)
        {
            CaptureGroup = captureGroup;
            Key = key;
            Captures = captures;
        }

        public CaptureGroup CaptureGroup { get; }

        public Capture Key { get; }

        public Capture?[] Captures { get; }

        public int Count => Captures.Length;

        public IEnumerator<Capture?> GetEnumerator() => ((IEnumerable<Capture?>)Captures).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Deconstruct(out string? value1, out string? value2)
        {
            VerifyDeconstruct(2);
            value1 = Captures[0]?.Value;
            value2 = Captures[1]?.Value;
        }

        private void VerifyDeconstruct(int nValues)
        {
            if (Captures.Length < nValues)
            {
                throw new InvalidOperationException($"Cannot get {nValues} values for a capturing group with just {Captures.Length} value{(Captures.Length > 1 ? "s" : "")}.");
            }
        }

        public void Deconstruct(out string? value1, out string? value2, out string? value3)
        {
            VerifyDeconstruct(3);
            value1 = Captures[0]?.Value;
            value2 = Captures[1]?.Value;
            value3 = Captures[2]?.Value;
        }

        public void Deconstruct(out string? value1, out string? value2, out string? value3, out string? value4)
        {
            VerifyDeconstruct(4);
            value1 = Captures[0]?.Value;
            value2 = Captures[1]?.Value;
            value3 = Captures[2]?.Value;
            value4 = Captures[3]?.Value;
        }

        public void Deconstruct(out string? value1, out string? value2, out string? value3, out string? value4, out string? value5)
        {
            VerifyDeconstruct(5);
            value1 = Captures[0]?.Value;
            value2 = Captures[1]?.Value;
            value3 = Captures[2]?.Value;
            value4 = Captures[3]?.Value;
            value5 = Captures[4]?.Value;
        }

        public static implicit operator string?(FlattenedCaptureGrouping flattenedCaptureGrouping)
            => flattenedCaptureGrouping.Captures[0]?.Value;
    }
}
