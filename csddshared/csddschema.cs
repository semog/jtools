using System;

namespace powertools
{
	public class NamedLocation
	{
		public string Name { get; set; }
		public string Location { get; set; }
	}

	public class LocationHistory
	{
		public long Date { get; set; }
		public string Location { get; set; }
	}
}
