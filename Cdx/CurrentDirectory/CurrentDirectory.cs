internal class CurrentDirectory(CurrentDirectory_V view)
{
	public string Value { get; private set; } = string.Empty;

	public void SetValue(string value)
	{
		Value = value;
		Directory.SetCurrentDirectory(value);
	}

	public void Render()
	{
		view.Render(Value);
	}
}
