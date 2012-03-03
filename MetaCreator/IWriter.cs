namespace MetaCreator
{
	public interface IWriter
	{
		void Write(string msg, params object[] args);
		void WriteLine(string msg, params object[] args);
		void Write(string msg);
		void WriteLine(string msg);
		void Write(object obj);
		void WriteLine(object obj);
		void WriteLine();
	}
}