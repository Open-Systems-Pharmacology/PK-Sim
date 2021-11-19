using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterIdUpdater : ContextSpecification<IParameterIdUpdater>
   {
      protected IParameter _para3;
      protected IReadOnlyList<IParameter> _allParameters;
      protected IParameter _para1;
      protected IParameter _para2;
      protected Individual _individual;
      protected ISimulationParameterOriginIdUpdater _simulationParameterOriginIdUpdater;

      protected override void Context()
      {
         _simulationParameterOriginIdUpdater = A.Fake<ISimulationParameterOriginIdUpdater>();
         sut = new ParameterIdUpdater(_simulationParameterOriginIdUpdater);

         _individual = A.Fake<Individual>();
         A.CallTo(() => _individual.BuildingBlockType).Returns(PKSimBuildingBlockType.Individual);
         _individual.Id = "toto";

         _para1 = new PKSimParameter {BuildingBlockType = PKSimBuildingBlockType.Individual};
         _para1.Origin.BuilingBlockId = "tata";

         _para2 = new PKSimParameter {BuildingBlockType = PKSimBuildingBlockType.Compound};
         _para2.Origin.BuilingBlockId = "tata";

         _para3 = new PKSimParameter {BuildingBlockType = PKSimBuildingBlockType.Individual};
         _para2.Origin.BuilingBlockId = "tata";

         _allParameters = new List<IParameter> {_para1, _para2, _para3};
      }
   }

   public class When_updating_the_building_block_id_of_a_set_of_parameters_for_a_given_building_block : concern_for_ParameterIdUpdater
   {
      protected override void Because()
      {
         sut.UpdateBuildingBlockId(_allParameters, _individual);
      }

      [Observation]
      public void should_resolve_all_parameter_with_the_accurate_building_block_type_and_set_their_building_block_id()
      {
         _para1.Origin.BuilingBlockId.ShouldBeEqualTo(_individual.Id);
         _para3.Origin.BuilingBlockId.ShouldBeEqualTo(_individual.Id);
      }

      [Observation]
      public void should_have_let_the_building_block_id_of_any_parameter_with_a_non_matching_building_block_tyoe_unchanged()
      {
         _para2.Origin.BuilingBlockId.ShouldBeEqualTo("tata");
      }
   }

   public class When_updating_the_building_block_id_of_a_container_for_a_given_building_block : concern_for_ParameterIdUpdater
   {
      private IContainer _container;

      protected override void Context()
      {
         base.Context();
         _container = A.Fake<IContainer>();
         A.CallTo(() => _container.GetAllChildren<IParameter>()).Returns(_allParameters);
      }

      protected override void Because()
      {
         sut.UpdateBuildingBlockId(_container, _individual);
      }

      [Observation]
      public void should_update_the_building_block_id_of_all_children_parameters_from_the_container_with_the_the_accurate_type()
      {
         _para1.Origin.BuilingBlockId.ShouldBeEqualTo(_individual.Id);
         _para3.Origin.BuilingBlockId.ShouldBeEqualTo(_individual.Id);
      }

      [Observation]
      public void should_have_let_the_building_block_id_of_any_parameter_with_a_non_matching_building_block_tyoe_unchanged()
      {
         _para2.Origin.BuilingBlockId.ShouldBeEqualTo("tata");
      }
   }

   public class When_updating_the_parameter_id_of_a_parameter_defined_in_a_simulation_from_another_parameter : concern_for_ParameterIdUpdater
   {
      private IParameter _sourceParameter;
      private IParameter _targetParameter;

      protected override void Context()
      {
         base.Context();
         _sourceParameter = new PKSimParameter().WithId("toto");
         _targetParameter = new PKSimParameter().WithId("tata");
         _targetParameter.Origin.SimulationId = "SimID";
      }

      protected override void Because()
      {
         sut.UpdateParameterId(_sourceParameter, _targetParameter);
      }

      [Observation]
      public void should_set_the_parameter_id_of_the_target_parameter_in_the_source_parameter()
      {
         _targetParameter.Origin.ParameterId.ShouldBeEqualTo(_sourceParameter.Id);
      }
   }

   public class When_updating_the_parameter_ids_of_parameter_defined_in_containers_that_belongs_in_a_simulation : concern_for_ParameterIdUpdater
   {
      private IContainer _sourceContainer;
      private IContainer _targetContainer;
      private IParameter _sourceParameter;
      private IParameter _targetParameter;
      private IParameter _targetParameter2;

      protected override void Context()
      {
         base.Context();
         _sourceContainer = new Container();
         _targetContainer = new Container();
         _sourceParameter = new PKSimParameter().WithName("toto").WithId("1");
         _targetParameter = new PKSimParameter().WithName("toto").WithId("10");
         _targetParameter.Origin.SimulationId = "SimId";
         _targetParameter2 = new PKSimParameter().WithName("titi").WithId("20");
         _sourceContainer.Add(_sourceParameter);
         _targetContainer.Add(_targetParameter);
         _targetContainer.Add(_targetParameter2);
      }

      protected override void Because()
      {
         sut.UpdateParameterIds(_sourceContainer, _targetContainer);
      }

      [Observation]
      public void should_update_the_parameter_id_of_parameter_having_the_same_name()
      {
         _targetParameter.Origin.ParameterId.ShouldBeEqualTo(_sourceParameter.Id);
      }
   }

   public class When_updating_the_parameter_ids_of_parameter_defined_in_parameter_cache : concern_for_ParameterIdUpdater
   {
      private PathCache<IParameter> _sourceParameters;
      private PathCache<IParameter> _targetParameters;
      private IParameter _sourceParameter;
      private IParameter _targetParameter;
      private IParameter _targetParameter2;

      protected override void Context()
      {
         base.Context();
         _sourceParameters = new PathCacheForSpecs<IParameter>();
         _targetParameters = new PathCacheForSpecs<IParameter>();
         _sourceParameter = new PKSimParameter().WithName("toto").WithId("1").WithParentContainer(new Container().WithName("Cont"));
         _targetParameter = new PKSimParameter().WithName("toto").WithId("10").WithParentContainer(new Container().WithName("Cont"));
         _targetParameter2 = new PKSimParameter().WithName("titi").WithId("20").WithParentContainer(new Container().WithName("Cont"));
         _sourceParameters.Add(_sourceParameter);
         _targetParameters.Add(_targetParameter);
         _targetParameters.Add(_targetParameter2);
      }

      protected override void Because()
      {
         sut.UpdateParameterIds(_sourceParameters, _targetParameters);
      }

      [Observation]
      public void should_update_the_parameter_id_of_parameter_having_the_same_path()
      {
         _targetParameter.Origin.ParameterId.ShouldBeEqualTo(_sourceParameter.Id);
      }
   }

   public class When_updating_the_simulation_id_in_a_simulation : concern_for_ParameterIdUpdater
   {
      private Simulation _simulation;
      private readonly string _simulationId = "SimId";
      private PKSimEvent _eventBuildingBlockInSimulation;
      private IParameter _eventParameter;

      protected override void Context()
      {
         base.Context();
         _simulation = new IndividualSimulation().WithId(_simulationId);
         _eventParameter = DomainHelperForSpecs.ConstantParameterWithValue(10);
         _eventBuildingBlockInSimulation = new PKSimEvent {_eventParameter};
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("TemplateId", PKSimBuildingBlockType.Event) {BuildingBlock = _eventBuildingBlockInSimulation});
      }

      protected override void Because()
      {
         sut.UpdateSimulationId(_simulation);
      }

      [Observation]
      public void should_delegate_to_the_core_simulation_id_updater_to_update_the_simulation()
      {
         A.CallTo(() => _simulationParameterOriginIdUpdater.UpdateSimulationId(_simulation)).MustHaveHappened();
      }

      [Observation]
      public void should_also_set_the_simulation_id_in_all_building_block_parameters()
      {
         _eventParameter.Origin.SimulationId.ShouldBeEqualTo(_simulationId);
      }
   }
}