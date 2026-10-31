using System.Text;

internal class App(
	Input input,
	CurrentDirectory currentDirectory,
	SearchTerms searchTerms,
	Children children)
{
	public void Run()
	{
		Console.OutputEncoding = Encoding.GetEncoding("utf-8");
		currentDirectory.SetValue(@"C:\");
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
			if (CommandManager.IsRecognized(keyInfo))
			{
				if (keyInfo == CommandManager.BackSpace)
				{
					searchTerms.RemoveLast();
				}
				continue;
			}

			string text = input.ReadLine(keyInfo);
			searchTerms.Add(text);
			children.Filter(searchTerms.Values);

			ActionResolver.State nextActionState =
				ActionResolver.Resolve(children.FilteredValue);

			if (nextActionState.Action == ActionResolver.ActionType.OpenFile)
			{
			}
			else if (nextActionState.Action == ActionResolver.ActionType.ChangeDirectory)
			{
				currentDirectory.SetValue(nextActionState.Directory);
				children.SetDirectory(currentDirectory.Value);
				searchTerms.Clear();
				children.Filter(searchTerms.Values);
			}
			else
			{
			}
		}
	}
}
