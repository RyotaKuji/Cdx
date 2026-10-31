internal class SearchTerms(SearchTerms_V view)
{
	public IEnumerable<string> Values => values.Reverse();

	readonly Stack<string> values = [];

	public void Add(string value)
	{
		values.Push(value);
	}

	public void RemoveLast()
	{
		if (values.Count > 0)
		{
			values.Pop();
		}
	}

	public void Clear()
	{
		values.Clear();
	}

	public void Render()
	{
		view.Render(Values);
	}
}
