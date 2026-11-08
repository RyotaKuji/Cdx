internal static class AccessManager
{
	public static bool IsValidDirectory(DirectoryInfo directory, out string exceptionMessage)
	{
		try
		{
			using var _ = directory.EnumerateFileSystemInfos().GetEnumerator();
			exceptionMessage = string.Empty;
			return true;
		}
		catch (Exception ex)
		{
			exceptionMessage = ex.Message;
			return false;
		}
	}

	public static bool IsValidFile(FileInfo file, out string exceptionMessage)
	{
		try
		{
			using var _ = file.OpenRead();
			exceptionMessage = string.Empty;
			return true;
		}
		catch (Exception ex)
		{
			exceptionMessage = ex.Message;
			return false;
		}
	}
}
