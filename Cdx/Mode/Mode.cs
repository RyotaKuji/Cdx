internal class Mode
{
	public ActionMode Value { get; private set; } = ActionMode.Reference;
	public void Set(ActionMode value)
	{
		Value = value;
	}
	public enum ActionMode
	{
		Reference,
		FileChange,
	}
}
