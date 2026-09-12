using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Services;
using OSPSuite.Core.Snapshots.Mappers;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using Compound = PKSim.Core.Model.Compound;
using DataRepository = OSPSuite.Core.Domain.Data.DataRepository;
using Individual = PKSim.Core.Model.Individual;
using Protocol = PKSim.Core.Model.Protocol;
using SnapshotIndividual = PKSim.Core.Snapshots.Individual;
using SnapshotProject = PKSim.Core.Snapshots.Project;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_SnapshotUpdater : ContextForIntegration<ISnapshotUpdater>
   {
      protected IJsonSerializer _jsonSerializer;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _jsonSerializer = IoC.Resolve<IJsonSerializer>();
      }
   }

   public class When_adding_a_snapshot_to_an_individual_building_block : concern_for_SnapshotUpdater
   {
      private Individual _individual;
      private IndividualBuildingBlock _individualBuildingBlock;
      private SnapshotIndividual _snapshot;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         DomainFactoryForSpecs.CreateExpressionProfileAndAddToIndividual<IndividualEnzyme>(_individual);
         _individualBuildingBlock = IoC.Resolve<IIndividualToIndividualBuildingBlockMapper>().MapFrom(_individual);
      }

      protected override void Because()
      {
         sut.AddSnapshotTo(_individualBuildingBlock, _individual);
         _snapshot = _jsonSerializer.DeserializeFromBase64String<SnapshotIndividual>(_individualBuildingBlock.Snapshot).Result;
      }

      [Observation]
      public void the_snapshot_should_describe_the_individual()
      {
         _snapshot.Name.ShouldBeEqualTo(_individual.Name);
         _snapshot.OriginData.ShouldNotBeNull();
      }

      [Observation]
      public void the_snapshot_should_not_reference_the_expression_profiles_of_the_individual()
      {
         _snapshot.ExpressionProfiles.ShouldBeNull();
      }

      [Observation]
      public void the_snapshot_should_recreate_the_individual_building_block()
      {
         var recreated = IoC.Resolve<IIndividualSnapshotToIndividualBuildingBlockMapper>().MapFrom(_individualBuildingBlock.Snapshot);
         recreated.Name.ShouldBeEqualTo(_individualBuildingBlock.Name);
         recreated.Count().ShouldBeEqualTo(_individualBuildingBlock.Count());
      }
   }

   public class When_adding_a_snapshot_to_an_expression_profile_building_block : concern_for_SnapshotUpdater
   {
      private ExpressionProfile _expressionProfile;
      private ExpressionProfileBuildingBlock _expressionProfileBuildingBlock;
      private ExpressionProfileBuildingBlock _recreated;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _expressionProfile = DomainFactoryForSpecs.CreateExpressionProfile<IndividualEnzyme>();
         _expressionProfileBuildingBlock = IoC.Resolve<IExpressionProfileToExpressionProfileBuildingBlockMapper>().MapFrom(_expressionProfile);
      }

      protected override void Because()
      {
         sut.AddSnapshotTo(_expressionProfileBuildingBlock, _expressionProfile);
         _recreated = IoC.Resolve<IExpressionProfileSnapshotToExpressionProfileBuildingBlockMapper>().MapFrom(_expressionProfileBuildingBlock.Snapshot);
      }

      [Observation]
      public void the_snapshot_should_recreate_the_expression_profile_building_block()
      {
         _recreated.Name.ShouldBeEqualTo(_expressionProfileBuildingBlock.Name);
         _recreated.MoleculeName.ShouldBeEqualTo(_expressionProfileBuildingBlock.MoleculeName);
         _recreated.Species.ShouldBeEqualTo(_expressionProfileBuildingBlock.Species);
         _recreated.Type.ShouldBeEqualTo(_expressionProfileBuildingBlock.Type);
      }
   }

   public class When_updating_the_snapshot_of_an_expression_profile_building_block_from_a_database_query : concern_for_SnapshotUpdater
   {
      private ExpressionProfileBuildingBlock _expressionProfileBuildingBlock;
      private ExpressionProfileBuildingBlock _recreated;
      private ExpressionProfile _expressionProfile;

      protected override void Context()
      {
         base.Context();
         _expressionProfile = DomainFactoryForSpecs.CreateExpressionProfile<IndividualEnzyme>();
         _expressionProfileBuildingBlock = IoC.Resolve<IExpressionProfileToExpressionProfileBuildingBlockMapper>().MapFrom(_expressionProfile);
         sut.AddSnapshotTo(_expressionProfileBuildingBlock, _expressionProfile);
      }

      protected override void Because()
      {
         var queryResults = new QueryExpressionResults(new[]
         {
            new ExpressionResult {ContainerName = CoreConstants.Compartment.PERIPORTAL, RelativeExpression = 1},
            new ExpressionResult {ContainerName = CoreConstants.Organ.KIDNEY, RelativeExpression = 0.5},
         });

         sut.UpdateSnapshotFromQuery(_expressionProfileBuildingBlock, queryResults);
         _recreated = IoC.Resolve<IExpressionProfileSnapshotToExpressionProfileBuildingBlockMapper>().MapFrom(_expressionProfileBuildingBlock.Snapshot);
      }

      [Observation]
      public void the_snapshot_should_contain_the_queried_relative_expressions()
      {
         relativeExpressionFor(CoreConstants.Compartment.PERIPORTAL).ShouldBeEqualTo(1);
         relativeExpressionFor(CoreConstants.Organ.KIDNEY).ShouldBeEqualTo(0.5);
      }

      private double relativeExpressionFor(string containerName) =>
         _recreated.ExpressionParameters.Single(x => x.HasExpressionName() && x.ContainerNameForRelativeExpressionParameter() == containerName).Value.Value;
   }

   public class When_updating_the_snapshot_of_an_expression_profile_building_block_without_snapshot : concern_for_SnapshotUpdater
   {
      private ExpressionProfileBuildingBlock _expressionProfileBuildingBlock;

      protected override void Context()
      {
         base.Context();
         _expressionProfileBuildingBlock = new ExpressionProfileBuildingBlock();
      }

      protected override void Because()
      {
         sut.UpdateSnapshotFromQuery(_expressionProfileBuildingBlock, new QueryExpressionResults(new ExpressionResult[] { }));
      }

      [Observation]
      public void the_building_block_should_still_have_no_snapshot()
      {
         _expressionProfileBuildingBlock.HasSnapshot.ShouldBeFalse();
      }
   }

   public abstract class concern_for_SnapshotUpdater_with_a_simulation : ContextForSimulationIntegration<ISnapshotUpdater>
   {
      protected Compound _compound;
      protected Individual _individual;
      protected Protocol _protocol;
      protected PKSimProject _project;
      protected ISnapshotMapper _snapshotMapper;
      protected IJsonSerializer _jsonSerializer;
      protected DataRepository _observedData;
      protected Simulation _subjectSimulation;
      protected Population _population;
      protected IModelCoreSimulation _moBiSimulation;
      protected PKSimProject _deserializedProject;
      private ExpressionProfile _expressionProfile;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         _population = DomainFactoryForSpecs.CreateDefaultPopulation(_individual);
         _expressionProfile = DomainFactoryForSpecs.CreateExpressionProfileAndAddToIndividual<IndividualEnzyme>(_individual);
         _protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
         _snapshotMapper = IoC.Resolve<ISnapshotMapper>();
         _jsonSerializer = IoC.Resolve<IJsonSerializer>();
         _project = new PKSimProject();
         IoC.Resolve<ICoreWorkspace>().Project = _project;

         _observedData = DomainHelperForSpecs.IndividualSimulationDataRepositoryFor("S").WithName("obs data");

         _project.AddBuildingBlock(_compound);
         _project.AddBuildingBlock(_individual);
         _project.AddBuildingBlock(_population);
         _project.AddBuildingBlock(_protocol);
         _project.AddBuildingBlock(_expressionProfile);
         _project.AddObservedData(_observedData);

         _subjectSimulation = CreateSimulation();
         _subjectSimulation.AddUsedObservedData(UsedObservedData.From(_observedData));
         _project.AddBuildingBlock(_subjectSimulation);
      }

      protected override void Context()
      {
         base.Context();
         var configuration = IoC.Resolve<ISimulationConfigurationTask>().CreateFor(_subjectSimulation, shouldValidate: true, createAgingDataInSimulation: false);
         _moBiSimulation = IoC.Resolve<ISimulationToModelCoreSimulationMapper>().MapFrom(_subjectSimulation, configuration, shouldCloneModel: true);
      }

      protected override void Because()
      {
         sut.AddSnapshotsToModelCoreSimulation(_subjectSimulation, _moBiSimulation, _project);
         var projectSnapshot = _jsonSerializer.DeserializeFromBase64String<SnapshotProject>(_moBiSimulation.Configuration.ModuleConfigurations.Single().Module.Snapshot).Result;
         _deserializedProject = _snapshotMapper.MapToModel(projectSnapshot, new ProjectContext(new PKSimProject(), runSimulations: false)).Result as PKSimProject;
      }

      [Observation]
      public void the_snapshots_should_be_set()
      {
         _moBiSimulation.Configuration.Individual.Snapshot.ShouldNotBeEmpty();
         _moBiSimulation.Configuration.ModuleConfigurations.Single().Module.Snapshot.ShouldNotBeEmpty();
         _moBiSimulation.Configuration.ExpressionProfiles.Count.ShouldBeGreaterThan(0);
         _moBiSimulation.Configuration.ExpressionProfiles.Each(x => x.Snapshot.ShouldNotBeEmpty());
      }

      [Observation]
      public void the_individual_snapshot_should_not_reference_the_expression_profiles()
      {
         var individualSnapshot = _jsonSerializer.DeserializeFromBase64String<SnapshotIndividual>(_moBiSimulation.Configuration.Individual.Snapshot).Result;
         individualSnapshot.ExpressionProfiles.ShouldBeNull();
      }

      protected abstract Simulation CreateSimulation();
   }

   public class When_adding_the_snapshots_of_an_individual_simulation_to_a_model_core_simulation : concern_for_SnapshotUpdater_with_a_simulation
   {
      [Observation]
      public void the_module_snapshot_should_contain_the_building_blocks_observed_data_and_simulation()
      {
         _deserializedProject.All<Compound>().Count.ShouldBeEqualTo(1);
         _deserializedProject.All<Protocol>().Count.ShouldBeEqualTo(1);
         _deserializedProject.All<Individual>().Count.ShouldBeEqualTo(1);
         _deserializedProject.All<IndividualSimulation>().Count.ShouldBeEqualTo(1);
         _deserializedProject.AllObservedData.Count.ShouldBeEqualTo(1);
      }

      protected override Simulation CreateSimulation() => DomainFactoryForSpecs.CreateSimulationWith(_individual, _compound, _protocol);
   }

   public class When_adding_the_snapshots_of_a_population_simulation_to_a_model_core_simulation : concern_for_SnapshotUpdater_with_a_simulation
   {
      [Observation]
      public void the_module_snapshot_should_contain_the_building_blocks_observed_data_and_simulation()
      {
         _deserializedProject.All<Compound>().Count.ShouldBeEqualTo(1);
         _deserializedProject.All<Protocol>().Count.ShouldBeEqualTo(1);
         _deserializedProject.All<Individual>().Count.ShouldBeEqualTo(1);
         _deserializedProject.All<PopulationSimulation>().Count.ShouldBeEqualTo(1);
         _deserializedProject.All<Population>().Count.ShouldBeEqualTo(1);
         _deserializedProject.AllObservedData.Count.ShouldBeEqualTo(1);
      }

      protected override Simulation CreateSimulation() => DomainFactoryForSpecs.CreateSimulationWith(_population, _compound, _protocol);
   }
}
