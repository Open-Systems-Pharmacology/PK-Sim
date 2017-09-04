using System.Collections.Generic;

namespace PKSim.Core.Snapshots
{
   public class Molecule : ParameterContainerSnapshotBase
   {
      public string Type { get; set; }
      //Proteins only
      public string MembraneLocation { get; set; }

      public string TissueLocation { get; set; }
      public string IntracellularVascularEndoLocation { get; set; }

      //Transporters only
      public string TransportType { get; set; }

      public List<LocalizedParameter> Expression { get; set; } = new List<LocalizedParameter>();
   }
}