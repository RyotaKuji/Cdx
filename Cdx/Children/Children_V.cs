using System.Globalization;
using System.Text;
using Spectre.Console;
using Spectre.Console.Rendering;

internal class Children_V(Children model)
{
	public IRenderable GetComp(int columnsCount, int width, int height, int page)
	{
		Grid grid = new Grid()
			.AddColumns(columnsCount);

		List<FileSystemInfo> values = model.FilteredValue.ToList();

		int itemsCountPerPage = columnsCount * height;
		int startIndex = itemsCountPerPage * page;
		int maxWidth = width / columnsCount - 2;

		for (int i = 0; i < height; i++)
		{
			List<FileSystemInfo> row = [];
			for (int j = 0; j < columnsCount; j++)
			{
				int index = startIndex + height * j + i;
				if (index < values.Count)
				{
					row.Add(values[index]);
				}
			}
			grid.AddRow(row.Select(GetItemText).Select(text => TruncateByWidth(text, maxWidth)).ToArray());
		}

		return grid;
	}

	static string GetItemText(FileSystemInfo item)
	{
		string marker = item is DirectoryInfo ? "📁" : "📄";
		return $"{marker} {item.Name}";
	}

	static string TruncateByWidth(string text, int maxWidth)
	{
		if (maxWidth <= 0)
		{
			return string.Empty;
		}

		int width = 0;
		var result = new StringBuilder();

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

			if (width + charWidth > maxWidth)
			{
				break;
			}

			result.Append(rune);
			width += charWidth;
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
