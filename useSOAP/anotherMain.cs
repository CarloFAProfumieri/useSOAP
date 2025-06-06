using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace useSOAP
{
    class anotherMain
    {
        static async Task Main(string[] args)
        {
            CargaMedico medicoEnvelope = new CargaMedico(
                "clave123",
                101,
                "api-xyz",
                456,
                "153413",
                "Dr. Eggman"
            );
            Console.WriteLine(medicoEnvelope.getEnvelope());
            
            //int result = await AddAsync(5, 3);
            //Console.WriteLine("Result: " + result);
        }
        static async Task<int> AddAsync(int a, int b)
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
/* envelope de ejemplo obtenido de soapUI: 
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:wsl="https://www.santafe.gob.ar/labcentral/ws/WSLabCentralApi/">
   <soapenv:Header>
      <id_laboratorio>?</id_laboratorio>
      <claveestd>?</claveestd>
      <api_key>?</api_key>
   </soapenv:Header>
   <soapenv:Body>
      <wsl:cargaMedico>
         <medico_id>?</medico_id>
         <matricula>?</matricula>
         <nombre>?</nombre>
      </wsl:cargaMedico>
   </soapenv:Body>
</soapenv:Envelope>
 */ 
    public class CargaMedico
    {
        public string claveEstd { get; set; }
        public int idLaboratorio { get; set; }
        public string apiKey { get; set; }
        public int medicoId { get; set; }
        public string matricula { get; set; }
        public string nombre { get; set; }

        public CargaMedico(string claveEstd, int idLaboratorio, string apiKey, int medicoId, string matricula, string nombre)
        {
            this.claveEstd = claveEstd;
            this.idLaboratorio = idLaboratorio;
            this.apiKey = apiKey;
            this.medicoId = medicoId;
            this.matricula = matricula;
            this.nombre = nombre;
        }
            
        public string getEnvelope()
        {
            XNamespace envNamespace = "http://schemas.xmlsoap.org/soap/envelope/";
            XNamespace wsl = "https://www.santafe.gob.ar/labcentral/ws/WSLabCentralApi/";
            var soapEnvelope = new XDocument(
                    new XElement(envNamespace + "Envelope",
                        new XAttribute(XNamespace.Xmlns + "soapenv", envNamespace),
                        new XAttribute(XNamespace.Xmlns + "wsl", wsl),
                        new XElement(envNamespace + "Header",
                            new XElement("id_laboratorio", idLaboratorio),
                            new XElement("claveestd", claveEstd),
                            new XElement("api_key", apiKey)
                        ),
                        new XElement(envNamespace + "Body",
                            new XElement(wsl + "cargaMedico",
                                new XAttribute("medico_id", medicoId),
                                new XAttribute("matricula", matricula),
                                new XAttribute("nombre", nombre)
                            )
                        )
                    )
                );
            return soapEnvelope.ToString();
        }
    }
}