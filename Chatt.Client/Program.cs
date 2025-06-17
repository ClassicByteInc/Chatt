using System.Text;

namespace Chatt.Client;
internal class Program
{
	internal const int Port = 54256;
	internal static string Title = "Chatt";
	internal static int height = Console.WindowHeight;
	internal static List<string> History = [];
	static int scrollOffset = 0; // 新增：滚动偏移量
	internal static string currentPrompt = ">";
	internal static string _defaultServer = "localhost";

	static void Main()
	{
		Console.CancelKeyPress += Console_CancelKeyPress;
		AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
		Console.Clear();
		ShowTitle();

		while (true)
		{
			ShowHistoryPanel();

			Console.SetCursorPosition(0, height - 1);
			Console.Write(currentPrompt);
			var inputBuffer = new StringBuilder();
			int cursorPos = 0; // 新增：光标在缓冲区中的位置

			while (true)
			{
				if (Console.KeyAvailable)
				{
					var keyInfo = Console.ReadKey(true);
					if (keyInfo.Key == ConsoleKey.Enter)
					{
						break;
					}
					else if (keyInfo.Key == ConsoleKey.Backspace)
					{
						if (cursorPos > 0)
						{
							inputBuffer.Remove(cursorPos - 1, 1);
							cursorPos--;
							RedrawInputLine(inputBuffer, cursorPos);
						}
					}
					else if (keyInfo.Key == ConsoleKey.Delete)
					{
						if (cursorPos < inputBuffer.Length)
						{
							inputBuffer.Remove(cursorPos, 1);
							RedrawInputLine(inputBuffer, cursorPos);
						}
					}
					else if (keyInfo.Key == ConsoleKey.LeftArrow)
					{
						if (cursorPos > 0)
						{
							cursorPos--;
							Console.SetCursorPosition(currentPrompt.Length + cursorPos, height - 1);
						}
					}
					else if (keyInfo.Key == ConsoleKey.RightArrow)
					{
						if (cursorPos < inputBuffer.Length)
						{
							cursorPos++;
							Console.SetCursorPosition(currentPrompt.Length + cursorPos, height - 1);
						}
					}
					else if (keyInfo.Key == ConsoleKey.PageUp)
					{
						ScrollHistory(1, inputBuffer);
						Console.SetCursorPosition(currentPrompt.Length + cursorPos, height - 1);
					}
					else if (keyInfo.Key == ConsoleKey.PageDown)
					{
						ScrollHistory(-1, inputBuffer);
						Console.SetCursorPosition(currentPrompt.Length + cursorPos, height - 1);
					}
					else if (!char.IsControl(keyInfo.KeyChar))
					{
						inputBuffer.Insert(cursorPos, keyInfo.KeyChar);
						cursorPos++;
						RedrawInputLine(inputBuffer, cursorPos);
					}
				}
				Thread.Sleep(100);
			}

			string input = inputBuffer.ToString();
			if (string.IsNullOrWhiteSpace(input))
			{
				ClearInputLine(height);
				continue;
			}

			History.Add($"[{DateTime.Now:G}]{currentPrompt}'{input}'");
			scrollOffset = 0;
			ClearInputLine(height);
			new CommandParser().Parse(input);
			ShowTitle();
		}
	}

	private static void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
	{
		History.Add("如果要退出，请在输入quit。");
		ShowHistoryPanel();
		e.Cancel = true; // 阻止程序退出
	}

	private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		History.Add(((Exception)e.ExceptionObject).ToString());
		ShowHistoryPanel();
	}

	static void ShowTitle()
	{
		Console.SetCursorPosition(0, 0);
		Console.Write(new string(' ', Console.WindowWidth));
		Console.SetCursorPosition(0, 0);
		Console.WriteLine(Title);
	}

	static void ShowHistoryPanel()
	{
		int panelHeight = height - 2; // 除去title和输入行
		int total = History.Count;
		int start = Math.Max(0, total - panelHeight - scrollOffset);
		int end = Math.Max(0, total - scrollOffset);

		// 清空history区域
		for (int i = 1; i <= panelHeight; i++)
		{
			Console.SetCursorPosition(0, i);
			Console.Write(new string(' ', Console.WindowWidth));
		}

		// 显示history
		int line = 1;
		for (int i = start; i < end; i++)
		{
			Console.SetCursorPosition(0, line++);
			Console.Write(History[i]);
		}
	}

	static void ScrollHistory(int direction, StringBuilder? currentInput = null)
	{
		int panelHeight = height - 2;
		int maxOffset = Math.Max(0, History.Count - panelHeight);
		scrollOffset = Math.Clamp(scrollOffset + direction * panelHeight, 0, maxOffset);
		ShowHistoryPanel();

		// 恢复输入行
		Console.SetCursorPosition(0, height - 1);
		Console.Write(">");

		if (currentInput != null)
		{
			Console.Write(currentInput.ToString());
			// 将光标移到输入内容末尾
			Console.SetCursorPosition(1 + currentInput.Length, height - 1);
		}
		else
		{
			Console.SetCursorPosition(1, height - 1);
		}
	}

	static void ClearInputLine(int height)
	{
		Console.SetCursorPosition(0, height - 1);
		Console.Write(new string(' ', Console.WindowWidth));
		Console.SetCursorPosition(0, height - 1);
	}

	static void RedrawInputLine(StringBuilder buffer, int cursorPos)
	{
		Console.SetCursorPosition(0, height - 1);
		Console.Write(new string(' ', Console.WindowWidth));
		Console.SetCursorPosition(0, height - 1);
		Console.Write(currentPrompt);
		Console.Write(buffer.ToString());
		Console.SetCursorPosition(currentPrompt.Length + cursorPos, height - 1);
	}

	static void ProgramExit()
	{
		History.Add("正在退出。");
		History.Clear();
		
	}
}