using System.Collections.Generic;

namespace Capsaicin.RegexUtil
{
    public interface ICaptureGroup : ICaptureGroupBase
    {
        NestedCaptureGroupingDefinition GroupNestedBy(GroupSpecifier group);
        CaptureGrouping Into(params GroupSpecifier[] toSelect);
        IEnumerable<string> Select(GroupSpecifier toSelect);
        IEnumerable<string?[]> Select(params GroupSpecifier[] toSelect);
    }
}
