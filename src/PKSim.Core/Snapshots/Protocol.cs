using System.Collections.Generic;
using Newtonsoft.Json;

namespace PKSim.Core.Snapshots
{
   public class Protocol : ParameterContainerSnapshotBase
   {
      //Simple protocol properties
      public string ApplicationType { get; set; }
      public string DosingInterval { get; set; }
      public string TargetOrgan { get; set; }
      public string TargetCompartment { get; internal set; }

      [JsonIgnore]
      public bool IsSimple => !string.IsNullOrEmpty(ApplicationType) && !string.IsNullOrEmpty(DosingInterval);

      //Advanced protocol properties
      public Schema[] Schemas { get; set; }

      public string TimeUnit { get; set; }
   }
}