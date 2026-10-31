internal class History
{
	Stack<string> undoHistory = [];
	Stack<string> redoHistory = [];
	string currentPath = string.Empty;

	public void Add(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			throw new ArgumentException("Path cannot be null or empty.", nameof(path));
		}

		if (string.IsNullOrEmpty(currentPath) == false)
		{
			undoHistory.Push(currentPath);
		}
		currentPath = path;
		redoHistory.Clear();
	}

	public string? TryGetUndoPath()
	{
		undoHistory.TryPeek(out string? path);
		return path;
	}
	public string? TryGetRedoPath()
	{
		redoHistory.TryPeek(out string? path);
		return path;
	}

	public string? Undo()
	{
		if (string.IsNullOrEmpty(currentPath))
		{
			throw new InvalidOperationException("Cannot undo because the current path is null or empty.");
		}

		if (undoHistory.TryPop(out string? path))
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
	public string? Redo()
	{
		if (string.IsNullOrEmpty(currentPath))
		{
			throw new InvalidOperationException("Cannot redo because the current path is null or empty.");
		}

		if (redoHistory.TryPop(out string? path))
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
