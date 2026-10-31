internal class SearchTerms_V
{
	const int CursorTop = 2;
	public void Render(IEnumerable<string> values)
	{
		Console.SetCursorPosition(0, CursorTop);
		Console.Write("\x1b[2K");
		Console.WriteLine(string.Join(" | ", values));
	}
}
