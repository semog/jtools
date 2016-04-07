using System;
using System.IO;
using LiteDB;
using static powertools.Help;
using static powertools.Utils;

namespace powertools
{
	class cddapp
	{
		public const string appversion = "1.0.0";

		/// <summary>
		/// Change directory to a named location.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		static int Main(string[] args)
		{
			string locationname = string.Empty;
			bool showhelp = false;
			bool dolist = false;

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
					else if(arg.MatchArg("^.+$"))
					{
						locationname = arg;
					}
				}
			}

			if(showhelp)
			{
				ShowCDDHelp();
				return 0;
			}

			int retval = 0;

			using(CSDB db = new CSDB(GetDBConnectionString()))
			{
				LiteCollection<NamedLocation> names = db.GetCollection<NamedLocation>(namedlocationtable);

				if(dolist || string.IsNullOrWhiteSpace(locationname))
				{
					names.DisplayAllNamedLocations();
				}
				else
				{
					NamedLocation namedlocation = names.FindOne(x => x.Name == locationname);

					if(null != namedlocation)
					{
						if(Directory.Exists(namedlocation.Location))
						{
							if(IsLinux)
							{
								CreateLinuxCDScript(namedlocation);
							}
							else
							{
								CreateWinCDScript(namedlocation);
							}

							PushCurrentLocation(db);
							retval = 1;
						}
						else
						{
							Console.WriteLine($"Location no longer exists: {namedlocation.Location}");
						}
					}
					else
					{
						Console.WriteLine($"Unknown Named Location: {locationname}");
					}
				}
			}

			return retval;
		}

		/// <summary>
		/// Save the currenty directory into a history table that pddx will "pop".
		/// </summary>
		/// <param name="db"></param>
		private static void PushCurrentLocation(CSDB db)
		{
			LiteCollection<LocationHistory> locationhistory = db.GetCollection<LocationHistory>(locationhistorytable);
			locationhistory.Insert(new LocationHistory { Date = DateTime.UtcNow.ToFileTime(), Location = Directory.GetCurrentDirectory() });
		}

		/// <summary>
		/// Create the temporary script that the calling script will execute in order to
		/// change the parent process's currenty location. It can't be changed directly by
		/// this process.  This is the Linux Platform version.
		/// </summary>
		/// <param name="namedlocation"></param>
		private static void CreateLinuxCDScript(NamedLocation namedlocation)
		{
			string scriptfilename = Path.Combine(GetLocalAppDataFolder(), "changedir.sh");
			File.WriteAllLines(scriptfilename, new string[] {
							$"cd \"{namedlocation.Location}\""
						});
		}

		/// <summary>
		/// Create the temporary script that the calling script will execute in order to
		/// change the parent process's currenty location. It can't be changed directly by
		/// this process.  This is the Windows Platform version.
		/// </summary>
		/// <param name="namedlocation"></param>
		private static void CreateWinCDScript(NamedLocation namedlocation)
		{
			string scriptfilename = Path.Combine(GetLocalAppDataFolder(), "changedir.cmd");
			File.WriteAllLines(scriptfilename, new string[] {
							"@echo off",
							$"cd /d \"{namedlocation.Location}\""
						});
		}
	}
}
