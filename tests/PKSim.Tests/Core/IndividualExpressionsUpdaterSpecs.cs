using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Services;
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
      protected IDialogCreator _dialogCreator;

      protected override void Context()
      {
         _sourceIndividual = DomainHelperForSpecs.CreateIndividual();
         _targetIndividual = DomainHelperForSpecs.CreateIndividual();
         _expressionProfile1 = new ExpressionProfile();
         _targetIndividualOtherSpecies = DomainHelperForSpecs.CreateIndividual(speciesName: "OTHER_SPECIES");
         _moleculeExpressionTask = A.Fake<IMoleculeExpressionTask<Individual>>();
         _dialogCreator= A.Fake<IDialogCreator>();
         sut = new IndividualExpressionsUpdater(_moleculeExpressionTask, _dialogCreator);

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
      public void should_warn_the_user_that_the_expression_won_t_be_used()
      {
         A.CallTo(() => _dialogCreator.MessageBoxInfo(A<string>._)).MustHaveHappened();
      }

      [Observation]
      public void should_not_add_the_same_expression_profile_to_the_target_individual()
      {
         A.CallTo(() => _moleculeExpressionTask.AddExpressionProfile(_targetIndividual, _expressionProfile1)).MustNotHaveHappened();
      }
   }
}