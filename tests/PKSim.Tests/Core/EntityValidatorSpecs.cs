using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_EntityValidator : ContextSpecification<IEntityValidator>
   {
      private IParameterIdentificationValidator _parameterIdentificaitonValidator;
      private ISensitivityAnalysisValidator _sensitivityAnalysisValidator;

      protected override void Context()
      {
         _parameterIdentificaitonValidator= A.Fake<IParameterIdentificationValidator>();
         _sensitivityAnalysisValidator = A.Fake<ISensitivityAnalysisValidator>();
         sut = new EntityValidator(_parameterIdentificaitonValidator,_sensitivityAnalysisValidator);
      }
   }

   public class When_validating_a_container_container_two_parameters_with_one_invalid : concern_for_EntityValidator
   {
      private IContainer _container;
      private ValidationResult _result;
      private IParameter _invalidParameter;

      protected override void Context()
      {
         base.Context();
         _container = new Container().WithName("Container");
         _container.Add(DomainHelperForSpecs.ConstantParameterWithValue(5).WithName("Valid"));
         _invalidParameter = DomainHelperForSpecs.ConstantParameterWithValue(10).WithName("Invalid");
         _invalidParameter.Info.MaxIsAllowed = false;
         _invalidParameter.Info.MaxValue = 10;
         _container.Add(_invalidParameter);
      }

      protected override void Because()
      {
         _result = sut.Validate(_container);
      }

      [Observation]
      public void should_return_a_validation_results_having_the_state_invalid()
      {
         _result.ValidationState.ShouldBeEqualTo(ValidationState.Invalid);
      }

      [Observation]
      public void the_validation_messages_should_contain_the_entity_invalid()
      {
         _result.Messages.Count().ShouldBeEqualTo(1);
         _result.Messages.ElementAt(0).Object.ShouldBeEqualTo(_invalidParameter);
      }
   }

   public class When_validing_a_parameter_defined_in_a_dynamic_molecule_container_of_a_simulation : concern_for_EntityValidator
   {
      private ValidationResult _result;
      private Simulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = new IndividualSimulation();
         _simulation.SimulationSettings = new SimulationSettings();
         _simulation.Root = new RootContainer();
         _simulation.Model = new OSPSuite.Core.Domain.Model();
         _simulation.Model.Root = new RootContainer();
         var moleculeContainer = new Container {ContainerType = ContainerType.Molecule}.WithName("MOLECULE");
         _simulation.Model.Root.Add(moleculeContainer);

         var invalidParameter = DomainHelperForSpecs.ConstantParameterWithValue(10).WithName("Invalid");
         invalidParameter.Info.MaxIsAllowed = false;
         invalidParameter.Info.MaxValue = 10;
         invalidParameter.WithParentContainer(moleculeContainer);

         var compound = A.Fake<Compound>();
         compound.Name = "COMPOUND";
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Compound", PKSimBuildingBlockType.Compound) {BuildingBlock = compound});
      }

      protected override void Because()
      {
         _result = sut.Validate(_simulation);
      }

      [Observation]
      public void should_not_have_any_error_message_even_if_one_parameter_is_not_valid()
      {
         _result.Messages.Count().ShouldBeEqualTo(0);
         _result.ValidationState.ShouldBeEqualTo(ValidationState.Valid);
      }
   }

   public class When_validing_a_parameter_defined_in_a_dynamic_compound_container_of_a_simulation : concern_for_EntityValidator
   {
      private ValidationResult _result;
      private Simulation _simulation;
      private IParameter _invalidParameter;

      protected override void Context()
      {
         base.Context();
         _simulation = new IndividualSimulation {Model = new OSPSuite.Core.Domain.Model {Root = new RootContainer()}};
         _simulation.SimulationSettings = new SimulationSettings();
         var compound = A.Fake<Compound>();
         compound.Name = "COMPOUND";
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Compound", PKSimBuildingBlockType.Compound) {BuildingBlock = compound});


         var moleculeContainer = new Container {ContainerType = ContainerType.Molecule}.WithName(compound.Name);
         _simulation.Model.Root.Add(moleculeContainer);

         _invalidParameter = DomainHelperForSpecs.ConstantParameterWithValue(10).WithName("Invalid");
         _invalidParameter.Info.MaxIsAllowed = false;
         _invalidParameter.Info.MaxValue = 10;
         _invalidParameter.WithParentContainer(moleculeContainer);
      }

      protected override void Because()
      {
         _result = sut.Validate(_simulation);
      }

      [Observation]
      public void should_show_an_error_messaage()
      {
         _result.Messages.Count().ShouldBeEqualTo(1);
         _result.Messages.ElementAt(0).Object.ShouldBeEqualTo(_invalidParameter);
      }
   }
}