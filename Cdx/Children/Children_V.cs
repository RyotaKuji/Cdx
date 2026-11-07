using System.Globalization;
using System.Text;
using Spectre.Console;
using Spectre.Console.Rendering;

internal class Children_V(Children model)
{
	int page = 0;
	bool showTail = false;

	public IRenderable GetComp(int columnsCount, int width, int height)
	{
		List<FileSystemInfo> values = model.FilteredValue.ToList();

		if (values.Count == 0)
		{
			return new Grid()
				.AddColumn()
				.AddRow(new Text("1 / 1  ") { Justification = Justify.Right })
				.AddRow(new Text("(No items found.)"))
				.Expand();
		}

		Grid grid = new Grid()
			.AddColumns(columnsCount)
			.Expand();

		int innerHeight = height - 1;
		int itemsCountPerPage = columnsCount * innerHeight;
		int totalPages = (int)Math.Ceiling((double)values.Count / itemsCountPerPage);
		page = Math.Clamp(page, 0, totalPages - 1);
		int startIndex = itemsCountPerPage * page;
		int maxWidth = width / columnsCount - 2;

		Text[] pageIndexRow = Enumerable.Repeat(Text.Empty, columnsCount - 1)
			.Append(new Text($"{page + 1} / {totalPages}  ") { Justification = Justify.Right })
			.ToArray();

		grid.AddRow(pageIndexRow);

		for (int i = 0; i < innerHeight; i++)
		{
			List<FileSystemInfo> row = [];
			for (int j = 0; j < columnsCount; j++)
			{
				int index = startIndex + innerHeight * j + i;
				if (index < values.Count)
				{
					row.Add(values[index]);
				}
			}
			grid.AddRow(row.Select(
				info => $"{GetItemMarker(info)} {TruncateByWidth(info.Name, maxWidth - 2, showTail)}"
			).ToArray());
		}

		return grid;
	}

	public void PageUp() => page--;

	public void PageDown() => page++;

	public void ShowHead() => showTail = false;

	public void ShowTail() => showTail = true;

	static string GetItemMarker(FileSystemInfo item)
	{
		return item is DirectoryInfo ? "📁" : "📄";
	}

	static string TruncateByWidth(string text, int maxWidth, bool showTail)
	{
		if (maxWidth <= 0)
		{
			return string.Empty;
		}

		int width = 0;
		var result = new StringBuilder();

		IEnumerable<Rune> enumerator = text.EnumerateRunes();
		if (showTail)
		{
			enumerator = enumerator.Reverse();
		}

		foreach (Rune rune in enumerator)
		{
			var category = Rune.GetUnicodeCategory(rune);

			int charWidth =
				category is UnicodeCategory.NonSpacingMark
					or UnicodeCategory.EnclosingMark
					or UnicodeCategory.Format
					or UnicodeCategory.Control
				? 0
				: IsWide(rune.Value) ? 2 : 1;

			if (width + charWidth > maxWidth)
			{
				break;
			}

			result.Append(rune);
			width += charWidth;
		}

		if (showTail)
		{
			return string.Concat(result.ToString().EnumerateRunes().Reverse());
		}

		return result.ToString();
	}

	static bool IsWide(int code)
	{
		return
			// CJK 統合漢字・日本語
			(code >= 0x1100 && code <= 0x11FF) ||
			(code >= 0x2E80 && code <= 0xA4CF) ||
			(code >= 0xAC00 && code <= 0xD7A3) ||
			(code >= 0xF900 && code <= 0xFAFF) ||

			// 全角英数字・記号
			(code >= 0xFE10 && code <= 0xFE6F) ||
			(code >= 0xFF01 && code <= 0xFF60) ||
			(code >= 0xFFE0 && code <= 0xFFE6) ||

			// 絵文字など
			(code >= 0x1F300 && code <= 0x1FAFF) ||
			(code >= 0x20000 && code <= 0x3FFFF);
	}
}
