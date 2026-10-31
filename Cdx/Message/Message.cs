internal class Message(Message_V view)
{
	public string Value { get; private set; } = string.Empty;
	public bool HasValue => string.IsNullOrEmpty(Value) == false;

	public void SetValue(string value)
	{
		Value = value;
	}

	public void Render()
	{
		view.Render(Value);
		Value = string.Empty;
	}
}
