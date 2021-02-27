using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    /// <summary>
    /// Capture group with one key capture. That means this is not nested.
    /// </summary>
    public interface ICaptureGroup1 : ICaptureGroup
    {
        /// <summary>
        /// Gets the last capture in <see cref="ICaptureGroupBase.Key"/>.
        /// </summary>
        new Capture Key { get; }
    }
}
