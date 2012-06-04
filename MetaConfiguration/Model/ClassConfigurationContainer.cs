using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Markup;

namespace MetaConfiguration.Model
{
	[ContentProperty("Properties")]
	public class ClassConfigurationContainer
	{
		/// <summary>
		/// Название секции\класса, содержащего настройки
		/// </summary>
		private string _name;
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Описание для коментариев
		/// </summary>
		private string _description;
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		/// <summary>
		/// Флаг показывающий, является ли класс настроек секцией
		/// </summary>
		private bool _isSection;
		[DefaultValue(false)]
		public bool IsSection
		{
			get { return _isSection; }
			set { _isSection = value; }
		}

		private readonly List<PropertyContainer> _properties = new List<PropertyContainer>();

		/// <summary>
		/// Набор секций, хранящий настройки для нашего приложения
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public List<PropertyContainer> Properties
		{
			get { return _properties; }
		}
	}
}