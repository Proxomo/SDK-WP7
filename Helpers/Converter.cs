using System;
using System.Xml.Serialization;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json; // Had to add some new ones up here! Clean up?

namespace Proxomo
{
    internal class Converter
    {

        public static string Convert<t>(t obj, CommunicationType format, bool indent = false)
        {
            if (format == CommunicationType.XML)
            {
                return ToXML<t>(obj, indent);
            }
            else if (format == CommunicationType.JSON)
            {
                return ToJSON<t>(obj);
            }
            else
            {
                return string.Empty;
            }

        }

        private static string ToXML<t>(t obj, bool indent = false)
        {

            System.Xml.Serialization.XmlSerializerNamespaces ns = new System.Xml.Serialization.XmlSerializerNamespaces();
            XmlSerializer xs = new XmlSerializer(typeof(t));
            StringBuilder sbuilder = new StringBuilder();
            var xmlws = new System.Xml.XmlWriterSettings() { OmitXmlDeclaration = true, Indent = indent };

            ns.Add(string.Empty, string.Empty);

            using (var writer = System.Xml.XmlWriter.Create(sbuilder, xmlws))
            {
                xs.Serialize(writer, obj, ns);
            }

            string result = sbuilder.ToString();

            ns = null;
            xs = null;
            sbuilder = null;
            xmlws = null;

            return result;

        }

        private static string ToJSON<t>(t obj)
        {

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(t));
            using (MemoryStream ms = new MemoryStream())
            {
                string result = string.Empty;
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

                ser.WriteObject(ms, obj);
                result = encoding.GetString(ms.ToArray(), 0, ms.ToArray().Length);

                ms.Close();
                encoding = null;
                ser = null;

                return result;
            }
        }

    }
}