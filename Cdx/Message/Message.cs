internal class Message
{
	public string Value { get; private set; } = string.Empty;

	public void SetValue(string value)
	{
		Value = value;
		//Value = new(value.Where(c => char.IsControl(c) == false).ToArray());
	}
}
