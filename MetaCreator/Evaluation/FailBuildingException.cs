using System;
using System.Runtime.Serialization;

namespace MetaCreator.Evaluation
{
	[Serializable]
	public class FailBuildingException : Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public FailBuildingException()
		{
		}

		public FailBuildingException(string message) : base(message)
		{
		}

		public FailBuildingException(string message, Exception inner) : base(message, inner)
		{
		}

		protected FailBuildingException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}
}