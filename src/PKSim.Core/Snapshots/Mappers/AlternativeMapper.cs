using System.Threading.Tasks;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Snapshots.Mappers
{
   public class AlternativeMapper : ParameterContainerSnapshotMapperBase<ParameterAlternative, Alternative, ParameterAlternativeGroup>
   {
      private readonly ISpeciesRepository _speciesRepository;
      private readonly IParameterAlternativeFactory _parameterAlternativeFactory;

      public AlternativeMapper(ParameterMapper parameterMapper,
         IParameterAlternativeFactory parameterAlternativeFactory,
         ISpeciesRepository speciesRepository) : base(parameterMapper)
      {
         _speciesRepository = speciesRepository;
         _parameterAlternativeFactory = parameterAlternativeFactory;
      }

      public override Task<Alternative> MapToSnapshot(ParameterAlternative parameterAlternative)
      {
         if (parameterAlternative.IsCalculated)
            return Task.FromResult((Alternative)null);

         return SnapshotFrom(parameterAlternative, snapshot =>
         {
            snapshot.IsDefault = parameterAlternative.IsDefault;
            snapshot.Species = (parameterAlternative as ParameterAlternativeWithSpecies)?.Species.Name;
         });
      }

      public override async Task<ParameterAlternative> MapToModel(Alternative snapshot, ParameterAlternativeGroup parameterAlternativeGroup)
      {
         var alternative = _parameterAlternativeFactory.CreateAlternativeFor(parameterAlternativeGroup);
         alternative.IsDefault = snapshot.IsDefault;
         MapSnapshotPropertiesToModel(snapshot, alternative);
         await UpdateParametersFromSnapshot(snapshot, alternative, parameterAlternativeGroup.Name);

         var alternativeWithSpecies = alternative as ParameterAlternativeWithSpecies;
         if (alternativeWithSpecies == null)
            return alternative;

         var species = _speciesRepository.FindByName(snapshot.Species);
         if (species == null)
            throw new SnapshotOutdatedException(PKSimConstants.Error.CouldNotFindSpecies(snapshot.Species, _speciesRepository.AllNames()));

         alternativeWithSpecies.Species = species;
         return alternativeWithSpecies;
      }
   }
}