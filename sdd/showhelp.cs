using System;

namespace powertools
{
	public static class Help
	{
		public static void ShowSDDHelp()
		{
			Console.WriteLine($"Save Named Directory Tool {sddapp.appversion}");
			Console.WriteLine(@"
Usage:
    sdd [-h] [-l] [-d] <name>

Where:
    -h, -help           - Show this help screen.
    -l, -list           - List all saved named locations. (Optional)
    -f, -force          - Force replacing existing named location without prompting.
    -d, -delete         - Delete the specified name
    <name>              - Custom name to save for the current directory.

Description:
    This will save a custom name (or alias) for the currenty directory. Use the 'cdd'
    companion utility to quickly move to the named directory. If a name is not specified,
    then the list of saved named locations will be displayed.
");
		}
	}
}
