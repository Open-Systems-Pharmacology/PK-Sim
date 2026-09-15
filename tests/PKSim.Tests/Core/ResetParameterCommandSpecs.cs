using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core
{
   public abstract class concern_for_ResetParameterCommand : ContextSpecification<ResetParameterCommand>
   {
      protected IParameter _parameterToReset;
      protected IExecutionContext _executionContext;
      protected double _originValue;
      private IDimension _dimension;
      protected IValueOriginRepository _parameterInContainerRepository;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _dimension = A.Fake<IDimension>();
         _originValue = 10;
         var container = new Container();
         var oneParameter = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("P1");
         var objectPathFactory = new ObjectPathFactoryForSpecs();
         _parameterToReset = DomainHelperForSpecs.ConstantParameterWithValue(_originValue).WithName("tralala").WithId("tralala").WithDimension(_dimension);
         _parameterToReset.Formula = new ExplicitFormula("10");
         container.Add(oneParameter);
         container.Add(_parameterToReset);
         _parameterToReset.Formula.AddObjectPath(objectPathFactory.CreateRelativeFormulaUsablePath(_parameterToReset, oneParameter));
         _parameterToReset.Origin.SimulationId = "SimId";
         A.CallTo(() => _executionContext.Get<IParameter>(_parameterToReset.Id)).Returns(_parameterToReset);
         A.CallTo(() => _executionContext.BuildingBlockContaining(_parameterToReset)).Returns(A.Fake<IPKSimBuildingBlock>());

         _parameterInContainerRepository = A.Fake<IValueOriginRepository>();
         A.CallTo(() => _executionContext.Resolve<IValueOriginRepository>()).Returns(_parameterInContainerRepository);

         sut = new ResetParameterCommand(_parameterToReset);
      }
   }

   public class When_executing_the_reset_command_for_a_non_default_parameter : concern_for_ResetParameterCommand
   {
      protected override void Context()
      {
         base.Context();
         _parameterToReset.Value = 25;
         _parameterToReset.ValueOrigin.Method = ValueOriginDeterminationMethods.ManualFit;
         _parameterToReset.ValueOrigin.Source = ValueOriginSources.ParameterIdentification;
         var valueOrigin = new ValueOrigin
         {
            Method = ValueOriginDeterminationMethods.Assumption,
            Source = ValueOriginSources.Internet,
         };

         A.CallTo(() => _parameterInContainerRepository.ValueOriginFor(_parameterToReset)).Returns(valueOrigin);
      }

      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_have_reset_the_parameter_value_to_its_original_value()
      {
         _parameterToReset.Value.ShouldBeEqualTo(_originValue);
      }

      [Observation]
      public void the_parameter_should_have_been_marked_as_fixed()
      {
         _parameterToReset.IsFixedValue.ShouldBeFalse();
      }

      [Observation]
      public void should_reset_the_parameter_value_origin()
      {
         _parameterToReset.ValueOrigin.Method.ShouldBeEqualTo(ValueOriginDeterminationMethods.Assumption);
         _parameterToReset.ValueOrigin.Source.ShouldBeEqualTo(ValueOriginSources.Internet);
      }
   }

   public class When_executing_the_reset_command_for_a_default_parameter : concern_for_ResetParameterCommand
   {
      private ValueOrigin _databaseValueOrigin;
      
      protected override void Context()
      {
         base.Context();
         _parameterToReset.Value = 25;
         _parameterToReset.ValueOrigin.Method = ValueOriginDeterminationMethods.ManualFit;
         _parameterToReset.ValueOrigin.Source = ValueOriginSources.ParameterIdentification;

         _databaseValueOrigin = new ValueOrigin
         {
            Method = ValueOriginDeterminationMethods.InVivo,
            Source = ValueOriginSources.Database
         };

         A.CallTo(() => _parameterInContainerRepository.ValueOriginFor(_parameterToReset)).Returns(_databaseValueOrigin);
      }

      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_have_reset_the_parameter_value_to_its_original_value()
      {
         _parameterToReset.Value.ShouldBeEqualTo(_originValue);
      }

      [Observation]
      public void the_parameter_should_have_been_marked_as_fixed()
      {
         _parameterToReset.IsFixedValue.ShouldBeFalse();
      }

      [Observation]
      public void should_reset_the_parameter_default_state_to_true()
      {
         _parameterToReset.IsDefault.ShouldBeTrue();
         _parameterToReset.ValueOrigin.Method.ShouldBeEqualTo(ValueOriginDeterminationMethods.InVivo);
         _parameterToReset.ValueOrigin.Source.ShouldBeEqualTo(ValueOriginSources.Database);
      }
   }

   public class When_executing_the_inverse_command_of_the_reset_parameter_command : concern_for_ResetParameterCommand
   {
      protected override void Context()
      {
         base.Context();
         _parameterToReset.Value = 25;
         _parameterToReset.IsDefault = false;
         _parameterToReset.ValueOrigin.Method = ValueOriginDeterminationMethods.ManualFit;
         _parameterToReset.ValueOrigin.Source = ValueOriginSources.ParameterIdentification;
         _parameterToReset.ValueOrigin.Description = "Fitted to data";

         A.CallTo(() => _parameterInContainerRepository.ValueOriginFor(_parameterToReset)).Returns(new ValueOrigin
         {
            Method = ValueOriginDeterminationMethods.InVivo,
            Source = ValueOriginSources.Database
         });
      }

      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_executionContext);
      }

      [Observation]
      public void should_set_the_value_of_the_parameter_again_using_a_set_command()
      {
         _parameterToReset.Value.ShouldBeEqualTo(25);
         _parameterToReset.IsFixedValue.ShouldBeTrue();
      }

      [Observation]
      public void should_restore_the_default_state_of_the_parameter()
      {
         _parameterToReset.IsDefault.ShouldBeFalse();
      }

      [Observation]
      public void should_restore_the_value_origin_of_the_parameter()
      {
         _parameterToReset.ValueOrigin.Method.ShouldBeEqualTo(ValueOriginDeterminationMethods.ManualFit);
         _parameterToReset.ValueOrigin.Source.ShouldBeEqualTo(ValueOriginSources.ParameterIdentification);
         _parameterToReset.ValueOrigin.Description.ShouldBeEqualTo("Fitted to data");
      }
   }

   public class When_undoing_the_reset_of_a_parameter_whose_origin_parameter_has_a_different_default_state : concern_for_ResetParameterCommand
   {
      private IParameter _originParameter;

      protected override void Context()
      {
         base.Context();
         _parameterToReset.Value = 25;
         _parameterToReset.IsDefault = false;
         _parameterToReset.Origin.BuilingBlockId = "BuildingBlockId";
         _parameterToReset.Origin.ParameterId = "OriginParameterId";

         _originParameter = DomainHelperForSpecs.ConstantParameterWithValue(25).WithName("tralala").WithId("OriginParameterId");
         _originParameter.IsDefault = true;
         A.CallTo(() => _executionContext.Get<IParameter>("OriginParameterId")).Returns(_originParameter);
      }

      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_executionContext);
      }

      [Observation]
      public void should_restore_the_default_state_of_the_parameter()
      {
         _parameterToReset.IsDefault.ShouldBeFalse();
      }

      [Observation]
      public void should_restore_the_default_state_of_the_origin_parameter()
      {
         _originParameter.IsDefault.ShouldBeTrue();
      }
   }

   public class When_redoing_a_reset_parameter_command : concern_for_ResetParameterCommand
   {
      protected override void Context()
      {
         base.Context();
         _parameterToReset.Value = 25;
         _parameterToReset.IsDefault = false;
      }

      protected override void Because()
      {
         var undo = sut.ExecuteAndInvokeInverse(_executionContext);
         undo.DowncastTo<IReversibleCommand<IExecutionContext>>().InvokeInverse(_executionContext);
      }

      [Observation]
      public void should_reset_the_parameter_again()
      {
         _parameterToReset.Value.ShouldBeEqualTo(_originValue);
         _parameterToReset.IsFixedValue.ShouldBeFalse();
         _parameterToReset.IsDefault.ShouldBeTrue();
      }
   }

   public class When_resetting_a_compound_dependent_simulation_parameter : concern_for_ResetParameterCommand
   {
      private IndividualSimulation _simulation;
      private IEntityPathResolver _entityPathResolver;

      protected override void Context()
      {
         base.Context();
         _entityPathResolver = A.Fake<IEntityPathResolver>();

         _parameterToReset.BuildingBlockType = PKSimBuildingBlockType.Simulation;
         _parameterToReset.Value = 25;

         var compound = new Compound { Name = "Aspirin" };
         _simulation = new IndividualSimulation { Id = "SimId" };
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("CompId", PKSimBuildingBlockType.Compound) { BuildingBlock = compound });
         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|tralala");

         A.CallTo(() => _executionContext.Get<Simulation>("SimId")).Returns(_simulation);
         A.CallTo(() => _executionContext.Resolve<IEntityPathResolver>()).Returns(_entityPathResolver);
         A.CallTo(() => _entityPathResolver.PathFor(_parameterToReset)).Returns("Organism|Aspirin|tralala");
      }

      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_remove_the_parameter_path_from_the_tracker()
      {
         _simulation.ParameterChangeTracker.HasUncommittedChanges.ShouldBeFalse();
      }
   }

   public class When_undoing_a_reset_command_that_untracked_a_parameter : concern_for_ResetParameterCommand
   {
      private IndividualSimulation _simulation;
      private IEntityPathResolver _entityPathResolver;

      protected override void Context()
      {
         base.Context();
         _entityPathResolver = A.Fake<IEntityPathResolver>();

         _parameterToReset.BuildingBlockType = PKSimBuildingBlockType.Simulation;
         _parameterToReset.Value = 25;

         var compound = new Compound { Name = "Aspirin" };
         _simulation = new IndividualSimulation { Id = "SimId" };
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("CompId", PKSimBuildingBlockType.Compound) { BuildingBlock = compound });
         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|tralala");

         A.CallTo(() => _executionContext.Get<Simulation>("SimId")).Returns(_simulation);
         A.CallTo(() => _executionContext.Resolve<IEntityPathResolver>()).Returns(_entityPathResolver);
         A.CallTo(() => _entityPathResolver.PathFor(_parameterToReset)).Returns("Organism|Aspirin|tralala");
      }

      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_executionContext);
      }

      [Observation]
      public void should_re_track_the_parameter_path()
      {
         _simulation.ParameterChangeTracker.HasUncommittedChanges.ShouldBeTrue();
         _simulation.ParameterChangeTracker.IsTracked("Organism|Aspirin|tralala").ShouldBeTrue();
      }
   }

   public abstract class concern_for_resetting_a_parameter_overwritten_by_the_overwrite_parameter_set_applied_to_the_simulation : concern_for_ResetParameterCommand
   {
      protected IndividualSimulation _simulation;
      protected const string PARAMETER_PATH = "Aspirin|tralala";

      protected override void Context()
      {
         base.Context();
         var entityPathResolver = A.Fake<IEntityPathResolver>();

         //the overwrite parameter set application flags the parameter as a compound parameter
         _parameterToReset.BuildingBlockType = PKSimBuildingBlockType.Compound;
         _parameterToReset.Value = 25;

         var compound = new Compound { Name = "Aspirin" };
         _simulation = new IndividualSimulation { Id = "SimId" };
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("CompId", PKSimBuildingBlockType.Compound) { BuildingBlock = compound });

         var overwriteParameterSet = new OverwriteParameterSet { Name = "MySet" };
         overwriteParameterSet.Add(new ParameterValue { Path = PARAMETER_PATH.ToObjectPath(), Value = 25 });
         _simulation.AddOverwriteParameterSetSelection("Aspirin", overwriteParameterSet);

         A.CallTo(() => _executionContext.Get<Simulation>("SimId")).Returns(_simulation);
         A.CallTo(() => _executionContext.Resolve<IEntityPathResolver>()).Returns(entityPathResolver);
         A.CallTo(() => entityPathResolver.PathFor(_parameterToReset)).Returns(PARAMETER_PATH);
      }
   }

   public class When_resetting_a_parameter_overwritten_by_the_overwrite_parameter_set_applied_to_the_simulation : concern_for_resetting_a_parameter_overwritten_by_the_overwrite_parameter_set_applied_to_the_simulation
   {
      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_track_the_parameter_path_as_a_pending_removal_from_the_set()
      {
         _simulation.ParameterChangeTracker.IsTracked(PARAMETER_PATH).ShouldBeTrue();
      }
   }

   public class When_undoing_the_reset_of_a_parameter_overwritten_by_the_overwrite_parameter_set_applied_to_the_simulation : concern_for_resetting_a_parameter_overwritten_by_the_overwrite_parameter_set_applied_to_the_simulation
   {
      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_executionContext);
      }

      [Observation]
      public void should_untrack_the_parameter_path_again()
      {
         _simulation.ParameterChangeTracker.HasUncommittedChanges.ShouldBeFalse();
      }
   }

   public class When_redoing_the_reset_of_a_parameter_overwritten_by_the_overwrite_parameter_set_applied_to_the_simulation : concern_for_resetting_a_parameter_overwritten_by_the_overwrite_parameter_set_applied_to_the_simulation
   {
      protected override void Because()
      {
         var undo = sut.ExecuteAndInvokeInverse(_executionContext);
         undo.DowncastTo<IReversibleCommand<IExecutionContext>>().InvokeInverse(_executionContext);
      }

      [Observation]
      public void should_track_the_parameter_path_as_a_pending_removal_again()
      {
         _simulation.ParameterChangeTracker.IsTracked(PARAMETER_PATH).ShouldBeTrue();
      }
   }

   public class When_undoing_the_reset_of_an_already_tracked_parameter_overwritten_by_the_overwrite_parameter_set_applied_to_the_simulation : concern_for_resetting_a_parameter_overwritten_by_the_overwrite_parameter_set_applied_to_the_simulation
   {
      protected override void Context()
      {
         base.Context();
         _simulation.ParameterChangeTracker.Track(PARAMETER_PATH);
      }

      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_executionContext);
      }

      [Observation]
      public void should_keep_the_parameter_path_tracked()
      {
         _simulation.ParameterChangeTracker.IsTracked(PARAMETER_PATH).ShouldBeTrue();
      }
   }
}