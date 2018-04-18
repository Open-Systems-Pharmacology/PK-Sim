using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core.Snapshots.Mappers
{
   public class AlternativeMapper : ParameterContainerSnapshotMapperBase<ParameterAlternative, Alternative, ParameterAlternativeGroup>
   {
      private readonly ISpeciesRepository _speciesRepository;
      private readonly IParameterAlternativeFactory _parameterAlternativeFactory;
      private readonly ICompoundAlternativeTask _compoundAlternativeTask;

      public AlternativeMapper(ParameterMapper parameterMapper,
         IParameterAlternativeFactory parameterAlternativeFactory,
         ICompoundAlternativeTask compoundAlternativeTask,
         ISpeciesRepository speciesRepository) : base(parameterMapper)
      {
         _speciesRepository = speciesRepository;
         _parameterAlternativeFactory = parameterAlternativeFactory;
         _compoundAlternativeTask = compoundAlternativeTask;
      }

      public override async Task<Alternative> MapToSnapshot(ParameterAlternative parameterAlternative)
      {
         if (parameterAlternative.IsCalculated)
            return null;

         var snapshot =  await SnapshotFrom(parameterAlternative, x =>
         {
            x.IsDefault = SnapshotValueFor(parameterAlternative.IsDefault, true);
            x.Species = (parameterAlternative as ParameterAlternativeWithSpecies)?.Species.Name;
         });

         return snapshot;
      }

      public override async Task<ParameterAlternative> MapToModel(Alternative snapshot, ParameterAlternativeGroup parameterAlternativeGroup)
      {
         var alternative = _parameterAlternativeFactory.CreateAlternativeFor(parameterAlternativeGroup);
         alternative.IsDefault = ModelValueFor(snapshot.IsDefault, true);
         MapSnapshotPropertiesToModel(snapshot, alternative);

         await UpdateParametersFromSnapshot(snapshot, alternative, parameterAlternativeGroup.Name);

         if(parameterAlternativeGroup.IsNamed(CoreConstants.Groups.COMPOUND_SOLUBILITY))
            updateSolubilityAlternative(alternative);

         var alternativeWithSpecies = alternative as ParameterAlternativeWithSpecies;
         if (alternativeWithSpecies == null)
            return alternative;

         alternativeWithSpecies.Species = _speciesRepository.FindByName(snapshot.Species);
         if (alternativeWithSpecies.Species == null)
            throw new SnapshotOutdatedException(PKSimConstants.Error.CouldNotFindSpecies(snapshot.Species, _speciesRepository.AllNames()));

         return alternativeWithSpecies;
      }

      private void updateSolubilityAlternative(ParameterAlternative solubilityAlternative)
      {
         var solubilityTable = solubilityAlternative.Parameter(CoreConstants.Parameters.SOLUBILITY_TABLE);
         //default structure, nothing to change
         if (!solubilityTable.Formula.IsTable())
            return;

         _compoundAlternativeTask.PrepareSolubilityAlternativeForTableSolubility(solubilityAlternative);
      }

   }
}