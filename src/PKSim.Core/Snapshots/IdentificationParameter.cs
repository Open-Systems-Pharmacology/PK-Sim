using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public class IdentificationParameter : ParameterContainerSnapshotBase
   {
      public Scalings Scaling { get; set; }
      public bool? UseAsFactor { get; set; }
      public bool? IsFixed { get; set; }
      public string[] LinkedParameters { get; set; }
   }
}