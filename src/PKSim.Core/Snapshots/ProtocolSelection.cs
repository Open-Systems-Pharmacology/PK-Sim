using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public class ProtocolSelection: IWithName
   {
      public string Name { get; set; }
      public FormulationSelection[] Formulations { get; set; }
   }
}