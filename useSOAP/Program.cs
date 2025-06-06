using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
class Program
{
    static async Task MainTesting(string[] args)
    {
        int result = await SOAPAdd(5, 3);
        Console.WriteLine("Result: " + result);
    }
    static async Task<int> SOAPAdd(int a, int b)
    {
        XNamespace soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
        XNamespace tem = "http://tempuri.org/";
        var soapRequestXml = new XDocument(
            new XElement(soapenv + "Envelope",
                new XAttribute(XNamespace.Xmlns + "soapenv", soapenv),
                new XAttribute(XNamespace.Xmlns + "tem", tem),
                new XElement(soapenv + "Header"),
                new XElement(soapenv + "Body",
                    new XElement(tem + "Add",
                        new XElement(tem + "intA", a),
                        new XElement(tem + "intB", b)
                    )
                )
            )
        );
        string soapRequestString = soapRequestXml.Declaration + soapRequestXml.ToString();
        var httpClient = new HttpClient();
        var content = new StringContent(soapRequestString, Encoding.UTF8, "text/xml");

        content.Headers.Add("SOAPAction", "\"http://tempuri.org/Add\"");

        var response = await httpClient.PostAsync("http://www.dneonline.com/calculator.asmx", content);
        var result = await response.Content.ReadAsStringAsync();
        var documentoX = XDocument.Parse(result);
        var soapNs = XNamespace.Get("http://schemas.xmlsoap.org/soap/envelope/");
        var tempuriNs = XNamespace.Get("http://tempuri.org/");
        var addResult = documentoX
            .Element(soapNs + "Envelope")?
            .Element(soapNs + "Body")?
            .Element(tempuriNs + "AddResponse")?
            .Element(tempuriNs + "AddResult")?
            .Value;

        //Console.WriteLine("THE X DOC:::");
        //Console.WriteLine(documentoX.XPathSelectElement("AddResult"));
        //Console.WriteLine(addResult);
        //Console.WriteLine(result);
        return int.Parse(addResult);
    }
}