using System.Text;
using System.Windows.Markup;
using System.Xml;
using MetaConfiguration.Model;

namespace MetaConfiguration.Common
{
	public class Serializer
	{
		public static string Serialize(ConfigurationContainer config)
		{
			return Method(config);
		}

		private static string Method(object obj)
		{
			var settings =
				new XmlWriterSettings
					{
						Indent = true,
						IndentChars = "\t",
						NewLineOnAttributes = true,
						ConformanceLevel = ConformanceLevel.Fragment,
					};

			var sb = new StringBuilder();
			var writer = XmlWriter.Create(sb, settings);
			var manager =
				new XamlDesignerSerializationManager(writer)
					{
						XamlWriterMode = XamlWriterMode.Expression
					};

			XamlWriter.Save(obj, manager);

			return sb.ToString();
		}

		public static ConfigurationContainer Deserialize(string settings)
		{
			return (ConfigurationContainer) XamlReader.Parse(settings);
		}
	}
}