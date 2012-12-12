using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Waveface.Stream.WindowsClient
{
	public class CommandLineHelper
	{
		#region Const
		private const string NAME_MATCH_GROUP = "name";
		private const string VALUE_MATCH_GROUP = "value";
		#endregion

		private static readonly Regex ArgRegex =
			new Regex(@"(?<name>[^=\s]+)=?((?<quoted>\""?)(?<value>(?(quoted)[^\""]+|[^,]+))\""?,?)*",
				RegexOptions.Compiled | RegexOptions.CultureInvariant |
				RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);



		public static void ProcessCommandLineArgs(Action printUsage, params CommandLineCommand[] commands)
		{
			var args = Environment.GetCommandLineArgs().Skip(1);
			if ((from arg in args
				 from Match match in ArgRegex.Matches(arg)
				 from command in commands
				 where match.Success &&
					 ((string.Compare(match.Groups[NAME_MATCH_GROUP].Value, command.Name, true) == 0) ||
					 (string.Compare(match.Groups[NAME_MATCH_GROUP].Value, command.ShortForm, true) == 0))
				 select command.InvokeHandler(match.Groups[VALUE_MATCH_GROUP].Value.Split(','))).Sum() == 0)
				printUsage();
		}
	}
}
