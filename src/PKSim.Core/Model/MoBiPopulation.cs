using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class MoBiPopulation : Population
   {
      private int _numberOfItems;

      public override Individual FirstIndividual => null;

      public override int NumberOfItems => _numberOfItems;

      public virtual void SetNumberOfItems(int numberOfItems)
      {
         _numberOfItems = numberOfItems;
      }

      public override OriginData OriginData { get; } = new OriginData();

      public override bool IsPreterm { get; } = false;

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var population = sourceObject as MoBiPopulation;
         if (population == null) return;
         _numberOfItems = population.NumberOfItems;
      }
   }
}