using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    public interface ICaptureGroupBase
    {
        Capture[] Key { get; }

        Capture? First(GroupSpecifier group);

        IParentCaptureGroup? Parent { get; }
    }
}
