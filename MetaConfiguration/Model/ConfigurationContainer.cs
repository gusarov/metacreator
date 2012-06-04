using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Markup;

namespace MetaConfiguration.Model
{
	[ContentProperty("Sections")]
	public class ConfigurationContainer
	{
		private readonly List<ClassConfigurationContainer> _sections = new List<ClassConfigurationContainer>();

		/// <summary>
		/// Набор секций, хранящий настройки для нашего приложения
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public List<ClassConfigurationContainer> Sections
		{
			get { return _sections; }
		}
	}
}