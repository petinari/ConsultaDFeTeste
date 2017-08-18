using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConsultaDFe.ServiceReferenceDFe;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Xml.Linq;

namespace ConsultaDFe
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NFeDistribuicaoDFeSoapClient consDFe = new NFeDistribuicaoDFeSoapClient();
            
            consDFe.ClientCredentials.ClientCertificate.Certificate = ObterDoRepositorio();
            distDFeInt distDfe = new distDFeInt();
            distDfe.versao = TVerDistDFe.Item100;
            distDfe.tpAmb = TAmb.Item1;
            distDfe.cUFAutor = TCodUfIBGE.Item50;
            distDfe.ItemElementName = ItemChoiceType.CNPJ;
            distDfe.Item = "000000000000";
            distDFeIntDistNSU distNSU = new distDFeIntDistNSU();
            distNSU.ultNSU = "00000000000";
            distDfe.Item1 = distNSU;
            string xmlEnvio = SerializeToString(distDfe);
            var removeq1 = new string[] { ":q1", "q1:" };
            foreach (var item in removeq1)
            {
                xmlEnvio = xmlEnvio.Replace(item, string.Empty);
            }
            XElement xml = XElement.Parse(xmlEnvio);
            var asd = consDFe.nfeDistDFeInteresse(xml);
            textBox1.Text = asd.ToString();
           

        }
        public static X509Certificate2 ObterDoRepositorio()
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.OpenExistingOnly | OpenFlags.MaxAllowed);

            var collection = store.Certificates;
            var fcollection = collection.Find(X509FindType.FindByTimeValid, DateTime.Now, true);
            var scollection = X509Certificate2UI.SelectFromCollection(fcollection, "Certificados válidos:", "Selecione o certificado que deseja usar",
                X509SelectionFlag.SingleSelection);

            if (scollection.Count == 0)
            {
                throw new Exception("Nenhum certificado foi selecionado!");
            }

            store.Close();
            return scollection[0];
        }
        public static string SerializeToString(Object value)
        {
            var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(value.GetType());
            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, value, emptyNamepsaces);
                return stream.ToString();
            }
        }

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    byte[] buffer = Convert.FromBase64String(textBox1.Text);
        //    File.WriteAllBytes(@"c:\home\arquivo.rar", buffer);
        //}
    }
}
