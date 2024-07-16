using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CloudMining.Application.Services;

public static class CastService
{
    public static JObject CastXmlToJObject(string responseXmlContent)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(responseXmlContent);

        var jsonContent = JsonConvert.SerializeObject(xmlDoc);
        return JObject.Parse(jsonContent);
    }
    
}