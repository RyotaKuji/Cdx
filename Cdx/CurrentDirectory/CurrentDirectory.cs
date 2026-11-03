internal class CurrentDirectory
{
	public string Value { get; private set; } = string.Empty;

	public void SetValue(string value)
	{
		Value = value;
		Directory.SetCurrentDirectory(value);
	}
}
