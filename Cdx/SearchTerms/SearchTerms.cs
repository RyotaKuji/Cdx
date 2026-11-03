internal class SearchTerms
{
	public IEnumerable<string> Values => values.Reverse();

	readonly Stack<string> values = [];

	public void Add(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return;
		}
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
}
