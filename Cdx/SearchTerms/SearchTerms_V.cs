internal class SearchTerms_V
{
	public void Render(IEnumerable<string> values)
	{
		Console.SetCursorPosition(0, Console.CursorTop);
		Console.Write("\x1b[2K");
		Console.WriteLine(string.Join(" | ", values));
	}
}
