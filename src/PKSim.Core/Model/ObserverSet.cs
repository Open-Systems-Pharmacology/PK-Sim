using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core.Model
{
   public class ObserverSet : PKSimBuildingBlock
   {
      public IEnumerable<ObserverBuilder> Observers => GetChildren<ObserverBuilder>();

      public ObserverSet() : base(PKSimBuildingBlockType.ObserverSet)
      {
      }
   }
}