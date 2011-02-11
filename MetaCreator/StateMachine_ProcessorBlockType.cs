using System;

namespace MetaCreator
{
	internal abstract class StateMachine_ProcessorBlockType
	{
		public static StateMachine_ProcessorBlockType CommandBlock = new Command();
		public static StateMachine_ProcessorBlockType BodyBlock = new Body();
		public static StateMachine_ProcessorBlockType ClassBlock = new Class();
		public static StateMachine_ProcessorBlockType ExpressionBlock = new Expression();

		public char Type;

		public abstract bool CanSwitchTo(StateMachine_ProcessorBlockType state);
		public virtual bool CanSwitchFrom(StateMachine_ProcessorBlockType state)
		{
			return true;
		}

		class Command : StateMachine_ProcessorBlockType
		{
			public override bool CanSwitchTo(StateMachine_ProcessorBlockType state)
			{
				return true;
			}

			public override bool CanSwitchFrom(StateMachine_ProcessorBlockType state)
			{
				if (state == CommandBlock)
				{
					return true;
				}
				return false;
			}
		}

		class Body : StateMachine_ProcessorBlockType
		{
			public override bool CanSwitchTo(StateMachine_ProcessorBlockType state)
			{
				if (state == CommandBlock)
				{
					return false;
				}
				if (state == BodyBlock)
				{
					return true;
				}
				if (state == ClassBlock)
				{
					return true;
				}
				if (state == ExpressionBlock)
				{
					return true;
				}
				throw new ArgumentOutOfRangeException();
			}

		}

		class Class : StateMachine_ProcessorBlockType
		{
			public override bool CanSwitchTo(StateMachine_ProcessorBlockType state)
			{
				return true;
			}
		}

		class Expression : StateMachine_ProcessorBlockType
		{
			public override bool CanSwitchTo(StateMachine_ProcessorBlockType state)
			{
				if (state == CommandBlock)
				{
					return false;
				}
				if (state == BodyBlock)
				{
					return true;
				}
				if (state == ClassBlock)
				{
					return true;
				}
				if (state == ExpressionBlock)
				{
					return true;
				}
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}