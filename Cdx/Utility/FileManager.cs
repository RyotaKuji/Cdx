internal static class FileManager
{
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
}
