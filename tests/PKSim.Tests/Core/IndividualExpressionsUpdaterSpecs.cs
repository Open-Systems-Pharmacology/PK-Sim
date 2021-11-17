using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Exceptions;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_IndividualExpressionsUpdater : ContextSpecification<IIndividualExpressionsUpdater>
   {
      protected Individual _targetIndividual;
      protected Individual _sourceIndividual;
      protected IMoleculeExpressionTask<Individual> _moleculeExpressionTask;
      protected Individual _targetIndividualOtherSpecies;
      protected ExpressionProfile _expressionProfile1;

      protected override void Context()
      {
         _sourceIndividual = DomainHelperForSpecs.CreateIndividual();
         _targetIndividual = DomainHelperForSpecs.CreateIndividual();
         _expressionProfile1 = new ExpressionProfile();
         _targetIndividualOtherSpecies = DomainHelperForSpecs.CreateIndividual(speciesName: "OTHER_SPECIES");
         _moleculeExpressionTask = A.Fake<IMoleculeExpressionTask<Individual>>();

         sut = new IndividualExpressionsUpdater(_moleculeExpressionTask);

         _sourceIndividual.AddExpressionProfile(_expressionProfile1);

      }
   }

   public class When_updating_the_expression_profile_from_one_individual_to_another_using_the_same_species : concern_for_IndividualExpressionsUpdater
   {
 protected override void Because()
      {
         sut.Update(_sourceIndividual, _targetIndividual);
      }

      [Observation]
      public void should_add_the_same_expression_profile_to_the_target_individual()
      {
         A.CallTo(() => _moleculeExpressionTask.AddExpressionProfile(_targetIndividual, _expressionProfile1)).MustHaveHappened();
      }
   }


   public class When_updating_the_expression_profile_from_one_individual_to_another_using_the_different_species : concern_for_IndividualExpressionsUpdater
   {
      protected override void Because()
      {
         sut.Update(_sourceIndividual, _targetIndividualOtherSpecies);
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(()=> sut.Update(_sourceIndividual, _targetIndividualOtherSpecies)).ShouldThrowAn<OSPSuiteException>();
      }
   }
}