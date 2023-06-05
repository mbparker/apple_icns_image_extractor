using AppleIcnsImageExtractor.Models.AppleIconsFile;

namespace AppleIcnsImageExtractor.Abstract;

public interface IAppleIconsFileParser
{
    AppleIconsCollection Parse(byte[] bytes);
}