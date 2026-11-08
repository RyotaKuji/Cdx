using System.Text;

internal class App(
	Input input,
	CurrentDirectory currentDirectory,
	SearchTerms searchTerms,
	Children children,
	Children_V children_V,
	Clipboard clipboard,
	History history,
	Message message,
	Renderer renderer)
{
	public string Run(DirectoryInfo initialCurrentDirectory)
	{
		Console.OutputEncoding = Encoding.UTF8;
		if (AccessManager.IsValidDirectory(initialCurrentDirectory, out string exceptionMessage) == false)
		{
			throw new ArgumentException(exceptionMessage);
		}
		currentDirectory.SetValue(initialCurrentDirectory);
		history.Add(currentDirectory.Value);
		children.SetDirectory(currentDirectory.Value);
		children.Filter(searchTerms.Values);
		while (true)
		{
			renderer.Render();

			message.SetValue(string.Empty);

			ConsoleKeyInfo keyInfo = input.ReadKey(Input.InputMode.AppCommand);
			ConsoleKey key = keyInfo.Key;
			ActionResolver.State nextActionState;
			// 一番上の候補を選択する
			if (key == ConsoleKey.Enter)
			{
				FileSystemInfo? selectedItem = children.GetBestMatchItem();
				if (selectedItem is not null)
				{
					nextActionState = ActionResolver.Resolve([selectedItem]);
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
			// ページを進む
			else if (key == ConsoleKey.PageUp)
			{
				children_V.PageUp();
				nextActionState = ActionResolver.Resolve([]);
			}
			// ページを戻る
			else if (key == ConsoleKey.PageDown)
			{
				children_V.PageDown();
				nextActionState = ActionResolver.Resolve([]);
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
				clipboard.Capture([currentDirectory.Value], Clipboard.ClipboardItemType.Parent);
				clipboard.Capture(children.FilteredValue, Clipboard.ClipboardItemType.Child);
				nextActionState = ActionResolver.Resolve([]);
			}
			// Children の表示を先頭から表示する
			else if (key == ConsoleKey.Home)
			{
				children_V.ShowHead();
				nextActionState = ActionResolver.Resolve([]);
			}
			// Children の表示を末尾から表示する
			else if (key == ConsoleKey.End)
			{
				children_V.ShowTail();
				nextActionState = ActionResolver.Resolve([]);
			}
			// 入力
			else
			{
				bool isRowCommand = keyInfo.KeyChar == ':';

				string text;
				if (isRowCommand)
				{
					text = input.ReadLine(Input.InputMode.RowCommand);
				}
				else
				{
					text = input.ReadLine(keyInfo, Input.InputMode.Input);
				}
				if (isRowCommand || text.StartsWith(':'))
				{
					Console.CursorVisible = false;
					string formattedText = text.TrimStart(':');
					if (string.IsNullOrWhiteSpace(formattedText) == false)
					{
						if (formattedText.Equals("q", StringComparison.OrdinalIgnoreCase) ||
							formattedText.Equals("quit", StringComparison.OrdinalIgnoreCase))
						{
							return currentDirectory.Value.FullName;
						}
						(string output, string error) = CommandManager.Execute(formattedText, currentDirectory.Value, clipboard);
						if (string.IsNullOrEmpty(error) == false)
						{
							message.SetValue(error);
						}
						else
						{
							message.SetValue(output);
						}
						children.SetDirectory(currentDirectory.Value);
						children.Filter(searchTerms.Values);
					}
					nextActionState = ActionResolver.Resolve([]);
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
