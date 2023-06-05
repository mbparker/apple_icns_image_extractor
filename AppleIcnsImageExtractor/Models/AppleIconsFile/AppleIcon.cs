namespace AppleIcnsImageExtractor.Models.AppleIconsFile;

public class AppleIcon
{
    public string IconType { get; set; }
    public AppleIconFormat Format { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }
    public byte[] Data { get; set; }
}