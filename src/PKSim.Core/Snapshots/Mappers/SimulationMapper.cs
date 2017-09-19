﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using SnapshotSimulation = PKSim.Core.Snapshots.Simulation;
using ModelSimulation = PKSim.Core.Model.Simulation;

namespace PKSim.Core.Snapshots.Mappers
{
   public class SimulationMapper : ObjectBaseSnapshotMapperBase<ModelSimulation, SnapshotSimulation, PKSimProject, PKSimProject>
   {
      private readonly SimulationPropertiesMapper _simulationPropertiesMapper;
      private readonly SolverSettingsMapper _solverSettingsMapper;
      private readonly OutputSchemaMapper _outputSchemaMapper;
      private readonly OutputSelectionsMapper _outputSelectionsMapper;
      private readonly CompoundPropertiesMapper _compoundPropertiesMapper;
      private readonly ParameterMapper _parameterMapper;
      private readonly AdvancedParameterMapper _advancedParameterMapper;
      private readonly EventPropertiesMapper _eventPropertiesMapper;
      private readonly ISimulationFactory _simulationFactory;
      private readonly IExecutionContext _executionContext;
      private readonly ISimulationModelCreator _simulationModelCreator;
      private readonly ISimulationBuildingBlockUpdater _simulationBuildingBlockUpdater;

      public SimulationMapper(
         SimulationPropertiesMapper simulationPropertiesMapper,
         SolverSettingsMapper solverSettingsMapper,
         OutputSchemaMapper outputSchemaMapper,
         OutputSelectionsMapper outputSelectionsMapper,
         CompoundPropertiesMapper compoundPropertiesMapper,
         ParameterMapper parameterMapper,
         AdvancedParameterMapper advancedParameterMapper,
         EventPropertiesMapper eventPropertiesMapper,
         ISimulationFactory simulationFactory,
         IExecutionContext executionContext,
         ISimulationModelCreator simulationModelCreator,
         ISimulationBuildingBlockUpdater simulationBuildingBlockUpdater)
      {
         _simulationPropertiesMapper = simulationPropertiesMapper;
         _solverSettingsMapper = solverSettingsMapper;
         _outputSchemaMapper = outputSchemaMapper;
         _outputSelectionsMapper = outputSelectionsMapper;
         _compoundPropertiesMapper = compoundPropertiesMapper;
         _parameterMapper = parameterMapper;
         _advancedParameterMapper = advancedParameterMapper;
         _eventPropertiesMapper = eventPropertiesMapper;
         _simulationFactory = simulationFactory;
         _executionContext = executionContext;
         _simulationModelCreator = simulationModelCreator;
         _simulationBuildingBlockUpdater = simulationBuildingBlockUpdater;
      }

