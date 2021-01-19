using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    public interface IFlattenedCaptureGrouping : IReadOnlyCollection<Capture?>
    {
        CaptureGroup CaptureGroup { get; }
        Capture?[] Captures { get; }
        Capture Key { get; }

        IParentCaptureGroup? Parent => CaptureGroup.Parent;
    }
}
