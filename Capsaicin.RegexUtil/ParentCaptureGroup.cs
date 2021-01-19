using System.Text.RegularExpressions;

namespace Capsaicin.RegexUtil
{
    internal class ParentCaptureGroup : CaptureGroupBase, IParentCaptureGroup
    {
        // TODO: implement ICaptureGroup? This would make interface IParentCaptureGroup obsolete

        public ParentCaptureGroup(CaptureGroupingDefinitionBase definition, Capture[] key)
            : base(definition, key)
        {
        }

        public Capture? First(GroupSpecifier group)
        {
            var root = Definition.Root;
            var groupObj = group.GetGroup(root.Match);
            var key = Key[^1];

            int max = key.Index + key.Length;

            for (var i = 0; i < groupObj.Captures.Count; i++)
            {
                var capture = groupObj.Captures[i];
                if (capture.Index > max)
                {
                    break;
                }
                else if (capture.Index>= key.Index)
                {
                    return capture;
                }
            }
            return null;
        }
    }
}
