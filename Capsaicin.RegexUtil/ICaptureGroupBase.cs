using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    public interface ICaptureGroupBase
    {
        ImmutableArray<Capture> Key { get; }

        Capture? First(GroupSpecifier group);

        IParentCaptureGroup? Parent { get; }
    }
}
