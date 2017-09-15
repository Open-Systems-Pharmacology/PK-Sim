using System;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Repositories;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using SnapshotSimulation = PKSim.Core.Snapshots.Simulation;
using ModelSimulation = PKSim.Core.Model.Simulation;

namespace PKSim.Core.Snapshots.Mappers
{
   public class SimulationMapper : ObjectBaseSnapshotMapperBase<ModelSimulation, SnapshotSimulation, PKSimProject>
   {
      private readonly SimulationPropertiesMapper _simulationPropertiesMapper;
      private readonly SolverSettingsMapper _solverSettingsMapper;
      private readonly OutputSchemaMapper _outputSchemaMapper;
      private readonly OutputSelectionsMapper _outputSelectionsMapper;
      private readonly IObservedDataRepository _observedDataRepository;
      private readonly CompoundPropertiesMapper _compoundPropertiesMapper;
      private readonly ParameterMapper _parameterMapper;
      private readonly IBuildingBlockRepository _buildingBlockRepository;
      private readonly AdvancedParameterMapper _advancedParameterMapper;

      public SimulationMapper(
         SimulationPropertiesMapper simulationPropertiesMapper,
         SolverSettingsMapper solverSettingsMapper,
         OutputSchemaMapper outputSchemaMapper,
         OutputSelectionsMapper outputSelectionsMapper,
         IObservedDataRepository observedDataRepository,
         CompoundPropertiesMapper compoundPropertiesMapper,
         ParameterMapper parameterMapper,
         IBuildingBlockRepository buildingBlockRepository,
         AdvancedParameterMapper advancedParameterMapper)
      {
         _simulationPropertiesMapper = simulationPropertiesMapper;
         _solverSettingsMapper = solverSettingsMapper;
         _outputSchemaMapper = outputSchemaMapper;
         _outputSelectionsMapper = outputSelectionsMapper;
         _observedDataRepository = observedDataRepository;
         _compoundPropertiesMapper = compoundPropertiesMapper;
         _parameterMapper = parameterMapper;
         _buildingBlockRepository = buildingBlockRepository;
         _advancedParameterMapper = advancedParameterMapper;
      }

      public override async Task<SnapshotSimulation> MapToSnapshot(ModelSimulation simulation)
      {
         var snapshot = await SnapshotFrom(simulation);
         snapshot.Configuration = await _simulationPropertiesMapper.MapToSnapshot(simulation.Properties);
         snapshot.Parameters = await allParametersChangedByUserFrom(simulation);
         snapshot.Solver = await _solverSettingsMapper.MapToSnapshot(simulation.Solver);
         snapshot.OutputSchema = await _outputSchemaMapper.MapToSnapshot(simulation.OutputSchema);
         snapshot.OutputSelections = await _outputSelectionsMapper.MapToSnapshot(simulation.OutputSelections);
         snapshot.Individual = usedSimulationSubject<Model.Individual>(simulation);
         snapshot.Population = usedSimulationSubject<Model.Population>(simulation);
         snapshot.Compounds = await usedCompoundsFrom(simulation);
         snapshot.Events = await usedEventsFrom(simulation);
         snapshot.ObservedData = usedObervedDataFrom(simulation);
         snapshot.AdvancedParameters = await advancedParametersFrom(simulation);
         return snapshot;
      }

      private async Task<AdvancedParameter[]> advancedParametersFrom(ModelSimulation simulation)
      {
         var populationSimulation = simulation as PopulationSimulation;
         if (populationSimulation == null)
            return null;

         return await _advancedParameterMapper.MapToSnapshot(populationSimulation.AdvancedParameters);
      }

      private async Task<EventSelection[]> usedEventsFrom(ModelSimulation simulation)
      {
         var eventMappings = simulation.EventProperties.EventMappings;
         if (!eventMappings.Any())
            return null;

         var tasks = simulation.EventProperties.EventMappings.Select(eventSelectionFrom);
         return await Task.WhenAll(tasks);
      }

      private async Task<EventSelection> eventSelectionFrom(EventMapping eventMapping)
      {
         var eventBuildingBlock = _buildingBlockRepository.ById(eventMapping.TemplateEventId);
         return new EventSelection
         {
            Name = eventBuildingBlock.Name,
            StartTime = await _parameterMapper.MapToSnapshot(eventMapping.StartTime)
         };
      }

      private Task<CompoundProperties[]> usedCompoundsFrom(ModelSimulation simulation)
      {
         var tasks = simulation.CompoundPropertiesList.Select(_compoundPropertiesMapper.MapToSnapshot);
         return Task.WhenAll(tasks);
      }

      private Task<LocalizedParameter[]> allParametersChangedByUserFrom(ModelSimulation simulation)
      {
         var changedParameters = simulation.Model.Root.GetAllChildren<IParameter>(x => x.ParameterHasChanged());
         return _parameterMapper.LocalizedParametersFrom(changedParameters);
      }

      private string[] usedObervedDataFrom(ModelSimulation simulation)
      {
         return simulation.UsedObservedData
            .Select(_observedDataRepository.FindFor)
            .Select(x => x.Name).ToArray();
      }

      private string usedSimulationSubject<T>(ModelSimulation simulation) where T : class, ISimulationSubject
      {
         var name = simulation.BuildingBlock<T>()?.Name;
         if (typeof(T).IsAnImplementationOf<Model.Individual>() && simulation.IsAnImplementationOf<IndividualSimulation>())
            return name;

         if (typeof(T).IsAnImplementationOf<Model.Population>() && simulation.IsAnImplementationOf<PopulationSimulation>())
            return name;

         return null;
      }

      public override Task<ModelSimulation> MapToModel(SnapshotSimulation snapshot, PKSimProject project)
      {
         throw new NotImplementedException();
      }
   }
}