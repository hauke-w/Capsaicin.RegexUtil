using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    public interface ICaptureGrouping1 : ICaptureGrouping, IEnumerable<string?>
    {
        new IEnumerator<string?> GetEnumerator()
            => Captures.Select(c => c[0]?.Value).GetEnumerator();

        IEnumerator<string?> IEnumerable<string?>.GetEnumerator()
            => GetEnumerator();
    }

    public interface ICaptureGrouping2 : ICaptureGrouping1
    {
        new IEnumerator<(string? Value1,string? Value2)> GetEnumerator()
            => Captures.Select(c => (c[0]?.Value, c[1]?.Value)).GetEnumerator();
    }

    public interface ICaptureGrouping3 : ICaptureGrouping2
    {
        new IEnumerator<(string? Value1, string? Value2, string? Value3)> GetEnumerator()
            => Captures.Select(c => (c[0]?.Value, c[1]?.Value, c[2]?.Value)).GetEnumerator();
    }

    public interface ICaptureGrouping4 : ICaptureGrouping3
    {
        new IEnumerator<(string? Value1, string? Value2, string? Value3, string? Value4)> GetEnumerator()
            => Captures.Select(c => (c[0]?.Value, c[1]?.Value, c[2]?.Value, c[3]?.Value)).GetEnumerator();
    }

    public interface ICaptureGrouping5 : ICaptureGrouping4
    {
        new IEnumerator<(string? Value1, string? Value2, string? Value3, string? Value4, string? Value5)> GetEnumerator()
            => Captures.Select(c => (c[0]?.Value, c[1]?.Value, c[2]?.Value, c[3]?.Value, c[4]?.Value)).GetEnumerator();
    }

    public class CaptureGrouping : ICaptureGrouping, ICaptureGrouping5
    {
        internal CaptureGrouping(CaptureGroup captureGroup, Capture key, IEnumerable<Capture?[]> captures)
        {
            Grouping = captureGroup;
            Key = key;
            Captures = captures;
        }

        public CaptureGroup Grouping { get; }

        public IParentCaptureGroup? Parent => Grouping.Parent;

        public Capture Key { get; }
        public IEnumerable<Capture?[]> Captures { get; }

        public IEnumerator<Capture?[]> GetEnumerator() => Captures.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Deconstruct(out Capture key, out List<Capture?[]> captures)
        {
            key = Key;
            captures = Captures.ToList();
        }
    }
}
