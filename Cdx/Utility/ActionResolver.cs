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
				if (AccessManager.IsValidDirectory(directory, out string exceptionMessage))
				{
					return new(
						Action: ActionType.ChangeDirectory,
						Directory: directory);
				}
				else
				{
					return new(ActionType.Cancel, Message: exceptionMessage);
				}
			}
			else if (singleItem is FileInfo file)
			{
				if (AccessManager.IsValidFile(file, out string exceptionMessage))
				{
					return new(
						Action: ActionType.OpenFile,
						Directory: file.Directory,
						FilePath: file.FullName);
				}
				else
				{
					return new(ActionType.Cancel, Message: exceptionMessage);
				}
			}
			else
			{
				return new(ActionType.Cancel, Message: $"The item {singleItem.FullName} is not valid.");
			}
		}

		else
		{
			return new();
		}
	}

	public record struct State(
		ActionType Action = ActionType.None,
		DirectoryInfo? Directory = null,
		string FilePath = "",
		string Message = ""
	);

	public enum ActionType
	{
		None,
		ChangeDirectory,
		OpenFile,
		Cancel,
	}
}
