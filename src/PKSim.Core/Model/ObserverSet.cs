using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core.Model
{
   public class ObserverSet : PKSimBuildingBlock
   {
      public IEnumerable<IObserverBuilder> Observers => GetChildren<IObserverBuilder>();

      public ObserverSet() : base(PKSimBuildingBlockType.ObserverSet)
      {
      }
   }
}