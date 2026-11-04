internal class History
{
	Stack<DirectoryInfo> undoHistory = [];
	Stack<DirectoryInfo> redoHistory = [];
	DirectoryInfo? currentPath;

	public void Add(DirectoryInfo path)
	{
		if (path is null)
		{
			throw new ArgumentException("Path cannot be null.", nameof(path));
		}

		if (currentPath is not null)
		{
			undoHistory.Push(currentPath);
		}
		currentPath = path;
		redoHistory.Clear();
	}

	public DirectoryInfo? TryGetUndoPath()
	{
		undoHistory.TryPeek(out DirectoryInfo? path);
		return path;
	}
	public DirectoryInfo? TryGetRedoPath()
	{
		redoHistory.TryPeek(out DirectoryInfo? path);
		return path;
	}

	public DirectoryInfo? Undo()
	{
		if (currentPath is null)
		{
			throw new InvalidOperationException("Cannot undo because the current path is null.");
		}

		if (undoHistory.TryPop(out DirectoryInfo? path))
		{
			redoHistory.Push(currentPath);
			currentPath = path;
			return currentPath;
		}
		else
		{
			return null;
		}
	}
	public DirectoryInfo? Redo()
	{
		if (currentPath is null)
		{
			throw new InvalidOperationException("Cannot redo because the current path is null.");
		}

		if (redoHistory.TryPop(out DirectoryInfo? path))
		{
			undoHistory.Push(currentPath);
			currentPath = path;
			return currentPath;
		}
		else
		{
			return null;
		}
	}
}
