using System.Linq;
using System.Collections.Generic;
using System;

namespace MetaCreator
{
	public static class TransactionalWriterExt
	{
		public static void Transactional(this IMetaWriter writer, Action<IMetaWriter> writingAction)
		{
			var collector = new TransactionalWriter();
			writingAction(collector);
			collector.Apply(writer);
		}
	}
}