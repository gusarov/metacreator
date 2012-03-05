using System.Linq;
using System.Collections.Generic;
using System;

namespace MetaCreator
{
	public interface IMetaEngine
	{
		void AddToCompile(string fileContent);
		void AddToCompile(string fileName, string fileContent);
		void AddToCompile(bool fileInProject, string fileName, string fileContent);
	}
}