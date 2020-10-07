using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure;
using OSPSuite.Core.Domain;
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
      }

   }

   public class When_creating_a_metabolism_expression_for_an_individual : concern_for_IndividualEnzymeFactory
   {
      private IndividualMolecule _result;

     
      protected override void Because()
      {
         _result = sut.CreateFor(_individual);
      }

      [Observation]
      public void should_return_a_metabolism_expression()
      {
         _result.ShouldBeAnInstanceOf<IndividualEnzyme>();
      }

      [Observation]
      public void should_return_an_expression_containing_at_least_the_container_blood_cells_and_plasma()
      {
         _result.ContainsName(CoreConstants.Compartment.BloodCells).ShouldBeTrue();
         _result.ContainsName(CoreConstants.Compartment.Plasma).ShouldBeTrue();
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
         _undefined = sut.UndefinedLiverFor(_individual);
      }

      [Observation]
      public void should_add_the_relative_expression_to_periportal_and_pericentral_and_set_the_value_to_100()
      {
         _undefined.ExpressionContainer(CoreConstants.Compartment.Pericentral).RelativeExpression.ShouldBeEqualTo(1);
         _undefined.ExpressionContainer(CoreConstants.Compartment.Periportal).RelativeExpression.ShouldBeEqualTo(1);
      }

   }
}