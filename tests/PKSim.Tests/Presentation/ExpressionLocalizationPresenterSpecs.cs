using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Services;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.Presentation
{
   public abstract class concern_for_ExpressionLocalizationPresenter : ContextSpecification<IExpressionLocalizationPresenter<Individual>>
   {
      protected IExpressionLocalizationView _view;
      protected IMoleculeExpressionTask<Individual> _moleculeExpressionTask;
      protected IDialogCreator _dialogCreator;
      protected IndividualProtein _molecule;
      protected Individual _individual;
      protected bool _notified = false;
      protected ICommand _command;
      protected ICommandCollector _commandCollector;

      protected override void Context()
      {
         _view = A.Fake<IExpressionLocalizationView>();
         _moleculeExpressionTask = A.Fake<IMoleculeExpressionTask<Individual>>();
         _dialogCreator = A.Fake<IDialogCreator>();
         sut = new ExpressionLocalizationPresenter<Individual>(_view, _moleculeExpressionTask, _dialogCreator);
         _commandCollector = new PKSimMacroCommand();
         sut.InitializeWith(_commandCollector);
         _molecule = new IndividualEnzyme();
         _individual = new Individual();
         sut.Edit(_molecule, _individual);

         sut.LocalizationChanged += (o, e) => _notified = true;

         _command = A.Fake<IPKSimCommand>();
         A.CallTo(_moleculeExpressionTask).WithReturnType<ICommand>().Returns(_command);
      }
   }

   public class When_switching_on_a_localization_for_a_given_molecule : concern_for_ExpressionLocalizationPresenter
   {
      protected override void Because()
      {
         sut.UpdateLocalization(Localization.Intracellular, true);
      }

      [Observation]
      public void should_update_the_localization()
      {
         A.CallTo(() => _moleculeExpressionTask.SetExpressionLocalizationFor(_molecule, Localization.Intracellular, _individual)).MustHaveHappened();
      }

      [Observation]
      public void should_not_notify_the_user_that_parameter_might_be_reset()
      {
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(A<string>._, ViewResult.Yes)).MustNotHaveHappened();
      }

      [Observation]
      public void should_notify_a_localization_changed()
      {
         _notified.ShouldBeTrue();
      }

      [Observation]
      public void should_add_the_update_command_to_the_history()
      {
         _commandCollector.All().ShouldContain(_command);
      }
   }

   public class
      When_switching_off_a_localization_for_a_given_molecule_that_would_not_result_in_a_category_being_empty :
         concern_for_ExpressionLocalizationPresenter
   {
      private bool _localizationUpdated;

      protected override void Context()
      {
         base.Context();
         _molecule.Localization = Localization.Interstitial;
      }

      protected override void Because()
      {
         _localizationUpdated = sut.UpdateLocalization(Localization.Intracellular, false);
      }

      [Observation]
      public void should_update_the_localization()
      {
         A.CallTo(() => _moleculeExpressionTask.SetExpressionLocalizationFor(_molecule, Localization.Intracellular, _individual)).MustHaveHappened();
      }

      [Observation]
      public void should_not_notify_the_user_that_parameter_might_be_reset()
      {
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(A<string>._, ViewResult.Yes)).MustNotHaveHappened();
      }

      [Observation]
      public void should_notify_a_localization_changed()
      {
         _notified.ShouldBeTrue();
      }

      [Observation]
      public void should_return_the_the_localization_was_updated()
      {
         _localizationUpdated.ShouldBeTrue();
      }

      [Observation]
      public void should_add_the_update_command_to_the_history()
      {
         _commandCollector.All().ShouldContain(_command);
      }
   }

   public class
      When_switching_off_a_localization_for_a_given_molecule_that_would_result_in_a_category_being_empty_and_the_user_accepts_the_update :
         concern_for_ExpressionLocalizationPresenter
   {
      protected override void Context()
      {
         base.Context();
         _molecule.Localization = Localization.BloodCellsIntracellular;
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(A<string>._, ViewResult.Yes)).Returns(ViewResult.Yes);
      }

      protected override void Because()
      {
         sut.UpdateLocalization(Localization.BloodCellsIntracellular, false);
      }

      [Observation]
      public void should_update_the_localization()
      {
         A.CallTo(() => _moleculeExpressionTask.SetExpressionLocalizationFor(_molecule, Localization.BloodCellsIntracellular, _individual))
            .MustHaveHappened();
      }

      [Observation]
      public void should_notify_the_user_that_parameter_might_be_reset()
      {
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(A<string>._, ViewResult.Yes)).MustHaveHappened();
      }

      [Observation]
      public void should_notify_a_localization_changed()
      {
         _notified.ShouldBeTrue();
      }

      [Observation]
      public void should_add_the_update_command_to_the_history()
      {
         _commandCollector.All().ShouldContain(_command);
      }
   }

   public class
      When_switching_off_a_localization_for_a_given_molecule_that_would_result_in_a_category_being_empty_and_the_user_cancels_the_update :
         concern_for_ExpressionLocalizationPresenter
   {
      private bool _localizationUpdated;

      protected override void Context()
      {
         base.Context();
         _molecule.Localization = Localization.BloodCellsIntracellular;
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(A<string>._, ViewResult.Yes)).Returns(ViewResult.No);
      }

      protected override void Because()
      {
         _localizationUpdated = sut.UpdateLocalization(Localization.BloodCellsIntracellular, false);
      }

      [Observation]
      public void should_not_update_the_localization()
      {
         A.CallTo(() => _moleculeExpressionTask.SetExpressionLocalizationFor(_molecule, Localization.BloodCellsIntracellular, _individual))
            .MustNotHaveHappened();
      }

      [Observation]
      public void should_notify_the_user_that_parameter_might_be_reset()
      {
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(A<string>._, ViewResult.Yes)).MustHaveHappened();
      }

      [Observation]
      public void should_not_notify_a_localization_changed()
      {
         _notified.ShouldBeFalse();
      }

      [Observation]
      public void should_return_the_the_localization_was_not_updated()
      {
         _localizationUpdated.ShouldBeFalse();
      }


      [Observation]
      public void should_not_add_the_update_command_to_the_history()
      {
         _commandCollector.All().ShouldNotContain(_command);
      }
   }
}