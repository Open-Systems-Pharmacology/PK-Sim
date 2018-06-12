using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class ParameterAlternativeWithSpecies : ParameterAlternative, ISpeciesDependentEntity
   {
      private Species _species;

      public virtual Species Species
      {
         get => _species;
         set => SetProperty(ref _species, value);
      }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceAlternative = sourceObject as ParameterAlternativeWithSpecies;
         if (sourceAlternative == null) return;
         Species = sourceAlternative.Species;
      }
   }
}