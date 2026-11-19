using System.Data;
using System.Globalization;
using System.Numerics;
using System.Text;
using Spectre.Console;
using Spectre.Console.Rendering;

internal class Children_V(Children model)
{
	int page = 0;
	bool showTail = false;

	public IRenderable GetComp(int width, int height)
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

		int innerHeight = height - 1;

		if (innerHeight <= 0)
		{
			return new Grid()
				.AddColumn()
				.AddRow(new Text("1 / 1  ") { Justification = Justify.Right })
				.AddRow(new Text("(Window height is too small.)"))
				.Expand();
		}

		int totalColumnsCount = (values.Count + innerHeight - 1) / innerHeight;

		int maxItemWidth = values.Select(info => GetStringWidth(GetDisplayedItemName(info, width, false))).Max();
		int columnsCountPerPage = Math.Max(width / maxItemWidth, Math.Min(totalColumnsCount, 4));
		int columnWidth = width / columnsCountPerPage;

		int itemsCountPerPage = columnsCountPerPage * innerHeight;
		int totalPages = (values.Count + itemsCountPerPage - 1) / itemsCountPerPage;

		page = Math.Clamp(page, 0, totalPages - 1);
		int startIndex = itemsCountPerPage * page;

		Text[] pageIndexRow = Enumerable.Repeat(Text.Empty, columnsCountPerPage - 1)
			.Append(new Text($"{page + 1} / {totalPages}  ") { Justification = Justify.Right })
			.ToArray();

		Grid grid = new Grid()
			.AddColumns(columnsCountPerPage)
			.Expand();

		grid.AddRow(pageIndexRow);

		for (int i = 0; i < innerHeight; i++)
		{
			List<FileSystemInfo> row = [];
			for (int j = 0; j < columnsCountPerPage; j++)
			{
				int index = startIndex + innerHeight * j + i;
				if (index < values.Count)
				{
					row.Add(values[index]);
				}
			}
			grid.AddRow(row.Select(
				info => Markup.Escape(GetDisplayedItemName(info, columnWidth, showTail))
			).ToArray());
		}

		return grid;
	}

	public void PageUp() => page--;

	public void PageDown() => page++;

	public void ShowHead() => showTail = false;

	public void ShowTail() => showTail = true;

	static string GetDisplayedItemName(FileSystemInfo info, int maxWidth, bool showTail)
	{
		string marker = info is DirectoryInfo ? "📁" : "📄";
		string text = TruncateByWidth(info.Name, maxWidth - 5, showTail);
		return $"{marker} {text}  ";
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

	static int GetStringWidth(string text)
	{
		int width = 0;
		foreach (Rune rune in text.EnumerateRunes())
		{
			var category = Rune.GetUnicodeCategory(rune);
			int charWidth =
				category is UnicodeCategory.NonSpacingMark
					or UnicodeCategory.EnclosingMark
					or UnicodeCategory.Format
					or UnicodeCategory.Control
				? 0
				: IsWide(rune.Value) ? 2 : 1;
			width += charWidth;
		}
		return width;
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
