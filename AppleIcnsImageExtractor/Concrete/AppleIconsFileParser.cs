using System.Text;
using MediaWidget.Core.Abstract;
using MediaWidget.Core.Models;
using MediaWidget.Core.Models.AppleIconsFile;

namespace MediaWidget.Core.Concrete;

public class AppleIconsFileParser : IAppleIconsFileParser
{
    private readonly IBinaryStreamReader binaryStreamReader;
    
    public AppleIconsFileParser(IBinaryStreamReader binaryStreamReader)
    {
        this.binaryStreamReader = binaryStreamReader;
    }
    
    public AppleIconsCollection Parse(byte[] bytes)
    {
        var result = new AppleIconsCollection();
        using (var stream = new MemoryStream(bytes))
        {
            binaryStreamReader.Stream = stream;
            var header = binaryStreamReader.ReadBytes(4);
            if (header.SequenceEqual(new byte[] { 0x69, 0x63, 0x6E, 0x73 }))
            {
                binaryStreamReader.ReadBytes(4); // file length
                var entryType = Encoding.ASCII.GetString(binaryStreamReader.ReadBytes(4));
                if (entryType == "TOC ")
                {
                    var len = binaryStreamReader.ReadInt32(Endianess.BigEndian);
                    if (len % 8 != 0)
                    {
                        throw new ApplicationException("Invalid TOC record in ICNS.");
                    }
                    
                    var recordCount = len / 8;
                    var recordInfo = new List<Tuple<string, int>>();
                    for (int i = 1; i <= recordCount; i++)
                    {
                        var iconType = Encoding.ASCII.GetString(binaryStreamReader.ReadBytes(4));
                        var iconLen = binaryStreamReader.ReadInt32(Endianess.BigEndian);
                        recordInfo.Add(new Tuple<string, int>(iconType, iconLen));
                    }

                    for (int i = 0; i < recordInfo.Count; i++)
                    {
                        var icon = new AppleIcon();
                        icon.IconType = recordInfo[i].Item1;
                        icon.Data = binaryStreamReader.ReadBytes(recordInfo[i].Item2);
                        if (IsSupported(icon.IconType))
                        {
                            SetIconMetrics(icon);
                            result.Add(icon);   
                        }
                    }
                }
            }
        }

        return result;
    }

    private bool IsSupported(string iconType)
    {
        switch (iconType)
        {
            case "icp6":
            case "ic07":
            case "ic08":
            case "ic09":
            case "ic10":
            case "ic11":
            case "ic12":
            case "ic13":
            case "ic14":
            case "icsB":
            case "sb24":
            case "SB24":
                return true;
            default:
                return false;
        }
    }

    private void SetIconMetrics(AppleIcon icon)
    {
        switch (icon.IconType)
        {
            case "icp6":
                icon.Height = 48;
                icon.Width = 48;
                break;
            case "ic07":
                icon.Height = 128;
                icon.Width = 128;
                break;
            case "ic08":
                icon.Height = 256;
                icon.Width = 256;
                break;
            case "ic09":
                icon.Height = 512;
                icon.Width = 512;
                break;
            case "ic10":
                icon.Height = 1024;
                icon.Width = 1024;
                break;
            case "ic11":
                icon.Height = 32;
                icon.Width = 32;
                break;
            case "ic12":
                icon.Height = 64;
                icon.Width = 64;
                break;
            case "ic13":
                icon.Height = 256;
                icon.Width = 256;
                break;
            case "ic14":
                icon.Height = 512;
                icon.Width = 512;
                break;
            case "icsB":
                icon.Height = 36;
                icon.Width = 36;
                break;
            case "sb24":
                icon.Height = 24;
                icon.Width = 24;
                break;
            case "SB24":
                icon.Height = 48;
                icon.Width = 48;
                break;
        }
        
        icon.Format = IdentifyFormat(icon.Data);
    }

    private AppleIconFormat IdentifyFormat(byte[] data)
    {
        if (data.Length > 4 && data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47)
        {
            return AppleIconFormat.Png;
        }
        
        if (data.Length > 3 && data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF)
        {
            return AppleIconFormat.Jpeg;
        }        

        return AppleIconFormat.Unknown;
    }
}