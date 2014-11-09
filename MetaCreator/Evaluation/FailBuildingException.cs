using System;
using System.Runtime.Serialization;

namespace MetaCreator.Evaluation
{
	[Serializable]
	public class FailBuildingException : Exception
	{
		public EvaluationResult Result { get; set; }
		public EarlyPassMode IgnoreThisFile { get; set; }

		public FailBuildingException()
		{
		}

		public FailBuildingException(string message)
			: base(message)
		{
		}

		public FailBuildingException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected FailBuildingException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
			Result = (EvaluationResult)info.GetValue("Result", typeof(EvaluationResult));
			IgnoreThisFile = (EarlyPassMode)info.GetValue("IgnoreThisFile", typeof(EarlyPassMode));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Result", Result, typeof(EvaluationResult));
			info.AddValue("IgnoreThisFile", IgnoreThisFile);
		}
	}
}