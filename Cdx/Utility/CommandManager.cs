internal class CommandManager
{
	public static readonly ConsoleKeyInfo BackSpace = new((char)ConsoleKey.Backspace, ConsoleKey.Backspace, control: false, shift: false, alt: false);
	public static readonly ConsoleKeyInfo Enter = new((char)ConsoleKey.Enter, ConsoleKey.Enter, control: false, shift: false, alt: false);

	static readonly IEnumerable<ConsoleKeyInfo> Commands = [
		BackSpace,
		Enter,
	];

	public static bool IsRecognized(ConsoleKeyInfo keyInfo)
	{
		return Commands.Contains(keyInfo);
	}
}
