using MediaWidget.Core.Models.AppleIconsFile;

namespace MediaWidget.Core.Abstract;

public interface IAppleIconsFileParser
{
    AppleIconsCollection Parse(byte[] bytes);
}