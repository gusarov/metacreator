using System;
using System.Collections.Generic;
using System.Text;

namespace MetaCreator
{
	public interface IMetaWriter
	{
		void Write(string msg, params object[] args);
		void WriteLine(string msg, params object[] args);
		void Write(string msg);
		void WriteLine(string msg);
		void Write(object obj);
		void WriteLine(object obj);
		void WriteLine();

		string GetResult();
	}

	internal class DefaultMetaWriter : IMetaWriter
	{
		readonly StringBuilder _result = new StringBuilder();

		public void Write(string msg, params object[] args)
		{
			_result.AppendFormat(msg, args);
		}

		public void WriteLine(string msg, params object[] args)
		{
			_result.AppendFormat(msg + Environment.NewLine, args);
		}

		public void Write(string msg)
		{
			_result.Append(msg);
		}

		public void WriteLine(string msg)
		{
			_result.AppendLine(msg);
		}

		public void Write(object obj)
		{
			_result.Append(obj == null ? string.Empty : obj.ToString());
		}

		public void WriteLine(object obj)
		{
			_result.AppendLine(obj == null ? string.Empty : obj.ToString());
		}

		public void WriteLine()
		{
			_result.AppendLine();
		}

		public string GetResult()
		{
			return _result.ToString();
		}
	}
}