using System.Text;

static class App
{
	static Options CurrentOptions;
	static void MainA(string[] args)
	{
		CurrentOptions = ParseOptions(args);
		Console.OutputEncoding = Encoding.GetEncoding("utf-8");
		string currentDirectory = CurrentOptions.CurrentDirectory ?? @"C:\";
		Mode mode = Mode.Input;
		Stack<string> history = [];
		Stack<string> redoHistory = [];
		string message = string.Empty;

		while (true)
		{
			Stack<string> searchTerms = [];
			bool succeeded = TryGetChildren(currentDirectory, out List<FileSystemInfo> children);

			if (succeeded == false)
			{
				message = $"Cannot access directory '{currentDirectory}'";
				if (TryUndoMove(ref currentDirectory, history, redoHistory))
				{
					continue;
				}
				else
				{
					throw new UnauthorizedAccessException(message);
				}
			}

			while (true)
			{
				List<FileSystemInfo> filtered = FilterByTerms(children, searchTerms).ToList();

				if (filtered.Count == 0)
				{
					mode = Mode.Command;
				}

				if (searchTerms.Count > 0 && filtered.Count == 1)
				{
					ControlDeterminedItem(filtered[0], ref currentDirectory, history, redoHistory);
					break;
				}

				Console.Clear();
				if (string.IsNullOrEmpty(message))
				{
					RenderCurrentPath(currentDirectory);
				}
				else
				{
					RenderMessage(message);
					message = string.Empty;
				}
				(int left, int top) = RenderSearchBar(mode);
				RenderTerms(searchTerms);
				RenderDirItems(filtered);

				Console.SetCursorPosition(left, top);

				if (mode == Mode.Input)
				{
					string? input = new((Console.ReadLine()?.Where(c => !char.IsControl(c)) ?? "").ToArray());

					if (string.IsNullOrWhiteSpace(input))
					{
						mode = Mode.Command;
					}
					else if (input.StartsWith(':'))
					{
						ExitWithResult(currentDirectory);
					}
					else
					{
						if (searchTerms.Contains(input, StringComparer.OrdinalIgnoreCase) == false)
						{
							searchTerms.Push(input);
						}
					}
				}
				else
				{
					ConsoleKeyInfo keyInfo = Console.ReadKey(true);
					if (keyInfo.Key == ConsoleKey.Enter)
					{
						mode = Mode.Input;
						if (filtered.Count == 0)
						{
							break;
						}
					}
					else if (keyInfo.Key == ConsoleKey.Escape)
					{
						mode = Mode.Input;
						break;
					}
					else if (keyInfo.Key == ConsoleKey.Backspace)
					{
						if (searchTerms.TryPop(out _) == false)
						{
							message = "No search terms to remove.";
						}
					}
					else if (keyInfo.Key == ConsoleKey.DownArrow)
					{
						if (filtered.Count == 0)
						{
							message = "No items to select.";
						}
						else
						{
							ControlDeterminedItem(filtered[0], ref currentDirectory, history, redoHistory);
							break;
						}
					}
					else if (keyInfo.Key == ConsoleKey.UpArrow)
					{
						if (TryMoveToParentDirectory(ref currentDirectory, history, redoHistory))
						{
							break;
						}
						else
						{
							message = "No parent directory available.";
						}
					}
					else if (keyInfo.Key == ConsoleKey.LeftArrow)
					{
						if (TryUndoMove(ref currentDirectory, history, redoHistory))
						{
							break;
						}
						else
						{
							message = "No undo history available.";
						}
					}
					else if (keyInfo.Key == ConsoleKey.RightArrow)
					{
						if (TryRedoMove(ref currentDirectory, history, redoHistory))
						{
							break;
						}
						else
						{
							message = "No redo history available.";
						}
					}
					else
					{
						List<ConsoleKeyInfo> keys = [keyInfo];
						while (Console.KeyAvailable)
						{
							ConsoleKeyInfo additionalKey = Console.ReadKey(true);
							keys.Add(additionalKey);
						}

						string sendText = new(keys.Select(k => k.KeyChar).Where(c => !char.IsControl(c)).ToArray());
						if (string.IsNullOrWhiteSpace(sendText) == false)
						{
							SendKeys.SendWait(sendText);
							mode = Mode.Input;
							if (filtered.Count == 0)
							{
								break;
							}
						}
						else
						{
							message = "Unrecognized command.";
						}
					}
				}
			}
		}
	}

	static bool TryGetChildren(string directoryPath, out List<FileSystemInfo> result)
	{
		try
		{
			DirectoryInfo directory = new(directoryPath);
			result = directory.EnumerateFileSystemInfos()
				.OrderBy(x => x is FileInfo)
				.ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
				.ToList();
			return true;
		}
		catch
		{
			result = [];
			return false;
		}
	}

	static IEnumerable<FileSystemInfo> FilterByTerms(IEnumerable<FileSystemInfo> items, IEnumerable<string> terms)
	{
		IEnumerable<string> normalizedTerms = terms
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => x.Trim());

		return items
			.Where(item => normalizedTerms.All(term => item.Name.Contains(term, StringComparison.OrdinalIgnoreCase)));
	}

	static void RenderCurrentPath(string currentDirectory)
	{
		Console.WriteLine(currentDirectory);
	}

	static (int Left, int Top) RenderSearchBar(Mode mode)
	{
		string prompt = mode switch
		{
			Mode.Input => "> ",
			Mode.Command => "[COMMAND]",
			_ => string.Empty,
		};
		Console.CursorVisible = mode == Mode.Input;
		Console.Write($"{prompt} ");
		(int, int) result = (Console.CursorLeft, Console.CursorTop);
		Console.WriteLine();
		return result;
	}

	static void RenderTerms(Stack<string> searchTerms)
	{
		Console.WriteLine(string.Join(" | ", searchTerms.Reverse()));
	}

	static void RenderDirItems(IEnumerable<FileSystemInfo> filtered)
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
				return $"{marker} {item.Name}".Length;
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
			string text = $"{marker} {item.Name}";

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

	static void ControlDeterminedItem(
		FileSystemInfo item,
		ref string currentDirectory,
		Stack<string> history,
		Stack<string> redoHistory)
	{
		if (item is DirectoryInfo directory)
		{
			history.Push(currentDirectory);
			redoHistory.Clear();
			currentDirectory = directory.FullName;
		}
		else
		{
			ExitWithResult(item.FullName);
		}
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
		if (CurrentOptions.ResultFilePath is not null)
		{
			try
			{
				File.WriteAllText(CurrentOptions.ResultFilePath, resultPath);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"Error writing result to '{CurrentOptions.ResultFilePath}': {ex.Message}");
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
			else if (string.Equals(args[i], "--current-directory", StringComparison.OrdinalIgnoreCase))
			{
				if (i + 1 >= args.Length)
				{
					Console.Error.WriteLine("Missing value for --current-directory");
					Environment.Exit(1);
				}

				currentDirectory = args[++i];
			}
		}

		return new(currentDirectory, writeResult);
	}

	readonly record struct Options(string? CurrentDirectory, string? ResultFilePath);

	enum Mode
	{
		Input,
		Command,
	}
}
