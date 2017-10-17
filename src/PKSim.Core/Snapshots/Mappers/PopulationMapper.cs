using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OSPSuite.Utility.Exceptions;
using PKSim.Assets;
using PKSim.Core.Model;
using SnapshotPopulation = PKSim.Core.Snapshots.Population;
using ModelPopulation = PKSim.Core.Model.Population;

namespace PKSim.Core.Snapshots.Mappers
{
   public class PopulationMapper : ObjectBaseSnapshotMapperBase<ModelPopulation, SnapshotPopulation>
   {
      private readonly AdvancedParameterMapper _advancedParameterMapper;
      private readonly RandomPopulationSettingsMapper _randomPopulationSettingsMapper;
      private readonly IRandomPopulationFactory _randomPopulationFactory;

      public PopulationMapper(AdvancedParameterMapper advancedParameterMapper,
         RandomPopulationSettingsMapper randomPopulationSettingsMapper,
         IRandomPopulationFactory randomPopulationFactory)
      {
         _advancedParameterMapper = advancedParameterMapper;
         _randomPopulationSettingsMapper = randomPopulationSettingsMapper;
         _randomPopulationFactory = randomPopulationFactory;
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
               return Task.FromException(new OSPSuiteException(PKSimConstants.Error.PopulationSnapshotOnlySupportedForRandomPopulation));
         }
      }

      private async Task mapRandomPopulationProperties(SnapshotPopulation snapshot, RandomPopulation randomPopulation)
      {
         snapshot.Settings = await _randomPopulationSettingsMapper.MapToSnapshot(randomPopulation.Settings);
         snapshot.AdvancedParameters = await _advancedParameterMapper.MapToSnapshots(randomPopulation.AdvancedParameters.ToList());
      }

      public override async Task<ModelPopulation> MapToModel(SnapshotPopulation snapshot)
      {
         var randomPopulationSettings = await _randomPopulationSettingsMapper.MapToModel(snapshot.Settings);

         //Do not add default molecule variability as this will be loaded from snapshot
         var population = await _randomPopulationFactory.CreateFor(randomPopulationSettings, CancellationToken.None, snapshot.Seed, addMoleculeParametersVariability:false);
         MapSnapshotPropertiesToModel(snapshot, population);
         await _advancedParameterMapper.MapToModel(snapshot.AdvancedParameters, population);
         return population;
      }
   }
}