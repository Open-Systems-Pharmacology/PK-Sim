using System.Collections.Generic;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ClassificationContext
   {
      public IReadOnlyList<OSPSuite.Core.Domain.Classification> Classifications { get; set; }
      public IReadOnlyList<OSPSuite.Core.Domain.IClassifiableWrapper> Classifiables { get; set; }
   }
}