﻿namespace PKSim.Core.Snapshots
{
   public class DataRepository : SnapshotBase
   {
      public ExtendedProperty[] ExtendedProperties { get; set; }
      public DataColumn[] Columns { set; get; }
      public DataColumn BaseGrid { set; get; }
   }
}