using System.Collections.Generic;
using System.Xml.Serialization;

namespace SignalRSwaggerGen.Utils.XmlComments
{
	[XmlRoot("doc")]
	public class XmlComments
	{
		[XmlElement("assembly")]
		public AssemblyElement Assembly { get; set; }

		[XmlArray("members")]
		[XmlArrayItem("member")]
		public List<MemberElement> Members { get; set; } = new List<MemberElement>();
	}

	public class AssemblyElement
	{
		[XmlElement("name")]
		public AssemblyNameElement Name { get; set; }
	}

	public class AssemblyNameElement
	{
		[XmlText]
		public string Text { get; set; }
	}

	public class MemberElement
	{
		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlElement("summary")]
		public MemberSummaryElement Summary { get; set; }

		[XmlElement("param")]
		public List<ParamElement> Params { get; set; } = new List<ParamElement>();
	}

	public class MemberSummaryElement
	{
		[XmlText]
		public string Text { get; set; }
	}

	public class ParamElement
	{
		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlText]
		public string Text { get; set; }
	}
}
