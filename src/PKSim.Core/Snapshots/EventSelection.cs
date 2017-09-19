using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public class EventSelection: IWithName
   {
      public string Name { get; set; }
      public Parameter StartTime { get; set; }
   }
}