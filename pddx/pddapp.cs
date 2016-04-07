using System;
using System.IO;
using LiteDB;
using static powertools.Help;
using static powertools.Utils;

namespace powertools
{
	class pddapp
	{
		public const string appversion = "1.0.0";

		/// <summary>
		/// Pop the last location off the stack before 'cdd' was used to change.
		/// This will work regardless of whether the last location was a named location.
		/// This does not affect any saved named locations. Only the location history.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		static int Main(string[] args)
		{
			bool showhelp = false;
			bool dolist = false;
			bool dodelete = false;
			bool doclear = false;

			// Parse arguments
			if(null != args)
			{
				foreach(string arg in args)
				{
					if(arg.MatchArg("^-h(elp)?$"))
					{
						showhelp = true;
					}
					else if(arg.MatchArg("^-l(ist)?$"))
					{
						dolist = true;
					}
					else if(arg.MatchArg("^-d(elete)?$"))
					{
						dodelete = true;
					}
					else if(arg.MatchArg("^-c(lear)?$"))
					{
						doclear = true;
					}
					else
					{
						Console.WriteLine($"Unknown argument: {arg}");
						ShowPDDHelp();
						return 0;
					}
				}
			}

			if(showhelp)
			{
				ShowPDDHelp();
				return 0;
			}

			int retval = 0;

			using(CSDB db = new CSDB(GetDBConnectionString()))
			{
				LiteCollection<LocationHistory> locationhistory = db.GetCollection<LocationHistory>(locationhistorytable);

				if(dolist)
				{
					locationhistory.DisplayAll();
				}
				else if(doclear)
				{
					// Delete all items
					locationhistory.Delete(Query.All());
				}
				else if(dodelete)
				{
					// Delete the most recent item
					LocationHistory location = locationhistory.GetMostRecentLocation();

					if(null != location)
					{
						locationhistory.DeleteLocation(location);
					}
				}
				else
				{
					LocationHistory location = locationhistory.GetMostRecentLocation();

					if(null != location)
					{
						if(Directory.Exists(location.Location))
						{
							if(IsLinux)
							{
								CreateLinuxPDScript(location);
							}
							else
							{
								CreateWinPDScript(location);
							}

							retval = 1;
						}
						else
						{
							Console.WriteLine($"Location no longer exists: {location.Location}");
						}

						// Remove this item
						locationhistory.DeleteLocation(location);
					}
				}
			}

			return retval;
		}

		/// <summary>
		/// Create the temporary script that the calling script will execute in order to
		/// change the parent process's currenty location. It can't be changed directly by
		/// this process.  This is the Linux Platform version.
		/// </summary>
		/// <param name="location"></param>
		private static void CreateLinuxPDScript(LocationHistory location)
		{
			string scriptfilename = Path.Combine(GetLocalAppDataFolder(), "popdir.sh");
			File.WriteAllLines(scriptfilename, new string[] {
							$"cd \"{location.Location}\""
						});
		}

		/// <summary>
		/// Create the temporary script that the calling script will execute in order to
		/// change the parent process's currenty location. It can't be changed directly by
		/// this process.  This is the Windows Platform version.
		/// </summary>
		/// <param name="location"></param>
		private static void CreateWinPDScript(LocationHistory location)
		{
			string scriptfilename = Path.Combine(GetLocalAppDataFolder(), "popdir.cmd");
			File.WriteAllLines(scriptfilename, new string[] {
							"@echo off",
							$"cd /d \"{location.Location}\""
						});
		}
	}
}
