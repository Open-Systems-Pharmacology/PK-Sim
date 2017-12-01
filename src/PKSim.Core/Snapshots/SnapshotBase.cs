using Newtonsoft.Json;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public abstract class SnapshotBase : IWithName, IWithDescription
   {
      //We want the name to be the first entry in the snapshot. Default value for order is -1 so set to -2
      [JsonProperty(Order = -2)]
      public string Name { get; set; }

      public string Description { get; set; }

      public override string ToString()
      {
         return Name ?? base.ToString();
      }
   }

   public abstract class ParameterContainerSnapshotBase : SnapshotBase
   {
      public Parameter[] Parameters { get; set; }
   }
}