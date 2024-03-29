﻿using System.Collections.Generic;

namespace PKSim.Core.Snapshots
{
   public enum ExtendedPropertyType
   {
      String,
      Integer,
      Double,
      Boolean,
   }

   public class ExtendedProperty : SnapshotBase
   {
      public object Value { set; get; }
      public List<object> ListOfValues { set; get; }
      public string FullName { get; set; }
      public bool? ReadOnly { get; set; }
      public ExtendedPropertyType? Type { get; set;}
   }
}
