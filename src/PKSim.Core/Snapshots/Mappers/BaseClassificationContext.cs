using System.Collections.Generic;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots.Mappers
{
   public abstract class BaseClassificationContext<TClassification, TClassifiable>
   {
      public IReadOnlyList<TClassification> Classifications { get; set; }
      public IReadOnlyList<TClassifiable> Classifiables { get; set; }
   }

   public class ObservedDataClassificationContext : BaseClassificationContext<Classification, ClassifiableObservedData>
   {

   }
}