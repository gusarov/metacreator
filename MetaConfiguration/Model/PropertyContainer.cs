using System.ComponentModel;

namespace MetaConfiguration.Model
{
	public class PropertyContainer
	{
		/// <summary>
		/// Имя свойства
		/// </summary>
		private string _name;

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Тип свойства. Строкой, потому как на уровне компиляции еще не будет таких классов, которые используюттся в ссылках.
		/// </summary>
		private string _type;

		public string Type
		{
			get { return _type; }
			set { _type = value; }
		}

		/// <summary>
		/// Флаг, показывающий коллекционное ли свойство
		/// Не может употребляться с простыми типами
		/// </summary>
		private bool _isCollection;

		[DefaultValue(false)]
		public bool IsCollection
		{
			get { return _isCollection; }
			set { _isCollection = value; }
		}
	}
}