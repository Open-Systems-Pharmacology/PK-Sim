﻿using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Extensions;
using Event = PKSim.Core.Snapshots.Event;
using Project = PKSim.Core.Snapshots.Project;
using Simulation = PKSim.Core.Snapshots.Simulation;

namespace PKSim.Core
{
   public abstract class concern_for_ProjectMapper : ContextSpecificationAsync<ProjectMapper>
   {
      protected PKSimProject _project;
      protected Individual _individual;
      protected Compound _compound;
      protected PKSimEvent _event;
      protected Formulation _formulation;
      protected Protocol _protocol;
      protected IndividualSimulation _simulation;
      protected Project _snapshot;
      protected ISnapshotMapper _snapshotMapper;
      protected Snapshots.Compound _compoundSnapshot;
      protected Snapshots.Individual _individualSnapshot;
      protected IExecutionContext _executionContext;
      protected Event _eventSnapshot;
      protected Snapshots.Formulation _formulationSnapshot;
      protected Snapshots.Protocol _protocolSnapshot;
      protected Population _population;
      protected Snapshots.Population _populationSnapshot;
      protected DataRepository _observedData;
      protected Snapshots.DataRepository _observedDataSnapshot;
      protected SimulationMapper _simulationMapper;
      protected Simulation _simulationSnapshot;

      protected override Task Context()
      {
         _snapshotMapper = A.Fake<ISnapshotMapper>();
         _executionContext = A.Fake<IExecutionContext>();
         _simulationMapper= A.Fake<SimulationMapper>();
         sut = new ProjectMapper(_executionContext, _simulationMapper);
         A.CallTo(() => _executionContext.Resolve<ISnapshotMapper>()).Returns(_snapshotMapper);
         _individual = new Individual().WithName("IND");
         _compound = new Compound().WithName("COMP");
         _event = new PKSimEvent().WithName("EVENT");
         _formulation = new Formulation().WithName("FORM");
         _protocol = new SimpleProtocol().WithName("PROTO");
         _population = new RandomPopulation().WithName("POP");
         _observedData = new DataRepository().WithName("OD");

         _simulation = new IndividualSimulation().WithName("IND_SIM");

         _project = new PKSimProject();
         _project.AddBuildingBlock(_individual);
         _project.AddBuildingBlock(_compound);
         _project.AddBuildingBlock(_event);
         _project.AddBuildingBlock(_formulation);
         _project.AddBuildingBlock(_protocol);
         _project.AddBuildingBlock(_population);
         _project.AddObservedData(_observedData);
         _project.AddBuildingBlock(_simulation);

         _compoundSnapshot = new Snapshots.Compound();
         _individualSnapshot = new Snapshots.Individual();
         _eventSnapshot = new Event();
         _formulationSnapshot = new Snapshots.Formulation();
         _protocolSnapshot = new Snapshots.Protocol();
         _populationSnapshot = new Snapshots.Population();
         _observedDataSnapshot = new Snapshots.DataRepository();

         A.CallTo(() => _snapshotMapper.MapToSnapshot(_compound)).ReturnsAsync(_compoundSnapshot);
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_individual)).ReturnsAsync(_individualSnapshot);
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_event)).ReturnsAsync(_eventSnapshot);
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_formulation)).ReturnsAsync(_formulationSnapshot);
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_protocol)).ReturnsAsync(_protocolSnapshot);
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_population)).ReturnsAsync(_populationSnapshot);
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_observedData)).ReturnsAsync(_observedDataSnapshot);

         A.CallTo(() => _simulationMapper.MapToSnapshot(_simulation,_project)).ReturnsAsync(_simulationSnapshot);

         return Task.FromResult(true);
      }
   }

   public class When_exporting_a_project_to_snapshot : concern_for_ProjectMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_project);
      }

      [Observation]
      public void should_retrieve_the_snapshot_for_all_underlying_building_blocks()
      {
         _snapshot.Compounds.ShouldContain(_compoundSnapshot);
         _snapshot.Individuals.ShouldContain(_individualSnapshot);
         _snapshot.Protocols.ShouldContain(_protocolSnapshot);
         _snapshot.Formulations.ShouldContain(_formulationSnapshot);
         _snapshot.Events.ShouldContain(_eventSnapshot);
         _snapshot.Populations.ShouldContain(_populationSnapshot);
      }

      [Observation]
      public void should_retrieve_the_snapshot_for_all_used_observed_data()
      {
         _snapshot.ObservedData.ShouldContain(_observedDataSnapshot);
      }

      [Observation]
      public void should_retrieve_the_snapshot_for_all_simulations_used_in_the_project()
      {
         _snapshot.Simulations.ShouldContain(_simulationSnapshot);
      }
   }

   public class When_converting_a_project_snapshot_to_project : concern_for_ProjectMapper
   {
      private PKSimProject _newProject;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_project);
         A.CallTo(() => _snapshotMapper.MapToModel(_compoundSnapshot)).ReturnsAsync(_compound);
         A.CallTo(() => _snapshotMapper.MapToModel(_individualSnapshot)).ReturnsAsync(_individual);
         A.CallTo(() => _snapshotMapper.MapToModel(_protocolSnapshot)).ReturnsAsync(_protocol);
         A.CallTo(() => _snapshotMapper.MapToModel(_formulationSnapshot)).ReturnsAsync(_formulation);
         A.CallTo(() => _snapshotMapper.MapToModel(_eventSnapshot)).ReturnsAsync(_event);
         A.CallTo(() => _snapshotMapper.MapToModel(_populationSnapshot)).ReturnsAsync(_population);
         A.CallTo(() => _snapshotMapper.MapToModel(_observedDataSnapshot)).ReturnsAsync(_observedData);
         A.CallTo(() => _simulationMapper.MapToModel(_simulationSnapshot, A<PKSimProject>._)).ReturnsAsync(_simulation);
      }

      protected override async Task Because()
      {
         _newProject = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_return_a_project_with_the_expected_building_blocks()
      {
         _newProject.All<Compound>().ShouldContain(_compound);
         _newProject.All<Individual>().ShouldContain(_individual);
         _newProject.All<PKSimEvent>().ShouldContain(_event);
         _newProject.All<Formulation>().ShouldContain(_formulation);
         _newProject.All<Protocol>().ShouldContain(_protocol);
         _newProject.All<Population>().ShouldContain(_population);
      }

      [Observation]
      public void should_have_mapped_the_observed_data()
      {
         _newProject.AllObservedData.ShouldContain(_observedData);
      }


      [Observation]
      public void should_have_mapped_the_simulations()
      {
         _newProject.All<Model.Simulation>().ShouldContain(_simulation);
      }
   }
}