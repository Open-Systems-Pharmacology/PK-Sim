using System.Collections.Generic;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ClassificationContext
   {
      public IReadOnlyCollection<OSPSuite.Core.Domain.Classification> Classifications { get; set; }
      public IReadOnlyCollection<IClassifiableWrapper> Classifiables { get; set; }
   }
}