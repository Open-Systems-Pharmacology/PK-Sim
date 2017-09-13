using System;
using System.Collections.Generic;

namespace PKSim.Core.Snapshots
{
   public class ExtendedProperty : SnapshotBase
   {
      public Type Type { set; get; }
      public object Value { set; get; }
      public List<object> ListOfValues { set; get; }
      public string FullName { get; set; }
      public string DisplayName { get; set; }
      public bool ReadOnly { get; set; }
   }
}
