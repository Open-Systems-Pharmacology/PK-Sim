using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Container;
using PKSim.Core.Services;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_IndividualEnzymeFactory : ContextForIntegration<IIndividualEnzymeTask>
   {
      protected Individual _individual;
      public override void GlobalContext()
      {
         base.GlobalContext();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         sut = IoC.Resolve<IIndividualEnzymeTask>();
      }

   }

   public class When_creating_a_metabolism_expression_for_an_individual : concern_for_IndividualEnzymeFactory
   {
      private IndividualMolecule _result;
      private ICache<string, IParameter> _allExpressionParameters;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _result = sut.AddMoleculeTo(_individual, "CYP3A4");
      }

      protected override void Because()
      {
         _allExpressionParameters = _individual.AllExpressionParametersFor(_result);
      }

      [Observation]
      public void should_return_a_metabolism_expression()
      {
         _result.ShouldBeAnInstanceOf<IndividualEnzyme>();
      }

      [Observation]
      public void should_return_an_expression_containing_at_least_the_container_blood_cells_and_plasma()
      {
         _allExpressionParameters.Contains(CoreConstants.Compartment.BloodCells).ShouldBeTrue();
         _allExpressionParameters.Contains(CoreConstants.Compartment.Plasma).ShouldBeTrue();
      }

      [Observation]
      public void should_have_marked_the_reference_concentration_parameter_as_variable()
      {
         _result.ReferenceConcentration.CanBeVaried.ShouldBeTrue();
         _result.ReferenceConcentration.CanBeVariedInPopulation.ShouldBeTrue();
      }
   }

   public class When_creating_an_undefined_enzyme_for_a_given_individual : concern_for_IndividualEnzymeFactory
   {
      private IndividualEnzyme _undefined;

      protected override void Because()
      {
         _undefined = sut.AddUndefinedLiverTo(_individual);
      }

      [Observation]
      public void should_add_the_relative_expression_to_periportal_and_pericentral_and_set_the_value_to_100()
      {
         var allExpressionsParameters = _individual.AllExpressionParametersFor(_undefined);
         allExpressionsParameters[CoreConstants.Compartment.Pericentral].Value.ShouldBeEqualTo(1);
         allExpressionsParameters[CoreConstants.Compartment.Periportal].Value.ShouldBeEqualTo(1);
      }

   }
}