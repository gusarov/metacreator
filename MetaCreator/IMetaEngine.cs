using System.Linq;
using System.Collections.Generic;
using System;

namespace MetaCreator
{
	public interface IMetaEngine
	{
		void AddToCompile(string fileContent);
	}


}