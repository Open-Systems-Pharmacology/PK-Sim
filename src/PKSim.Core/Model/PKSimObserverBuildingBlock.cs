using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core.Model
{
   public class PKSimObserverBuildingBlock : PKSimBuildingBlock
   {
      public IEnumerable<IObserverBuilder> Observers => GetChildren<IObserverBuilder>();

      public PKSimObserverBuildingBlock() : base(PKSimBuildingBlockType.Observers)
      {
      }
   }
}