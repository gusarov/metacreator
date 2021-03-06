﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaCreator
{
	public interface IMetaGenerator : IMetaWriter
	{
		IMetaEngine Engine { get; }
		string Run();
	}
}
