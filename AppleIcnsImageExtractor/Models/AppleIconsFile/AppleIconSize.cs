using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MediaWidget.Core.Models.AppleIconsFile;

[JsonConverter(typeof(StringEnumConverter))]
public enum AppleIconSize
{
    Large,
    Small
}