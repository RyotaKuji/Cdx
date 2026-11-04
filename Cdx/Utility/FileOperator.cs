internal static class FileOperator
{

	public static void Copy(IEnumerable<RenameInfo> items, string destinationDirectory)
	{
		foreach (RenameInfo item in items)
		{
			if (item.Info is DirectoryInfo directory)
			{
				string newDirectoryPath = Path.Combine(destinationDirectory, $"{item.Prefix}{directory.Name}{item.Suffix}");
				Directory.CreateDirectory(newDirectoryPath);
				Copy(directory.EnumerateFileSystemInfos(), newDirectoryPath);
			}
			else if (item.Info is FileInfo file)
			{
				string newFilePath = Path.Combine(destinationDirectory, $"{item.Prefix}{file.Name}{item.Suffix}");
				file.CopyTo(newFilePath, overwrite: true);
			}
		}
	}

	public static void Copy(IEnumerable<FileSystemInfo> items, string destinationDirectory)
	{
		foreach (FileSystemInfo item in items)
		{
			if (item is DirectoryInfo directory)
			{
				string newDirectoryPath = Path.Combine(destinationDirectory, directory.Name);
				Directory.CreateDirectory(newDirectoryPath);
				Copy(directory.EnumerateFileSystemInfos(), newDirectoryPath);
			}
			else if (item is FileInfo file)
			{
				string newFilePath = Path.Combine(destinationDirectory, file.Name);
				file.CopyTo(newFilePath, overwrite: true);
			}
		}
	}

	public record struct RenameInfo(
		FileSystemInfo Info,
		string Prefix = "",
		string Suffix = ""
	);
}
