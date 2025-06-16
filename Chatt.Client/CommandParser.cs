using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Chatt.Client
{
	internal class CommandParser
	{
		public CommandParser() { }

		public CommandParser Parse(String command)
		{
			if (string.IsNullOrWhiteSpace(command))
			{
				throw new ArgumentException("Command cannot be null or empty.", nameof(command));
			}
			var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length == 0)
			{
				throw new ArgumentException("Command cannot be empty.", nameof(command));
			}
			try
			{
				switch (parts[0].ToLower())
				{
					case "help":
						Program.History.Add("here is help");
						break;
					case "throw":
						throw new IOException("WDNMD");
					case "exit":
						Environment.Exit(0);
						break;
					case "login":
						break;
					case "logout":
						break;
					case "reg":
						break;
					case "send":
						break;
					case "sendf":
						break;
					case "seleteusr":
						break;
					default:
						Program.History.Add($"[{DateTime.Now:G}]Unknown Command:'{parts[0]}'");
						break;
				}
			}
			catch (IndexOutOfRangeException)
			{
				throw;
			}
			return this;
		}

	}
}
