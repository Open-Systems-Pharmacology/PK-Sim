using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Services;
using FakeItEasy;
using NUnit.Framework;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Extensions;
using PKSim.Presentation.DTO.Populations;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.Populations;

using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   public abstract class concern_for_ImportPopulationSettingsPresenter : ContextSpecification<IImportPopulationSettingsPresenter>
   {
      protected IImportPopulationSettingsView _view;
      protected ILazyLoadTask _lazyLoadTask;
      protected IDialogCreator _dialogCreator;
      protected IImportPopulationFactory _importPopulationFactory;
      protected Individual _baseIndividual;
      protected ImportPopulationSettingsDTO _importSettingsDTO;

      protected override void Context()
      {
         _view = A.Fake<IImportPopulationSettingsView>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _importPopulationFactory = A.Fake<IImportPopulationFactory>();
         _baseIndividual = new Individual();
         sut = new ImportPopulationSettingsPresenter(_view, _lazyLoadTask, _dialogCreator, _importPopulationFactory);

         A.CallTo(() => _view.BindTo(A<ImportPopulationSettingsDTO>._))
            .Invokes(x => _importSettingsDTO = x.GetArgument<ImportPopulationSettingsDTO>(0));

         sut.PrepareForCreating(_baseIndividual);
      }
   }

   public class When_told_to_create_a_population_based_on_an_individual : concern_for_ImportPopulationSettingsPresenter
   {
      [Observation]
      public void should_load_the_individual()
      {
         A.CallTo(() => _lazyLoadTask.Load(_baseIndividual)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_view_with_the_selected_individual()
      {
         _importSettingsDTO.Individual.ShouldBeEqualTo(_baseIndividual);
      }
   }

   public class When_adding_a_population_file : concern_for_ImportPopulationSettingsPresenter
   {
      private string _newFile;

      protected override void Context()
      {
         base.Context();
         _newFile = "new file path";
         A.CallTo(() => _dialogCreator.AskForFileToOpen(PKSimConstants.UI.SelectPopulationFileToImport, Constants.Filter.CSV_FILE_FILTER, Constants.DirectoryKey.POPULATION, null, null))
            .Returns(_newFile);
      }

      protected override void Because()
      {
         sut.AddFile();
      }

      [Observation]
      public void should_add_the_file_to_the_list_of_files_to_import()
      {
         _importSettingsDTO.PopulationFiles.Select(x => x.FilePath).ShouldContain(_newFile);
      }
   }

   public class When_adding_a_population_file_and_the_action_is_cancelled : concern_for_ImportPopulationSettingsPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _dialogCreator.AskForFileToOpen(PKSimConstants.UI.SelectPopulationFileToImport, Constants.Filter.CSV_FILE_FILTER, Constants.DirectoryKey.POPULATION, null, null))
            .Returns(string.Empty);
      }

      protected override void Because()
      {
         sut.AddFile();
      }

      [Observation]
      public void should_not_add_any_new_file_to_the_list()
      {
         _importSettingsDTO.PopulationFiles.Count.ShouldBeEqualTo(0);
      }
   }

   public class When_starting_the_create_population : concern_for_ImportPopulationSettingsPresenter
   {
      protected override void Because()
      {
         sut.CreatePopulation();
      }

      [Observation]
      public void should_start_the_population_creation()
      {
         A.CallTo(_importPopulationFactory).WithReturnType<Task<ImportPopulation>>().MustHaveHappened();
      }
   }

   public class When_the_created_population_was_imported : concern_for_ImportPopulationSettingsPresenter
   {
      private ImportPopulation _population;
      private bool _success;
      private bool _hasWarningOrError;

      protected override void Context()
      {
         base.Context();
         _population = A.Fake<ImportPopulation>();
         _population.Name = "toto";


         A.CallTo(() => _importPopulationFactory.CreateFor(A<IReadOnlyList<string>>._, _baseIndividual, A<CancellationToken>._))
            .Returns(_population);

         sut.PopulationCreationFinished += (o, e) =>
         {
            _success = e.Success;
            _hasWarningOrError = e.HasWarningOrError;
         };
      }

      [TestCase(true, true)]
      [TestCase(true, false)]
      [TestCase(false, false)]
      public void should_notify_the_population_created_event_with_the_expected_value(bool success, bool hasErrorsOrWarning)
      {
         A.CallTo(() => _population.ImportSuccessful).Returns(success);
         A.CallTo(() => _population.ImportHasWarningOrError).Returns(hasErrorsOrWarning);

         sut.CreatePopulation();

         _success.ShouldBeEqualTo(success);
         _hasWarningOrError.ShouldBeEqualTo(hasErrorsOrWarning);
      }
   }

   public class When_the_user_stops_the_creation_of_the_import_population : concern_for_ImportPopulationSettingsPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _importPopulationFactory.CreateFor(A<IReadOnlyList<string>>._, _baseIndividual, A<CancellationToken>._)).Throws<OperationCanceledException>();
         sut.CreatePopulation();
      }

      protected override void Because()
      {
         sut.Cancel();
      }

      [Observation]
      public void should_have_the_population_created_set_to_false()
      {
         sut.PopulationCreated.ShouldBeFalse();
      }
   }

   public class When_checking_if_the_presenter_import_settings_presenter_is_in_a_valid_state : concern_for_ImportPopulationSettingsPresenter
   {
      [Observation]
      public void can_close_if_the_view_is_in_a_valid_state_and_at_least_a_file_was_selected()
      {
         _importSettingsDTO.AddFile("One File");
         A.CallTo(() => _view.HasError).Returns(false);
         sut.CanClose.ShouldBeTrue();
      }

      [Observation]
      public void can_not_close_if_the_view_has_in_invalid_state()
      {
         _importSettingsDTO.AddFile("One File");
         A.CallTo(() => _view.HasError).Returns(true);
         sut.CanClose.ShouldBeFalse();
      }

      [Observation]
      public void can_not_close_if_no_file_was_selected()
      {
         A.CallTo(() => _view.HasError).Returns(false);
         sut.CanClose.ShouldBeFalse();
      }
   }
}