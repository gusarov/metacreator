using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using MetaCreator.Utils;

namespace MetaCreator.Evaluation
{
	static class TempFiles
	{
		const int _reduceCleanups = 4;
		static string MetaTempPath { get { return Path.Combine(Path.GetTempPath(), "MetaCreator"); } }

		static string GetNewTempPath(string suggestion = null)
		{
			return Path.Combine(MetaTempPath, suggestion ?? Ext.GenerateId());
		}

		public static bool IsTemp(string path)
		{
			return path.StartsWith(MetaTempPath, StringComparison.InvariantCultureIgnoreCase);
		}

		public static string GetNewTempFolder()
		{
			try
			{
				var path = GetNewTempPath();
				Directory.CreateDirectory(path);
				return path;
			}
			finally
			{
				// cleanup old folders
				// reduce number of times for reading all folder
				if (_reduceCleanups.Random() == 0)
				{
					try
					{
						var dtn = DateTime.UtcNow;
						foreach (var dir in new DirectoryInfo(MetaTempPath).GetDirectories().Where(x => (dtn - x.CreationTimeUtc) > TimeSpan.FromDays(1)))
						{
							try
							{
								dir.Delete(true);
							}
							catch {}
						}
					}
					catch {}
				}
			}
		}

		public static string GetNewTempFile(string suggestion = null)
		{
			try
			{
				return GetNewTempPath(suggestion);
			}
			finally
			{
				// cleanup old folders
				// reduce number of times for reading all folder
				if (_reduceCleanups.Random() == 0)
				{
					try
					{
						var dtn = DateTime.UtcNow;
						foreach (var file in new DirectoryInfo(MetaTempPath).GetFiles().Where(x => (dtn - x.CreationTimeUtc) > TimeSpan.FromDays(1)))
						{
							try
							{
								File.Delete(file.FullName);
							}
							catch {}
						}
					}
					catch {}
				}
			}
		}
	}
}
