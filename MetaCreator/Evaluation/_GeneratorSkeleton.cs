// METACODE
// This file is generated and compiled in-memory.
// If there are any compilation errors - file can be saved to temporary path for working with IDE.

#region Imports
{0}
#endregion

public class Generator : MetaCreator.IMetaGenerator, MetaCreator.IMetaWriter
{{
	readonly IMetaEngine _engine;

	public IMetaEngine Engine
	{{
		get
		{{
			return _engine;
		}}
	}}

	public Generator(IMetaEngine engine)
	{{
		_engine = engine;
	}}

	public string Run()
	{{
#region methodbody

		{1}

#endregion
		return GetResult();
	}}

#region classbody

{2}

#endregion

#region utils

	public void Write(string msg, params object[] args)
	{{
		_engine.Writer.Write(msg, args);
	}}

	public void WriteLine(string msg, params object[] args)
	{{
		_engine.Writer.WriteLine(msg, args);
	}}

	public void Write(string msg)
	{{
		_engine.Writer.Write(msg);
	}}

	public void WriteLine(string msg)
	{{
		_engine.Writer.WriteLine(msg);
	}}

	public void Write(object obj)
	{{
		_engine.Writer.Write(obj);
	}}

	public void WriteLine(object obj)
	{{
		_engine.Writer.WriteLine(obj);
	}}

	public void WriteLine()
	{{
		_engine.Writer.WriteLine();
	}}

	public string GetResult()
	{{
		return _engine.Writer.GetResult();
	}}

#endregion
}}