      public override async Task<SnapshotSimulation> MapToSnapshot(ModelSimulation simulation, PKSimProject project)
      {
         var snapshot = await SnapshotFrom(simulation);
         snapshot.Individual = usedSimulationSubject<Model.Individual>(simulation);
         snapshot.Population = usedSimulationSubject<Model.Population>(simulation);
         snapshot.Compounds = await usedCompoundsFrom(simulation, project);
         snapshot.Configuration = await _simulationPropertiesMapper.MapToSnapshot(simulation.Properties);
         snapshot.Solver = await _solverSettingsMapper.MapToSnapshot(simulation.Solver);
         snapshot.OutputSchema = await _outputSchemaMapper.MapToSnapshot(simulation.OutputSchema);
         snapshot.OutputSelections = await _outputSelectionsMapper.MapToSnapshot(simulation.OutputSelections);
         snapshot.Events = await usedEventsFrom(simulation, project);
         snapshot.ObservedData = usedObervedDataFrom(simulation, project);
         snapshot.Parameters = await allParametersChangedByUserFrom(simulation);
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

      private Task<EventSelections> usedEventsFrom(ModelSimulation simulation, PKSimProject project)
      {
         return _eventPropertiesMapper.MapToSnapshot(simulation.EventProperties, project);
      }

      private Task<CompoundProperties[]> usedCompoundsFrom(ModelSimulation simulation, PKSimProject project)
      {
         var tasks = simulation.CompoundPropertiesList.Select(x => _compoundPropertiesMapper.MapToSnapshot(x, project));
         return Task.WhenAll(tasks);
      }

      private Task<LocalizedParameter[]> allParametersChangedByUserFrom(ModelSimulation simulation)
      {
         var changedParameters = simulation.Model.Root.GetAllChildren<IParameter>(x => x.ParameterHasChanged());
         return _parameterMapper.LocalizedParametersFrom(changedParameters);
      }

      private string[] usedObervedDataFrom(ModelSimulation simulation, PKSimProject project)
      {
         return simulation.UsedObservedData
            .Select(project.ObservedDataBy)
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

      public override async Task<ModelSimulation> MapToModel(SnapshotSimulation snapshot, PKSimProject project)
      {
         var simulationSubject = simulationSubjectFrom(snapshot, project);
         var compounds = compoundsFrom(snapshot.Compounds, project);
         var modelProperties = modelPropertiesFrom(snapshot.Configuration, simulationSubject);

         var simulation = _simulationFactory.CreateFrom(simulationSubject, compounds, modelProperties);
         MapSnapshotPropertiesToModel(snapshot, simulation);

         var compoundPropertiesContext = new CompoundPropertiesContext(project, simulation);
         var tasks = snapshot.Compounds.Select(x => _compoundPropertiesMapper.MapToModel(x, compoundPropertiesContext));
         await Task.WhenAll(tasks);

         _simulationBuildingBlockUpdater.UpdateProtocolsInSimulation(simulation);
         _simulationBuildingBlockUpdater.UpdateFormulationsInSimulation(simulation);

         simulation.EventProperties = await eventPropertiesFrom(snapshot.Events, project);
         _simulationModelCreator.CreateModelFor(simulation);

//         simulation.Solver = await solverSettingsFrom(snapshot.Solver);
//         simulation.OutputSchema = await  _outputSchemaMapper.MapToSnapshot(simulation.OutputSchema);
//         simulation.OutputSelections = await _outputSelectionsMapper.MapToSnapshot(simulation.OutputSelections);

//         await updateParameters(simulation, snapshot.Parameters);
//         await updateAdvancedParameters(simulation, snapshot.AdvancedParameters);

         return simulation;
      }

      private Task updateAdvancedParameters(ModelSimulation simulation, AdvancedParameter[] snapshotAdvancedParameters)
      {
         throw new NotImplementedException();
      }

      private Task updateParameters(ModelSimulation simulation, LocalizedParameter[] snapshotParameters)
      {
         throw new NotImplementedException();
      }

      private Task<OSPSuite.Core.Domain.SolverSettings> solverSettingsFrom(SolverSettings snapshotSolver)
      {
         throw new NotImplementedException();
      }

      private Task<EventProperties> eventPropertiesFrom(EventSelections snapshotEvents, PKSimProject project)
      {
         return _eventPropertiesMapper.MapToModel(snapshotEvents, project);
      }

      protected override void MapSnapshotPropertiesToModel(SnapshotSimulation snapshot, ModelSimulation simulation)
      {
         base.MapSnapshotPropertiesToModel(snapshot, simulation);
         simulation.AllowAging = snapshot.Configuration.AllowAging;
      }

      private ModelProperties modelPropertiesFrom(SimulationConfiguration simulationConfiguration, ISimulationSubject simulationSubject)
      {
         return _simulationPropertiesMapper.ModelPropertiesFrom(simulationConfiguration, simulationSubject);
      }

      private IReadOnlyList<Model.Compound> compoundsFrom(CompoundProperties[] compounds, PKSimProject project)
      {
         return compounds.Select(x => findBuildingBlock<Model.Compound>(x.Name, project)).ToList();
      }

      private ISimulationSubject simulationSubjectFrom(SnapshotSimulation snapshot, PKSimProject project)
      {
         if (snapshot.Individual != null)
            return findBuildingBlock<Model.Individual>(snapshot.Individual, project);

         if (snapshot.Population != null)
            return findBuildingBlock<Model.Population>(snapshot.Population, project);

         throw new SnapshotOutdatedException(PKSimConstants.Error.SimulationSubjectUndefinedInSnapshot);
      }

      private T findBuildingBlock<T>(string name, PKSimProject project) where T : class, IPKSimBuildingBlock
      {
         var buildingBlock = project.BuildingBlockByName<T>(name);
         if (buildingBlock == null)
            throw new SnapshotOutdatedException(PKSimConstants.Error.SimulationTemplateBuildingBlocktNotFoundInProject(name, typeof(T).Name));

         _executionContext.Load(buildingBlock);
         return buildingBlock;
      }
   }
}