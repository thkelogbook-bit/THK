using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Elogbook.Core.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Serialize<T>(T data)
        {
            try
            {
                using (var stringwriter = new StringWriter())
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(stringwriter, data);

                    return stringwriter.ToString();
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlText"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string xmlText)
        {
            try
            {
                using (var stringReader = new StringReader(xmlText))
                {
                    var serializer = new XmlSerializer(typeof(T));

                    return (T)serializer.Deserialize(stringReader);
                }
            }
            catch (Exception)
            {
                return default;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeToUtf8String<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));

            using (var memoryStream = new MemoryStream())
            {
                var settings = new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    Indent = true,
                };
                using (var xmlWriter = XmlWriter.Create(memoryStream, settings))
                {
                    xmlWriter.WriteStartDocument();
                    serializer.Serialize(xmlWriter, obj);
                }
                memoryStream.Seek(0L, SeekOrigin.Begin);

                using (var reader = new StreamReader(memoryStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utf8Xml"></param>
        /// <returns></returns>
        public static string ConvertToUtf16(string utf8Xml)
        {
            using (var ms = new MemoryStream())
            {
                var xDoc = XDocument.Parse(utf8Xml);

                if (xDoc.Declaration == null)
                {
                    xDoc.Declaration = new XDeclaration("1.0", "utf-8", "no");
                }

                xDoc.Declaration.Encoding = "utf-16";

                using (var sWriter = new StringWriter())
                {
                    xDoc.Save(sWriter);
                    return sWriter.GetStringBuilder().ToString();
                }
            }
        }

        /// <summary>
        /// ConvertMemoryStreamToString
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static string ConvertMemoryStreamToString(Stream ms)
        {
            if (ms == null)
            {
                return string.Empty;
            }

            using (ms)
            {
                ms.Position = 0;

                using (var reader = new StreamReader(ms))
                {
                    string text = reader.ReadToEnd();

                    return text;
                }
            }
        }

        /// <summary>
        /// ReadFile
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string ReadFile(string fileName)
        {
            string text;

            try
            {
                if (!File.Exists(fileName))
                {
                    Exception exception = new Exception("Cannot read the file '" + fileName + "' because it does not exist!");
                    throw exception;
                }

                text = File.ReadAllText(fileName);
            }
            catch (Exception ex)
            {
                Exception exception = new Exception("ReadFile() failed with error: " + ex.Message);
                throw exception;
            }

            return text;
        }

        /// <summary>
        /// ConvertStringToXmlReader
        /// </summary>
        /// <param name="strContent"></param>
        /// <returns></returns>
        public static XmlReader ConvertStringToXmlReader(string strContent)
        {
            using (var stringReader = new StringReader(strContent))
            {
                var xmlReader = XmlReader.Create(stringReader);
                return xmlReader;
            }
        }
    }
}
