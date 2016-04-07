using System;
using System.IO;
using LiteDB;
using static powertools.Utils;
using static powertools.Help;

namespace powertools
{
	public class sddapp
	{
		public const string appversion = "1.0.0";

		/// <summary>
		/// Save the current directory as a named location.
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			string locationname = string.Empty;
			bool showhelp = false;
			bool dolist = false;
			bool deleteName = false;
			bool force = false;

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
						deleteName = true;
					}
					else if(arg.MatchArg("^-f(orce)?$"))
					{
						force = true;
					}
					else if(arg.MatchArg("^.+$"))
					{
						locationname = arg;
					}
				}
			}

			if(showhelp)
			{
				ShowSDDHelp();
				return;
			}

			using(CSDB db = new CSDB(GetDBConnectionString()))
			{
				LiteCollection<NamedLocation> names = db.GetCollection<NamedLocation>(namedlocationtable);

				if(dolist || string.IsNullOrWhiteSpace(locationname))
				{
					names.DisplayAllNamedLocations();
				}
				else
				{
					if(deleteName)
					{
						if(names.Delete(x => x.Name == locationname) > 0)
						{
							Console.WriteLine($"Removed Named Location: {locationname}");
						}
						else
						{
							Console.WriteLine($"Named Location '{locationname}' does not exist.");
						}
					}
					else
					{
						if(names.Exists(x => x.Name == locationname))
						{
							if(!force)
							{
								// Prompt for overwriting existing name.
								Console.Write($"'{locationname}' already exists. Replace (y/N)? ");
								string response = Console.ReadLine();

								if(0 != string.Compare(response, "y", true))
								{
									return;
								}
							}

							// Remove any existing name.
							names.Delete(x => x.Name == locationname);
						}

						names.Insert(new NamedLocation { Name = locationname, Location = Directory.GetCurrentDirectory() });
						Console.WriteLine($"Add Named Location: {locationname}");
					}
				}
			}
		}
	}
}
