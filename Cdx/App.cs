using System.Text;

internal class App(
	Input input,
	CurrentDirectory currentDirectory,
	SearchTerms searchTerms,
	Children children,
	Clipboard clipboard,
	History history,
	Message message,
	Renderer renderer)
{
	public string Run(DirectoryInfo initialCurrentDirectory)
	{
		Console.OutputEncoding = Encoding.GetEncoding("utf-8");
		if (AccessManager.IsValidDirectory(initialCurrentDirectory) == false)
		{
			throw new InvalidOperationException("The initial directory is not valid.");
		}
		currentDirectory.SetValue(initialCurrentDirectory);
		history.Add(currentDirectory.Value);
		children.SetDirectory(currentDirectory.Value);
		children.Filter(searchTerms.Values);
		while (true)
		{
			renderer.Render();

			ConsoleKeyInfo keyInfo = input.ReadKey(Input.InputMode.Command);
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
				DirectoryInfo? undoPath = history.TryGetUndoPath();
				if (undoPath is not null)
				{
					nextActionState = ActionResolver.Resolve([undoPath]);
				}
				else
				{
					nextActionState = ActionResolver.Resolve([]);
				}
			}
			// Redo
			else if (key == ConsoleKey.RightArrow)
			{
				DirectoryInfo? redoPath = history.TryGetRedoPath();
				if (redoPath is not null)
				{
					nextActionState = ActionResolver.Resolve([redoPath]);
				}
				else
				{
					nextActionState = ActionResolver.Resolve([]);
				}
			}
			// 親ディレクトリに移動する
			else if (key == ConsoleKey.UpArrow)
			{
				DirectoryInfo? parentDirectory = currentDirectory.Value.Parent;
				if (parentDirectory is not null)
				{
					nextActionState = ActionResolver.Resolve([parentDirectory]);
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
			// リセット
			else if (key == ConsoleKey.Escape)
			{
				searchTerms.Clear();
				clipboard.Clear();
				nextActionState = ActionResolver.Resolve(children.FilteredValue);
			}
			// クリップボードに追加
			else if (key == ConsoleKey.DownArrow)
			{
				clipboard.Capture([currentDirectory.Value]);
				clipboard.Capture(children.Values);
				nextActionState = ActionResolver.Resolve([]);
			}
			// 入力
			else
			{
				string text = input.ReadLine(keyInfo, Input.InputMode.Input);
				if (text.StartsWith(':'))
				{
					string[] commandParts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
					if (commandParts[0].Equals(":copy", StringComparison.OrdinalIgnoreCase))
					{
					}
					nextActionState = ActionResolver.Resolve(children.FilteredValue);
				}
				else
				{
					searchTerms.Add(text);
					nextActionState = ActionResolver.Resolve(children.FilteredValue);
				}
			}

			if (nextActionState.Action == ActionResolver.ActionType.OpenFile)
			{
				return nextActionState.FilePath;
			}
			else if (nextActionState.Action == ActionResolver.ActionType.ChangeDirectory)
			{
				if (key == ConsoleKey.LeftArrow)
				{
					DirectoryInfo path = history.Undo() ?? throw new InvalidOperationException("Cannot undo because there is no history.");
					currentDirectory.SetValue(path);
				}
				else if (key == ConsoleKey.RightArrow)
				{
					DirectoryInfo path = history.Redo() ?? throw new InvalidOperationException("Cannot redo because there is no history.");
					currentDirectory.SetValue(path);
				}
				else
				{
					currentDirectory.SetValue(nextActionState.Directory ?? throw new InvalidOperationException("Cannot change directory because the directory is null."));
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
