using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ClassifiableContext
   {
      public IClassification Parent { set; get; }
      public string Name { set; get; }
   }
}