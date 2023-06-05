using System.Collections;

namespace MediaWidget.Core.Models.AppleIconsFile;

public class AppleIconsCollection : IEnumerable<AppleIcon>
{
    private readonly List<AppleIcon> items = new List<AppleIcon>();
    
    public void Add(AppleIcon item)
    {
        items.Add(item);
    }
    
    public IEnumerator<AppleIcon> GetEnumerator()
    {
        return items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}