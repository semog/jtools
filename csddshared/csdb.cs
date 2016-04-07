using LiteDB;
using static powertools.Utils;

namespace powertools
{
	public class CSDB : LiteDatabase
	{
		public CSDB(string connectionString)
			: base(connectionString)
		{
		}

		protected override void OnVersionUpdate(int newVersion)
		{
			if(1 == newVersion)
			{
				// Do init stuff
				LiteCollection<NamedLocation> names = this.GetCollection<NamedLocation>(namedlocationtable);
				LiteCollection<LocationHistory> locationhistory = this.GetCollection<LocationHistory>(locationhistorytable);

				// Set the indexes
				names.EnsureIndex(x => x.Name, new IndexOptions() { IgnoreCase = true, Unique = true });
				locationhistory.EnsureIndex(x => x.Date, new IndexOptions() { IgnoreCase = true });
			}
			else if(2 == newVersion)
			{
				// Do update version 1 -> 2 stuff
			}
		}
	}
}
