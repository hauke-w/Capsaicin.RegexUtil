namespace Capsaicin.RegexUtil
{
    public interface IFlattenedCaptureGrouping2 : IFlattenedCaptureGrouping
    {
        void Deconstruct(out string? value1, out string? value2);
    }
}
