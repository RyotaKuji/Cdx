internal static class ActionResolver
{
	public static State Resolve(IEnumerable<FileSystemInfo> filteredItems)
	{
		int itemsCount = filteredItems.Count();

		if (itemsCount == 0)
		{
			return new();
		}

		else if (itemsCount == 1)
		{
			FileSystemInfo singleItem = filteredItems.First();
			if (singleItem is DirectoryInfo directory)
			{
				if (AccessManager.IsValidDirectory(directory))
				{
					return new(
						Action: ActionType.ChangeDirectory,
						Directory: singleItem.FullName);
				}
				else
				{
					return new();
				}
			}
			else if (singleItem is FileInfo file)
			{
				if (AccessManager.IsValidFile(file))
				{
					return new(
						Action: ActionType.OpenFile,
						Directory: file.DirectoryName ?? Directory.GetCurrentDirectory(),
						FilePath: file.FullName);
				}
				else
				{
					return new();
				}
			}
			else
			{
				return new();
			}
		}

		else
		{
			return new();
		}
	}

	public record struct State(
		ActionType Action = ActionType.None,
		string Directory = "",
		string FilePath = ""
	);

	public enum ActionType
	{
		None,
		ChangeDirectory,
		OpenFile,
	}
}
