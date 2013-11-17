using System.Linq;
using System.Collections.Generic;
using System;

namespace MetaCreator
{
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
			_actions.Clear();
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

		public string GetResult()
		{
			throw new Exception("Can not read from TransactionWriter wrapper");
		}
	}
}