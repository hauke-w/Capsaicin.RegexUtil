using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    public interface ICaptureGrouping : IGrouping<Capture, Capture?[]>
    {
        CaptureGroup Grouping { get; }
        ImmutableList<Capture?[]> Captures { get; }

        IParentCaptureGroup? Parent { get; }

        void Deconstruct(out Capture key, out ImmutableList<Capture?[]> captures);
    }
}