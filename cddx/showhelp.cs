using System;

namespace powertools
{
	public static class Help
	{
		public static void ShowCDDHelp()
		{
			Console.WriteLine($"Change Named Directory Tool {cddapp.appversion}");
			Console.WriteLine(@"
Usage:
    cdd [-h] [-l] <name>

Where:
    -h, -help           - Show this help screen.
    -l, -list           - List all saved named locations. (Optional)
    <name>              - Custom name to change into.

Description:
    This will change the current directory to a saved custom name (or alias).  Use the 'sdd'
    companion utility to save custom location names.  If a name is not specified, then the
    list of saved named locations will be displayed.
");
		}
	}
}
