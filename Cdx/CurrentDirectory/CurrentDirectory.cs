internal class CurrentDirectory
{
	public DirectoryInfo Value { get; private set; } = new(Directory.GetCurrentDirectory());

	public void SetValue(DirectoryInfo value)
	{
		Value = value;
		Directory.SetCurrentDirectory(value.FullName);
	}
}
