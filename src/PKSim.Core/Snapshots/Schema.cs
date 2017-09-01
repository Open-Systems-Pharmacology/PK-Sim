using System.Collections.Generic;

namespace PKSim.Core.Snapshots
{
   public class Schema : ParameterContainerSnapshotBase
   {
      public List<SchemaItem> SchemaItems { get; set; } = new List<SchemaItem>();
   }
}