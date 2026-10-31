using System.ComponentModel;
using System.Runtime.InteropServices;

internal static class ConsoleInput
{
	private const int STD_INPUT_HANDLE = -10;
	private const ushort KEY_EVENT = 0x0001;

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern IntPtr GetStdHandle(int nStdHandle);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool WriteConsoleInputW(
		IntPtr hConsoleInput,
		INPUT_RECORD[] lpBuffer,
		uint nLength,
		out uint lpNumberOfEventsWritten);

	[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
	private struct INPUT_RECORD
	{
		[FieldOffset(0)]
		public ushort EventType;

		// INPUT_RECORD の union 部分。
		// KEY_EVENT_RECORD は offset 4 から始まる。
		[FieldOffset(4)]
		public KEY_EVENT_RECORD KeyEvent;
	}

	[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
	private struct KEY_EVENT_RECORD
	{
		[FieldOffset(0)]
		[MarshalAs(UnmanagedType.Bool)]
		public bool KeyDown;

		[FieldOffset(4)]
		public ushort RepeatCount;

		[FieldOffset(6)]
		public ushort VirtualKeyCode;

		[FieldOffset(8)]
		public ushort VirtualScanCode;

		[FieldOffset(10)]
		public char UnicodeChar;

		[FieldOffset(12)]
		public uint ControlKeyState;
	}

	public static void Write(string text)
	{
		IntPtr inputHandle = GetStdHandle(STD_INPUT_HANDLE);

		if (inputHandle == IntPtr.Zero || inputHandle == new IntPtr(-1))
		{
			throw new Win32Exception(Marshal.GetLastWin32Error());
		}

		var records = new INPUT_RECORD[text.Length];

		for (int i = 0; i < text.Length; i++)
		{
			records[i] = new INPUT_RECORD
			{
				EventType = KEY_EVENT,
				KeyEvent = new KEY_EVENT_RECORD
				{
					KeyDown = true,
					RepeatCount = 1,
					VirtualKeyCode = 0,
					VirtualScanCode = 0,
					UnicodeChar = text[i],
					ControlKeyState = 0
				}
			};
		}

		if (!WriteConsoleInputW(
				inputHandle,
				records,
				(uint)records.Length,
				out uint written))
		{
			throw new Win32Exception(Marshal.GetLastWin32Error());
		}

		if (written != records.Length)
		{
			throw new InvalidOperationException(
				$"Console input was not completely written. {written}/{records.Length}");
		}
	}
}
