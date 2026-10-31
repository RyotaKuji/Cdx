using System.Text;

internal class App(
	Input input,
	CurrentDirectory currentDirectory,
	SearchTerms searchTerms,
	Children children,
	History history)
{
	public void Run()
	{
		Console.OutputEncoding = Encoding.GetEncoding("utf-8");
		if (AccessManager.IsValidDirectory(new DirectoryInfo(@"C:\")) == false)
		{
			throw new InvalidOperationException("The initial directory is not valid.");
		}
		currentDirectory.SetValue(@"C:\");
		history.Add(currentDirectory.Value);
		children.SetDirectory(currentDirectory.Value);
		children.Filter(searchTerms.Values);
		while (true)
		{
			Console.CursorVisible = false;
			currentDirectory.Render();
			searchTerms.Render();
			children.Render();
			input.Render();
			Console.CursorVisible = true;

			ConsoleKeyInfo keyInfo = input.ReadKey();
			ConsoleKey key = keyInfo.Key;
			ActionResolver.State nextActionState;
			if (key == ConsoleKey.Backspace)
			{
				searchTerms.RemoveLast();
				nextActionState = ActionResolver.Resolve(children.FilteredValue);
			}
			else if (key == ConsoleKey.Enter)
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
			else if (key == ConsoleKey.UpArrow)
			{
				nextActionState = ActionResolver.Resolve(children.FilteredValue);
			}
			else if (key == ConsoleKey.DownArrow)
			{
				nextActionState = ActionResolver.Resolve(children.FilteredValue);
			}
			else
			{
				string text = input.ReadLine(keyInfo);
				searchTerms.Add(text);
				nextActionState = ActionResolver.Resolve(children.FilteredValue);
			}

			if (nextActionState.Action == ActionResolver.ActionType.OpenFile)
			{
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
			else if (nextActionState.Action == ActionResolver.ActionType.None)
			{
				children.Filter(searchTerms.Values);
			}
		}
	}
}
