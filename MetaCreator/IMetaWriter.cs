using System;
using System.Collections.Generic;

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
	}

	public static class TransactionalWriterExt
	{
		public static void Transactional(this IMetaWriter writer, Action<IMetaWriter> writingAction)
		{
			var collector = new TransactionalWriter();
			writingAction(collector);
			collector.Apply(writer);
		}
	}

	public class TransactionalWriter : IMetaWriter
	{
		private readonly List<Action<IMetaWriter>> _actions = new List<Action<IMetaWriter>>();

		internal TransactionalWriter()
		{
		}

		public void Apply(IMetaWriter writer)
		{
			foreach (var action in _actions)
			{
				action(writer);
			}
		}

		public void Write(string msg, params object[] args)
		{
			_actions.Add(w => w.Write(msg, args));
		}

		public void WriteLine(string msg, params object[] args)
		{
			_actions.Add(w => w.WriteLine(msg, args));
		}

		public void Write(string msg)
		{
			_actions.Add(w => w.Write(msg));
		}

		public void WriteLine(string msg)
		{
			_actions.Add(w => w.WriteLine(msg));
		}

		public void Write(object obj)
		{
			_actions.Add(w => w.Write(obj));
		}

		public void WriteLine(object obj)
		{
			_actions.Add(w => w.WriteLine(obj));
		}

		public void WriteLine()
		{
			_actions.Add(w => w.WriteLine());
		}
	}
}