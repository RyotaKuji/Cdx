internal class SearchTerms(SearchTerms_V view)
{
	public IEnumerable<string> Values => values.Reverse();

	readonly Stack<string> values = [];

	public void Add(string value)
	{
		values.Push(value);
		view.Render(Values);
	}

	public void Remove()
	{
		if (values.Count > 0)
		{
			values.Pop();
			view.Render(Values);
		}
	}

	public void Clear()
	{
		values.Clear();
		view.Render(Values);
	}
}
