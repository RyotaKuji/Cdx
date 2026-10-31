using System.Text;

internal class App(
	Input input,
	CurrentDirectory currentDirectory,
	SearchTerms searchTerms,
	Children children,
	History history,
	Message message)
{
	public string Run(DirectoryInfo initialCurrentDirectory, FileInfo? resultFile)
	{
		Console.OutputEncoding = Encoding.GetEncoding("utf-8");
		if (AccessManager.IsValidDirectory(initialCurrentDirectory) == false)
		{
			throw new InvalidOperationException("The initial directory is not valid.");
		}
		currentDirectory.SetValue(initialCurrentDirectory.FullName);
		history.Add(currentDirectory.Value);
		children.SetDirectory(currentDirectory.Value);
		children.Filter(searchTerms.Values);
		while (true)
		{
			Console.CursorVisible = false;
			if (message.HasValue)
			{
				message.Render();
			}
			else
			{
				currentDirectory.Render();
			}
			searchTerms.Render();
			children.Render();
			Console.CursorVisible = true;

			ConsoleKeyInfo keyInfo = input.ReadKey();
			ConsoleKey key = keyInfo.Key;
			ActionResolver.State nextActionState;
			// 一番上の候補を選択する
			if (key == ConsoleKey.Enter)
			{
				if (children.FilteredValue.Any())
				{
					nextActionState = ActionResolver.Resolve([children.FilteredValue.First()]);
				}
				else
				{
					nextActionState = ActionResolver.Resolve([]);
				}
			}
			// Undo
			else if (key == ConsoleKey.LeftArrow)
			{
				string? undoPath = history.TryGetUndoPath();
				if (undoPath is not null)
				{
					nextActionState = ActionResolver.Resolve([new DirectoryInfo(undoPath)]);
				}
				else
				{
					nextActionState = ActionResolver.Resolve([]);
				}
			}
			// Redo
			else if (key == ConsoleKey.RightArrow)
			{
				string? redoPath = history.TryGetRedoPath();
				if (redoPath is not null)
				{
					nextActionState = ActionResolver.Resolve([new DirectoryInfo(redoPath)]);
				}
				else
				{
					nextActionState = ActionResolver.Resolve([]);
				}
			}
			// 親ディレクトリに移動する
			else if (key == ConsoleKey.UpArrow)
			{
				string? parentDirectory = Directory.GetParent(currentDirectory.Value)?.FullName;
				if (parentDirectory is not null)
				{
					nextActionState = ActionResolver.Resolve([new DirectoryInfo(parentDirectory)]);
				}
				else
				{
					nextActionState = ActionResolver.Resolve([]);
				}
			}
			// 検索ワードを削除する
			else if (key == ConsoleKey.Backspace)
			{
				searchTerms.RemoveLast();
				nextActionState = ActionResolver.Resolve(children.FilteredValue);
			}
			// 検索ワードを全て削除する
			else if (key == ConsoleKey.Escape)
			{
				searchTerms.Clear();
				nextActionState = ActionResolver.Resolve(children.FilteredValue);
			}
			// 検索ワードを追加する
			else
			{
				string text = input.ReadLine(keyInfo);
				searchTerms.Add(text);
				nextActionState = ActionResolver.Resolve(children.FilteredValue);
			}

			if (nextActionState.Action == ActionResolver.ActionType.OpenFile)
			{
				return nextActionState.FilePath;
			}
			else if (nextActionState.Action == ActionResolver.ActionType.ChangeDirectory)
			{
				if (key == ConsoleKey.LeftArrow)
				{
					string path = history.Undo() ?? throw new InvalidOperationException("Cannot undo because there is no history.");
					currentDirectory.SetValue(path);
				}
				else if (key == ConsoleKey.RightArrow)
				{
					string path = history.Redo() ?? throw new InvalidOperationException("Cannot redo because there is no history.");
					currentDirectory.SetValue(path);
				}
				else
				{
					currentDirectory.SetValue(nextActionState.Directory);
					history.Add(nextActionState.Directory);
				}
				children.SetDirectory(currentDirectory.Value);
				searchTerms.Clear();
				children.Filter(searchTerms.Values);
			}
			else if (nextActionState.Action == ActionResolver.ActionType.Cancel)
			{
				message.SetValue(nextActionState.Message);
			}
			else if (nextActionState.Action == ActionResolver.ActionType.None)
			{
				children.Filter(searchTerms.Values);
			}
		}
	}
}
