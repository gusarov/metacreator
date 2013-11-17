using System.Linq;
using System.Collections.Generic;
using System;

namespace MetaCreator
{
	class ArgumentConverterWriterProxy : IMetaWriter
	{
		private readonly IMetaWriter _writer;
		private readonly Func<object, object> _converter;

		public ArgumentConverterWriterProxy(IMetaWriter writer, Func<object, object> converter)
		{
			_writer = writer;
			_converter = converter;
		}

		public void Write(string msg, params object[] args)
		{
			for (int i = 0; i < args.Length; i++)
			{
				args[i] = _converter(args[i]);
			}
			_writer.Write(msg, args);
		}

		public void WriteLine(string msg, params object[] args)
		{
			for (int i = 0; i < args.Length; i++)
			{
				args[i] = _converter(args[i]);
			}
			_writer.WriteLine(msg, args);
		}

		public void Write(string msg)
		{
			_writer.Write(msg);
		}

		public void WriteLine(string msg)
		{
			_writer.WriteLine(msg);
		}

		public void Write(object obj)
		{
			_writer.Write(_converter(obj));
		}

		public void WriteLine(object obj)
		{
			_writer.WriteLine(_converter(obj));
		}

		public void WriteLine()
		{
			_writer.WriteLine();
		}

		public string GetResult()
		{
			return _writer.GetResult();
		}
	}
}