using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    public abstract class CaptureGroupBase
    {
        internal CaptureGroupBase(CaptureGroupingDefinitionBase definition, Capture[] key)
        {
            Definition = definition;
            Key = key.ToImmutableArray();
        }

        protected CaptureGroupingDefinitionBase Definition { get; }

        public ImmutableArray<Capture> Key { get; }

        private IParentCaptureGroup? _Parent;
        public IParentCaptureGroup? Parent => _Parent ?? (_Parent = EvaluateParent());

        private IParentCaptureGroup? EvaluateParent()
        {
            return Definition.Parent is null 
                ? null 
                : new ParentCaptureGroup(Definition.Parent, Key.Slice(..^1));
        }
    }
}
