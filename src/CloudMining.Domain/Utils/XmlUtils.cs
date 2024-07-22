using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CloudMining.Domain.Utils;

public static class XmlUtils
{
    public static JObject ToJson(string xmlContent)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlContent);

        var jsonContent = JsonConvert.SerializeObject(xmlDoc);
        return JObject.Parse(jsonContent);
    }
}