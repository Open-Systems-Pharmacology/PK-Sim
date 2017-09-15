using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using SnapshotPopulation = PKSim.Core.Snapshots.Population;
using ModelPopulation = PKSim.Core.Model.Population;

namespace PKSim.Core.Snapshots.Mappers
{
   public class PopulationMapper : ObjectBaseSnapshotMapperBase<ModelPopulation, SnapshotPopulation>
   {
      private readonly AdvancedParameterMapper _advancedParameterMapper;
      private readonly RandomPopulationSettingsMapper _randomPopulationSettingsMapper;
      private readonly IRandomPopulationFactory _randomPopulationFactory;
      private readonly IParameterTask _parameterTask;

      public PopulationMapper(AdvancedParameterMapper advancedParameterMapper,
         RandomPopulationSettingsMapper randomPopulationSettingsMapper,
         IRandomPopulationFactory randomPopulationFactory,
         IParameterTask parameterTask)
      {
         _advancedParameterMapper = advancedParameterMapper;
         _randomPopulationSettingsMapper = randomPopulationSettingsMapper;
         _randomPopulationFactory = randomPopulationFactory;
         _parameterTask = parameterTask;
      }

      public override async Task<SnapshotPopulation> MapToSnapshot(ModelPopulation population)
      {
         var snapshot = await SnapshotFrom(population, x => { x.Seed = population.Seed; });
         await mapPopulationProperties(snapshot, population);
         return snapshot;
      }

      private Task mapPopulationProperties(SnapshotPopulation snapshot, ModelPopulation population)
      {
         switch (population)
         {
            case RandomPopulation randomPopulation:
               return mapRandomPopulationProperties(snapshot, randomPopulation);
            default:
               return FromException(new OSPSuiteException(PKSimConstants.Error.PopulationSnapshotOnlySupportedForRandomPopulation));
         }
      }

      private async Task mapRandomPopulationProperties(SnapshotPopulation snapshot, RandomPopulation randomPopulation)
      {
         snapshot.Settings = await _randomPopulationSettingsMapper.MapToSnapshot(randomPopulation.Settings);
         snapshot.AdvancedParameters = await snapshotAdvancedParametersFrom(randomPopulation);
      }

      private Task<AdvancedParameter[]> snapshotAdvancedParametersFrom(ModelPopulation population)
      {
         var tasks = population.AdvancedParameters.Select(_advancedParameterMapper.MapToSnapshot);
         return Task.WhenAll(tasks);
      }

      public override async Task<ModelPopulation> MapToModel(SnapshotPopulation snapshot)
      {
         var randomPopulationSettings = await _randomPopulationSettingsMapper.MapToModel(snapshot.Settings);

         var population = await _randomPopulationFactory.CreateFor(randomPopulationSettings, CancellationToken.None, snapshot.Seed);
         MapSnapshotPropertiesToModel(snapshot, population);
         await updateAdvancedParameters(population, snapshot.AdvancedParameters);
         return population;
      }

      private async Task updateAdvancedParameters(RandomPopulation population, AdvancedParameter[] snapshotAdvancedParameters)
      {
         if (snapshotAdvancedParameters == null)
            return;

         population.RemoveAllAdvancedParameters();
         var parameterCache = _parameterTask.PathCacheFor(population.AllIndividualParameters());
         var tasks = snapshotAdvancedParameters.Select(x => _advancedParameterMapper.MapToModel(x, parameterCache));
         var advancedParameters = await Task.WhenAll(tasks);

         advancedParameters.Each(x => population.AddAdvancedParameter(x, generateRandomValues: true));
      }
   }
}