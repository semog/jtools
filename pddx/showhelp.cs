using System;

namespace powertools
{
	public static class Help
	{
		public static void ShowPDDHelp()
		{
			Console.WriteLine($"Pop Last Directory Tool {pddapp.appversion}");
			Console.WriteLine(@"
Usage:
    pdd [-h] [-n] [-d] [-c] [-l]

Where:
    -h, -help           - Show this help screen.
    -d, -delete         - Delete the last saved folder location without changing to it.
    -c, -clear          - Clear all saved history locations.
    -l, -list           - List all saved history locations.

Description:
    This will 'pop' the last directory before the cdd command was used to change location.
");
		}
	}
}
