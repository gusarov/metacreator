// METACODE
// This file is generated and compiled in-memory.
// If there are any compilation errors - file can be saved to temporary path for working with IDE.

#region Imports
{0}
#endregion

public class Generator : MetaCreator.IGenerator, MetaCreator.IMetaWriter
{{
	readonly IMetaEngine Engine;

	public Generator(IMetaEngine engine)
	{{
		Engine = engine;
	}}

	public string Run()
	{{
#region methodbody

		{1}

#endregion
		return Result.ToString();
	}}

#region classbody

{2}

#endregion

#region utils

	public StringBuilder Result = new StringBuilder();

	public void Write(string msg, params object[] args)
	{{
		Result.AppendFormat(msg, args);
	}}

	public void WriteLine(string msg, params object[] args)
	{{
		Result.AppendFormat(msg + Environment.NewLine, args);
	}}

	public void Write(string msg)
	{{
		Result.Append(msg);
	}}

	public void WriteLine(string msg)
	{{
		Result.AppendLine(msg);
	}}

	public void Write(object obj)
	{{
		Result.Append(obj == null ? string.Empty : obj.ToString());
	}}

	public void WriteLine(object obj)
	{{
		Result.AppendLine(obj == null ? string.Empty : obj.ToString());
	}}

	public void WriteLine()
	{{
		Result.AppendLine();
	}}

#endregion
}}