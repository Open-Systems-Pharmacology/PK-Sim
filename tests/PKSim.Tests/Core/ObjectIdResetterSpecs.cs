using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using ObjectIdResetter = PKSim.Core.Services.ObjectIdResetter;

namespace PKSim.Core
{
   public abstract class concern_for_ObjectIdResetter : ContextSpecification<IObjectIdResetter>
   {
      protected override void Context()
      {
         sut = new ObjectIdResetter(new IdGenerator(), new ParameterIdUpdater(new SimulationParameterOriginIdUpdater()));
      }
   }

   public class When_resetting_the_id_in_a_simulation : concern_for_ObjectIdResetter
   {
      private Simulation _simulation;
      private IModel _model;
      private IContainer _modelRoot;
      private IPKSimBuildingBlock _eventBuidingBlock;
      private IParameter _eventParameter;
      private IParameter _eventParameterInSimulation;
      private readonly string _eventId = "EVENT_ID";
      private readonly string _eventParameterId = "EVENT_PARAMETER";
      private readonly string _eventParameterInSimulationId = "EVENT_PARAMETER_IN_SIMULATION";
      private readonly string _simulationId = "SIMULATION_ID";

      protected override void Context()
      {
         base.Context();
         _modelRoot = new Container();
         _model = new OSPSuite.Core.Domain.Model();
         _simulation = new IndividualSimulation {Model = _model, IsLoaded = true}.WithId(_simulationId);
         _model.Root = _modelRoot;
         _eventBuidingBlock = new PKSimEvent().WithId(_eventId);
         _eventParameter = new Parameter().WithId(_eventParameterId).WithName("P1");
         _eventParameterInSimulation = new Parameter {BuildingBlockType = PKSimBuildingBlockType.Event}.WithId(_eventParameterInSimulationId).WithName("P1");
         _eventBuidingBlock.Add(_eventParameter);

         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("TempId", PKSimBuildingBlockType.Event) {BuildingBlock = _eventBuidingBlock});

         _eventParameterInSimulation.Origin.ParameterId = _eventParameter.Id;
         _eventParameterInSimulation.Origin.BuilingBlockId = _eventBuidingBlock.Id;
         _eventParameterInSimulation.Origin.SimulationId = _simulation.Id;
         _modelRoot.Add(_eventParameterInSimulation);
      }

      protected override void Because()
      {
         sut.ResetIdFor(_simulation);
      }

      [Observation]
      public void should_update_the_building_block_id_of_parameters_in_the_simulation_referencing_a_building_block()
      {
         _eventBuidingBlock.Id.ShouldNotBeEqualTo(_eventId);
         _eventParameterInSimulation.Origin.BuilingBlockId.ShouldBeEqualTo(_eventBuidingBlock.Id);
      }

      [Observation]
      public void should_update_the_simulation_id_of_parameters_defined_in_the_simulation()
      {
         _simulation.Id.ShouldNotBeEqualTo(_simulationId);
         _eventParameterInSimulation.Origin.SimulationId.ShouldBeEqualTo(_simulation.Id);
      }

      [Observation]
      public void should_update_the_parameter_id_of_parameters_defined_in_the_simulation_and_referencing_a_building_block_parameter_whose_id_was_changed()
      {
         _eventParameter.Id.ShouldNotBeEqualTo(_eventParameterId);
         _eventParameterInSimulation.Origin.ParameterId.ShouldBeEqualTo(_eventParameter.Id);
      }
   }

   public class When_resetting_the_id_in_a_simulation_where_the_building_blocks_do_not_exist_anymore : concern_for_ObjectIdResetter
   {
      private Simulation _simulation;
      private IModel _model;
      private IContainer _modelRoot;
      private IParameter _eventParameterInSimulation;
      private readonly string _eventId = "EVENT_ID";
      private readonly string _eventParameterId = "EVENT_PARAMETER";
      private readonly string _eventParameterInSimulationId = "EVENT_PARAMETER_IN_SIMULATION";
      private readonly string _simulationId = "SIMULATION_ID";

      protected override void Context()
      {
         base.Context();
         _modelRoot = new Container();
         _model = new OSPSuite.Core.Domain.Model();
         _simulation = new IndividualSimulation {Model = _model, IsLoaded = true}.WithId(_simulationId);
         _model.Root = _modelRoot;
         _eventParameterInSimulation = new Parameter {BuildingBlockType = PKSimBuildingBlockType.Event}.WithId(_eventParameterInSimulationId).WithName("P1");

         _eventParameterInSimulation.Origin.ParameterId = _eventParameterId;
         _eventParameterInSimulation.Origin.BuilingBlockId = _eventId;
         _eventParameterInSimulation.Origin.SimulationId = _simulation.Id;

         _modelRoot.Add(_eventParameterInSimulation);
      }

      protected override void Because()
      {
         sut.ResetIdFor(_simulation);
      }

      [Observation]
      public void should_reset_the_building_block_id_of_parameters_in_the_simulation_referencing_a_building_block()
      {
         _eventParameterInSimulation.Origin.BuilingBlockId.ShouldBeEqualTo(string.Empty);
         _eventParameterInSimulation.Origin.ParameterId.ShouldBeEqualTo(string.Empty);
      }

      [Observation]
      public void should_update_the_simulation_id_of_parameters_defined_in_the_simulation()
      {
         _simulation.Id.ShouldNotBeEqualTo(_simulationId);
         _eventParameterInSimulation.Origin.SimulationId.ShouldBeEqualTo(_simulation.Id);
      }
   }
}