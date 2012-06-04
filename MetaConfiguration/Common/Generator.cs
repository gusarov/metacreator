using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MetaConfiguration.Model;

namespace MetaConfiguration.Common
{
	public class GeneratorConfiguration
	{
		public static string Generate(ConfigurationContainer configurationContainer)
		{
			var stringBuilder = new StringBuilder();
			foreach (var classConfig in configurationContainer.Sections)
			{
				if (classConfig.IsSection)
				{
					GenerateSection(stringBuilder, classConfig);
				}
				else
				{
					GenerateConfigurationElement(stringBuilder, classConfig);
				}
			}

			foreach (var classConfig in configurationContainer.Sections)
			{
				foreach (var property in classConfig.Properties.Where(x => x.IsCollection))
				{
					GenerateElementCollection(stringBuilder, property.Type);
				}
			}

			return stringBuilder.ToString();
		}

		private static void GenerateElementCollection(StringBuilder stringBuilder, string type)
		{
			const string pattern =
				@"
	[ConfigurationCollection(typeof(@ELEMENT_TYPE@))]
	public sealed class @ELEMENT_COLLECTION_CLASS_NAME@ : ConfigurationElementCollection
	{
		public @ELEMENT_COLLECTION_CLASS_NAME@()
		{
		}

		#region Properties

		public override ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.BasicMap;
			}
		}

		public @ELEMENT_TYPE@ this[int index]
		{
			get
			{
				return (@ELEMENT_TYPE@)BaseGet(index);
			}
			set
			{
				if (BaseGet(index) != null)
				{
					BaseRemoveAt(index);
				}

				BaseAdd(index, value);
			}
		}

		public new @ELEMENT_TYPE@ this[string name]
		{
			get
			{
				return (@ELEMENT_TYPE@)BaseGet(name);
			}
		}

		protected override string ElementName
		{
			get
			{
				return ""provider"";
			}
		}

		#endregion

		protected override ConfigurationElement CreateNewElement()
		{
			return new @ELEMENT_TYPE@();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			throw new Exception();
			//return ((ProviderConfigurationElement)element).Name;
		}
	}
";

			stringBuilder.Append(
				pattern
					.Replace("@ELEMENT_COLLECTION_CLASS_NAME@", GetCollectionTypeName(type))
					.Replace("@ELEMENT_TYPE@", type));
		}

		private static void GenerateSection(StringBuilder stringBuilder, ClassConfigurationContainer classConfig)
		{
			const string pattern =
				@"@CONFIGURATION_DESCRIPTION@
	public sealed class @CONFIGURATION_ELEMENT_NAME@ : ConfigurationSection
	{
		// public const string ConfigurationElementName = ""@CONFIGURATION_ELEMENT_NAME_LOWER@"";

		/// <summary>
		/// Initializes a new instance of the <see cref=""@CONFIGURATION_ELEMENT_NAME@""/> class.
		/// </summary>
		public @CONFIGURATION_ELEMENT_NAME@()
		{
		}
		
		#region Properties

@PROPERTIES@

		#endregion
	}
";

			stringBuilder.Append(
				pattern
					.Replace("@CONFIGURATION_DESCRIPTION@", GetConfigurationDescription(classConfig.Description))
					.Replace("@CONFIGURATION_ELEMENT_NAME@", GetConfigurationElementName(classConfig.Name))
					.Replace("@CONFIGURATION_ELEMENT_NAME_LOWER@", classConfig.Name.ToLowerCase())
					.Replace("@PROPERTIES@", GetProperiesString(classConfig.Properties))
				);
		}

		private static void GenerateConfigurationElement(StringBuilder stringBuilder, ClassConfigurationContainer classConfig)
		{
			const string pattern =
				@"@CONFIGURATION_DESCRIPTION@
	public sealed class @CONFIGURATION_ELEMENT_NAME@ : ConfigurationElement
	{
		// public const string ConfigurationElementName = ""@CONFIGURATION_ELEMENT_NAME_LOWER@"";

		/// <summary>
		/// Initializes a new instance of the <see cref=""@CONFIGURATION_ELEMENT_NAME@""/> class.
		/// </summary>
		public @CONFIGURATION_ELEMENT_NAME@()
		{
		}
		
		#region Properties

@PROPERTIES@

		#endregion
	}
";

			stringBuilder.Append(
				pattern
					.Replace("@CONFIGURATION_DESCRIPTION@", GetConfigurationDescription(classConfig.Description))
					.Replace("@CONFIGURATION_ELEMENT_NAME@", GetConfigurationElementName(classConfig.Name))
					.Replace("@CONFIGURATION_ELEMENT_NAME_LOWER@", classConfig.Name.ToLowerCase())
					.Replace("@PROPERTIES@", GetProperiesString(classConfig.Properties))
				);
		}

		private static string GetProperiesString(IEnumerable<PropertyContainer> properties)
		{
			var stringBuilder = new StringBuilder();

			foreach (var propertyContainer in properties)
			{
				if (propertyContainer.IsCollection)
				{
					// Коллекционное свойство
					// Напоминаение: не может быть простым (Пока)
					const string patternCollectionProperty =
						@"
		[ConfigurationProperty(""@PROPERTY_NAME_LOWER@"")]
		public @COLLECTION_TYPE_NAME@ @PROPERTY_NAME@
		{
			get
			{
				return (@COLLECTION_TYPE_NAME@)this[""@PROPERTY_NAME_LOWER@""];
			}
		}";
					stringBuilder.Append(
						patternCollectionProperty
							.Replace("@PROPERTY_NAME_LOWER@", propertyContainer.Name.ToLowerCase())
							.Replace("@COLLECTION_TYPE_NAME@", GetCollectionTypeName(propertyContainer.Type))
							.Replace("@PROPERTY_NAME@", propertyContainer.Name)
						);
				}
				else
				{
					const string patternProperty =
						@"
		[ConfigurationProperty(""@PROPERTY_NAME_LOWER@"")]
		public @PROPERTY_TYPE@ @PROPERTY_NAME@
		{
			get
			{
				return (@PROPERTY_TYPE@)this[""@PROPERTY_NAME_LOWER@""];
			}
			set
			{
				this[""@PROPERTY_NAME_LOWER@""] = value;
			}
		}";
					stringBuilder.Append(
						patternProperty
							.Replace("@PROPERTY_NAME_LOWER@", propertyContainer.Name.ToLowerCase())
							.Replace("@PROPERTY_TYPE@", propertyContainer.Type)
							.Replace("@PROPERTY_NAME@", propertyContainer.Name));
				}
			}

			return stringBuilder.ToString();
		}

		#region Help Pattern

		private static string GetCollectionTypeName(string type)
		{
			return type + "ElementCollection";
		}

		private static string GetConfigurationDescription(string description)
		{
			return !string.IsNullOrEmpty(description)
			       	? string.Format(@"
	/// <summary>
	/// {0}.
	/// </summary>", description)
			       	: string.Empty;
		}

		private static string GetConfigurationElementName(string name)
		{
			// Возможно стилистически правильно будет писать с суфиксом "ConfigurationElement".
			// Тогда просто добавляем позле названиея эту строку.
			return name;
		}

		#endregion
	}
}