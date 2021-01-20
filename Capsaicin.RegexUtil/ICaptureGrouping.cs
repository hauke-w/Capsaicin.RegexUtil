using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    public interface ICaptureGrouping : IGrouping<Capture, Capture?[]>
    {
        CaptureGroup Grouping { get; }
        List<Capture?[]> Captures { get; }

        IParentCaptureGroup? Parent { get; }

        void Deconstruct(out Capture key, out List<Capture?[]> captures);
    }
}