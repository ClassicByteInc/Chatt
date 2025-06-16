using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatt.Core
{
	internal class Util
	{
		public static string GetWorkspace()
		{
			return Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".Chatt")).FullName;
		}
	}
}
