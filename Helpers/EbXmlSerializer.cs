using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ExpressBase.Objects.Helpers
{
    public class EbXmlSerializer
    {
		public T Deserialize<T>(string input) where T : class
		{
			XmlSerializer ser = new XmlSerializer(typeof(T));
			using (StringReader sr = new StringReader(input))
			{
				return (T)ser.Deserialize(sr);
			}
		}

		public string Serialize<T>(T ObjectToSerialize)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());
			using (StringWriter textWriter = new StringWriter())
			{
				xmlSerializer.Serialize(textWriter, ObjectToSerialize);
				return textWriter.ToString();
			}
		}
	}
	public class SamClass
	{
		public int id;
		public string name;
	}
}
