using System;
using System.Collections.Generic;
using System.Text;

internal static class AccessManager
{
	public static bool IsValidDirectory(DirectoryInfo directory)
	{
		if (directory.Exists == false)
		{
			return false;
		}

		try
		{
			directory.EnumerateFileSystemInfos();
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static bool IsValidFile(FileInfo file)
	{
		if (file.Exists == false)
		{
			return false;
		}
		try
		{
			using (file.OpenRead())
			{
				return true;
			}
		}
		catch
		{
			return false;
		}
	}
}
