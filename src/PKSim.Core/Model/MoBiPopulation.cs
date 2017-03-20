using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class MoBiPopulation : Population
   {
      private readonly OriginData _originData;
      private int _numberOfItems;

      public MoBiPopulation()
      {
         _originData = new OriginData();
      }

      public override Individual FirstIndividual
      {
         get { return null; }
      }

      public override int NumberOfItems
      {
         get { return _numberOfItems; }
      }

      public virtual void SetNumberOfItems(int numberOfItems)
      {
         _numberOfItems = numberOfItems;
      }

      public override OriginData OriginData
      {
         get { return _originData; }
      }

      public override bool IsPreterm
      {
         get { return false; }
      }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var population = sourceObject as MoBiPopulation;
         if (population == null) return;
         _numberOfItems = population.NumberOfItems;
      }
   }
}