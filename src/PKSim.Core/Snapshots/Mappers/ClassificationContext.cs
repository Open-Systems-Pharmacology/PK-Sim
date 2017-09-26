using System.Collections.Generic;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ClassificationContext
   {
      public IReadOnlyList<OSPSuite.Core.Domain.Classification> Classifications { get; set; }
      public IReadOnlyList<IClassifiableWrapper> Classifiables { get; set; }
   }
}