internal class Renderer(
	CurrentDirectory currentDirectory,
	SearchTerms searchTerms,
	Children children,
	Message message)
{
	public void Render()
	{
		Console.CursorVisible = false;

		Console.SetCursorPosition(0, 0);

		if (message.HasValue)
		{
			message.Render();
		}
		else
		{
			currentDirectory.Render();
		}
		// input のカーソル位置を保存しておく
		(int cursorLeftForInput, int cursorTopForInput) = Console.GetCursorPosition();
		Console.SetCursorPosition(0, cursorTopForInput + 1);
		searchTerms.Render();
		children.Render();
		// input のカーソル位置を復元する
		Console.SetCursorPosition(cursorLeftForInput, cursorTopForInput);

		Console.CursorVisible = true;
	}
}
