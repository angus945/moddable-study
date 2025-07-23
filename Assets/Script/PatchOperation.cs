using System.Xml.Linq;
using System.Xml.XPath;

public abstract class PatchOperation
{
    public abstract void Apply(XDocument doc);
}

public class PatchOperationReplace : PatchOperation
{
    public string xpath;
    public XElement value;

    public override void Apply(XDocument doc)
    {
        foreach (XElement element in doc.XPathSelectElements(xpath))
        {
            element.ReplaceWith(new XElement(value));
        }
    }
}
public class PatchOperationAdd : PatchOperation
{
    public string xpath;
    public XElement value;
    public bool prepend = false;

    public override void Apply(XDocument doc)
    {
        foreach (XElement element in doc.XPathSelectElements(xpath))
        {
            if (prepend)
            {
                element.AddFirst(new XElement(value));
            }
            else
            {
                element.AddAfterSelf(new XElement(value));
            }
        }
    }
}