using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    public interface ICaptureGrouping1 : ICaptureGrouping
    {
        IEnumerable<string?> Values
            => Captures.Select(c => c[0]?.Value);
    }

    public interface ICaptureGrouping2 : ICaptureGrouping1
    {
        new IEnumerable<(string? Value1, string? Value2)> Values
            => Captures.Select(c => (c[0]?.Value, c[1]?.Value));
    }

    public interface ICaptureGrouping3 : ICaptureGrouping2
    {
        new IEnumerable<(string? Value1, string? Value2, string? Value3)> Values
            => Captures.Select(c => (c[0]?.Value, c[1]?.Value, c[2]?.Value));
    }

    public interface ICaptureGrouping4 : ICaptureGrouping3
    {
        new IEnumerable<(string? Value1, string? Value2, string? Value3, string? Value4)> Values
            => Captures.Select(c => (c[0]?.Value, c[1]?.Value, c[2]?.Value, c[3]?.Value));
    }

    public interface ICaptureGrouping5 : ICaptureGrouping4
    {
        new IEnumerable<(string? Value1, string? Value2, string? Value3, string? Value4, string? Value5)> Values
            => Captures.Select(c => (c[0]?.Value, c[1]?.Value, c[2]?.Value, c[3]?.Value, c[4]?.Value));
    }

    public class CaptureGrouping : ICaptureGrouping, ICaptureGrouping5
    {
        internal CaptureGrouping(CaptureGroup captureGroup, Capture key, IEnumerable<Capture?[]> captures)
        {
            Grouping = captureGroup;
            Key = key;
            Captures = captures.ToList();
        }

        public CaptureGroup Grouping { get; }

        public IParentCaptureGroup? Parent => Grouping.Parent;

        public Capture Key { get; }

        public List<Capture?[]> Captures { get; }

        public IEnumerator<Capture?[]> GetEnumerator() => Captures.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Deconstruct(out Capture key, out List<Capture?[]> captures)
        {
            key = Key;
            captures = Captures;
        }
    }
}
