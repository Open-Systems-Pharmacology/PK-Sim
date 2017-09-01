using System.Collections.Generic;
using Newtonsoft.Json;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public interface ISnapshot : IWithName, IWithDescription
   {
      
   }

   public abstract class SnapshotBase : ISnapshot
   {
      //We want the name to be the first entry in the snapshot. Default value for order is -1 so set to -2
      [JsonProperty(Order = -2)]
      public string Name { get; set; }

      public string Description { get; set; }
   }

   public abstract class ParameterContainerSnapshotBase : SnapshotBase
   {
      public List<Parameter> Parameters { get; set; } = new  List<Parameter>();
   }
}