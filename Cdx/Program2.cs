using System.Text;

static class App2
{
	static Options StartupOptions;
	static string Directory = string.Empty;
	static List<FileSystemInfo> Children = [];
	static List<FileSystemInfo> FilteredChildren = [];
	static Stack<string> SearchTerms = [];
	static Stack<string> History = [];
	static Stack<string> RedoHistory = [];
	static string Message = string.Empty;

	enum Mode
	{
		Ctrl_D = 0x04,
	}

	static void Main(string[] args)
	{
		StartupOptions = ParseOptions(args);
		Console.OutputEncoding = Encoding.GetEncoding("utf-8");
		ChangeDirectory(StartupOptions.CurrentDirectory ?? @"C:\");

		while (true)
		{
			Render();
			string? input = Console.ReadLine();
			if (string.IsNullOrEmpty(input))
			{
				if (FilteredChildren.Count > 0)
				{
					DetermineItem(FilteredChildren[0]);
				}
			}
			else if (char.IsControl(input[0]))
			{
				foreach (char c in input)
				{
					if (c == (char)Mode.Ctrl_D)
					{
						RemoveSearchTerm();
					}
				}
			}
			else
			{
				string normalizedTerms = new([.. input.Where(c => char.IsControl(c) == false)]);
				AddSearchTerm(normalizedTerms);
			}
			if (FilteredChildren.Count == 1)
			{
				DetermineItem(FilteredChildren[0]);
			}
		}
	}

	static void DetermineItem(FileSystemInfo item)
	{
		if (item is DirectoryInfo directory)
		{
			PushHistory(Directory, History, RedoHistory);
			ChangeDirectory(directory.FullName);
		}
		else
		{
			ExitWithResult(item.FullName);
		}
	}

	static string ReadInput()
	{
		return new((Console.ReadLine()?.Where(c => !char.IsControl(c)) ?? "").ToArray());
	}

	static void ChangeDirectory(string directory)
	{
		(List<FileSystemInfo>? children, string message) = GetChildren(directory);
		if (children is not null)
		{
			Directory = directory;
			Children = children;
			SearchTerms.Clear();
			FilteredChildren = GetFilteredChildren(Children, Directory, SearchTerms).ToList();
		}
		else
		{
			Message = message;
		}
	}

	static void AddSearchTerm(string term)
	{
		if (string.IsNullOrEmpty(term) == false)
		{
			SearchTerms.Push(term);
			FilteredChildren = GetFilteredChildren(Children, Directory, SearchTerms).ToList();
		}
	}

	static void RemoveSearchTerm()
	{
		if (SearchTerms.TryPop(out _))
		{
			FilteredChildren = GetFilteredChildren(Children, Directory, SearchTerms).ToList();
		}
	}

	static (List<FileSystemInfo>? Value, string Message) GetChildren(string path)
	{
		DirectoryInfo directory = new(path);
		if (directory.Exists == false)
		{
			return new(null, $"Path '{path}' does not exist.");
		}
		try
		{
			List<FileSystemInfo> result =
				directory.EnumerateFileSystemInfos()
				.OrderBy(x => x is FileInfo)
				.ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
				.ToList();
			if (directory.Parent is not null)
			{
				result.Insert(0, directory.Parent);
			}
			return new(result, string.Empty);
		}
		catch (Exception ex)
		{
			return new(null, $"Error accessing path '{path}': {ex.Message}");
		}
	}

	static IEnumerable<FileSystemInfo> GetFilteredChildren(IEnumerable<FileSystemInfo> items, string currentDirectory, IEnumerable<string> terms)
	{
		IEnumerable<string> normalizedTerms = terms
			.Where(x => string.IsNullOrEmpty(x) == false)
			.Select(x => x.Trim());

		return items
			.Where(item => normalizedTerms.All(term => Path.GetRelativePath(currentDirectory, item.FullName).Contains(term, StringComparison.OrdinalIgnoreCase)));
	}

	static void Render()
	{
		Console.Clear();
		if (string.IsNullOrEmpty(Message))
		{
			RenderCurrentPath(Directory);
		}
		else
		{
			RenderMessage(Message);
			Message = string.Empty;
		}
		(int left, int top) = RenderSearchBar();
		RenderTerms(SearchTerms);
		RenderDirItems(FilteredChildren, Directory);
		Console.SetCursorPosition(left, top);
	}

	static void RenderCurrentPath(string currentDirectory)
	{
		Console.WriteLine(currentDirectory);
	}

	static (int Left, int Top) RenderSearchBar()
	{
		Console.Write($"> ");
		(int, int) result = (Console.CursorLeft, Console.CursorTop);
		Console.WriteLine();
		return result;
	}

	static void RenderTerms(Stack<string> searchTerms)
	{
		Console.WriteLine(string.Join(" | ", searchTerms.Reverse()));
	}

	static void RenderDirItems(IEnumerable<FileSystemInfo> filtered, string currentDirectory)
	{
		if (filtered.Any() == false)
		{
			Console.WriteLine("(no items match)");
			return;
		}

		int startTop = Console.CursorTop;
		int startLeft = 0;
		int currentTop = startTop;
		int currentLeft = startLeft;
		int maxRow = Console.WindowHeight - 1;

		int maxItemWidth = filtered
			.Select(item =>
			{
				string marker = item is DirectoryInfo ? "📁" : "📄";
				return $"{marker} {Path.GetRelativePath(currentDirectory, item.FullName)}".Length;
			})
			.DefaultIfEmpty(0)
			.Max();
		int columnWidth = Math.Clamp(maxItemWidth, 1, Console.WindowWidth / 3);

		foreach (FileSystemInfo item in filtered)
		{
			if (currentTop > maxRow)
			{
				currentTop = startTop;
				currentLeft += columnWidth;
			}

			if (currentLeft >= Console.WindowWidth)
			{
				break;
			}

			string marker = item is DirectoryInfo ? "📁" : "📄";
			string text = $"{marker} {Path.GetRelativePath(currentDirectory, item.FullName)}";

			Console.SetCursorPosition(currentLeft, currentTop);
			int writableWidth = Math.Max(1, Console.WindowWidth - currentLeft);
			if (text.Length > writableWidth)
			{
				text = text[..Math.Max(0, writableWidth - 1)];
			}
			Console.Write(text);

			currentTop++;
		}
	}

	static void RenderMessage(string message)
	{
		Console.WriteLine(message);
	}

	static bool TryMoveToParentDirectory(
		ref string currentDirectory,
		Stack<string> history,
		Stack<string> redoHistory)
	{
		DirectoryInfo directory = new(currentDirectory);
		if (directory.Parent is not null)
		{
			history.Push(currentDirectory);
			redoHistory.Clear();
			currentDirectory = directory.Parent.FullName;
			return true;
		}
		else
		{
			return false;
		}
	}

	static void PushHistory(string currentDirectory, Stack<string> history, Stack<string> redoHistory)
	{
		history.Push(currentDirectory);
		redoHistory.Clear();
	}

	static bool TryUndoMove(ref string currentDirectory, Stack<string> history, Stack<string> redoHistory)
	{
		if (history.TryPop(out string? path))
		{
			redoHistory.Push(currentDirectory);
			currentDirectory = path;
			return true;
		}
		else
		{
			return false;
		}
	}

	static bool TryRedoMove(ref string currentDirectory, Stack<string> history, Stack<string> redoHistory)
	{
		if (redoHistory.TryPop(out string? path))
		{
			history.Push(currentDirectory);
			currentDirectory = path;
			return true;
		}
		else
		{
			return false;
		}
	}

	static void ExitWithResult(string resultPath)
	{
		Console.Clear();
		if (StartupOptions.ResultFilePath is not null)
		{
			try
			{
				File.WriteAllText(StartupOptions.ResultFilePath, resultPath);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"Error writing result to '{StartupOptions.ResultFilePath}': {ex.Message}");
				Environment.Exit(1);
			}
		}
		Environment.Exit(0);
	}

	static Options ParseOptions(string[] args)
	{
		string? writeResult = null;
		string? currentDirectory = null;

		for (int i = 0; i < args.Length; i++)
		{
			if (string.Equals(args[i], "--write-result", StringComparison.OrdinalIgnoreCase))
			{
				if (i + 1 >= args.Length)
				{
					Console.Error.WriteLine("Missing value for --write-result");
					Environment.Exit(1);
				}

				writeResult = args[++i];
			}
			else if (string.Equals(args[i], "--current-path", StringComparison.OrdinalIgnoreCase))
			{
				if (i + 1 >= args.Length)
				{
					Console.Error.WriteLine("Missing value for --current-path");
					Environment.Exit(1);
				}

				currentDirectory = args[++i];
			}
		}

		return new(currentDirectory, writeResult);
	}

	readonly record struct Options(string? CurrentDirectory, string? ResultFilePath);
}
