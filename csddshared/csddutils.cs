using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LiteDB;

namespace powertools
{
	public static class Utils
	{
		public const int dbversion = 1;
		public const string namedlocationtable = "namedlocation";
		public const string locationhistorytable = "locationhistory";

		private static string dbname = null;

		public static bool IsLinux
		{
			get
			{
				PlatformID pid = Environment.OSVersion.Platform;
				return (PlatformID.Unix == pid) || (PlatformID.MacOSX == pid);
			}
		}

		public static string GetLocalAppDataFolder()
		{
			string appfolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "powertools");

			// Make sure it exists
			Directory.CreateDirectory(appfolder);
			return appfolder;
		}

		public static string GetDBFilename()
		{
			if(null == dbname)
			{
				dbname = Path.Combine(GetLocalAppDataFolder(), "cdddatabase.db");
			}

			return dbname;
		}

		public static string GetDBConnectionString()
		{
			return $"filename={GetDBFilename()};version={dbversion}";
		}

		public static bool MatchArg(this string arg, string expr)
		{
			return Regex.IsMatch(arg, expr, RegexOptions.IgnoreCase);
		}

		public static void ForEach<T>(this IEnumerable<T> enumerableCollection, Action<T> action)
		{
			if(null == enumerableCollection || null == action)
			{
				return;
			}

			foreach(var item in enumerableCollection)
			{
				action(item);
			}
		}

		/// <summary>
		/// Get the most recent item
		/// </summary>
		/// <param name="locationhistory"></param>
		/// <returns>null if no history.</returns>
		public static LocationHistory GetMostRecentLocation(this LiteCollection<LocationHistory> locationhistory)
		{
			return locationhistory.FindAll()
						.OrderByDescending(x => x.Date)
						.DefaultIfEmpty(null)
						.FirstOrDefault();
		}

		public static void DisplayAllNamedLocations(this LiteCollection<NamedLocation> names)
		{
			Console.WriteLine("Name                 Location");
			Console.WriteLine("-----                ---------");
			names.FindAll()
				.ForEach(namedlocation => Console.WriteLine($"{namedlocation.Name,-20} {namedlocation.Location}"));
		}

		public static void DisplayAll(this LiteCollection<LocationHistory> locationhistory)
		{
			Console.WriteLine("Location");
			Console.WriteLine("---------");
			locationhistory.FindAll()
				.OrderByDescending(x => x.Date)
				.ForEach(location => Console.WriteLine($"{location.Location}"));
		}

		public static void DeleteLocation(this LiteCollection<LocationHistory> locationhistory, LocationHistory location)
		{
			locationhistory.Delete(x => x.Date == location.Date);
		}
	}
}
