using System.Collections.Generic;

namespace PKSim.Core.Snapshots
{
   public class DataRepository : SnapshotBase
   {
      public ExtendedProperties ExtendedProperties { get; set; }
      public List<DataColumn> Columns { set; get; }
      public DataColumn BaseGrid { set; get; }
   }
}
