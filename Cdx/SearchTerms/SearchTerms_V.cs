internal class SearchTerms_V
{
	public void Render(IEnumerable<string> values)
	{
		Console.WriteLine(string.Join(" | ", values));
	}
}
