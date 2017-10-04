using System.Collections.Generic;
using System.Threading;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Services;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Extensions;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Presentation
{
   public abstract class concern_for_ImportSimulationResultsPresenter : ContextSpecification<IImportSimulationResultsPresenter>
   {
      protected IDialogCreator _dialogCreator;
      protected IImportSimulationResultsView _view;
      protected ISimulationResultsImportTask _simulationResultsImportTask;
      protected PopulationSimulation _populationSimulation;
      protected ImportSimulationResultsDTO _dto;
      protected SimulationResults _simulationResults;
      protected readonly IList<SimulationResultsFile> _simulationResultsFiles = new List<SimulationResultsFile>();

      protected SimulationResultsImport _importedResults;

      protected override void Context()
      {
         _dialogCreator = A.Fake<IDialogCreator>();
         _view = A.Fake<IImportSimulationResultsView>();
         _simulationResultsImportTask = A.Fake<ISimulationResultsImportTask>();
         _populationSimulation = A.Fake<PopulationSimulation>();
         _importedResults = A.Fake<SimulationResultsImport>();
         _simulationResults = new SimulationResults();

         sut = new ImportSimulationResultsPresenter(_view, _simulationResultsImportTask, _dialogCreator);

         A.CallTo(() => _view.BindTo(A<ImportSimulationResultsDTO>._))
            .Invokes(x => _dto = x.GetArgument<ImportSimulationResultsDTO>(0));

         A.CallTo(() => _simulationResultsImportTask.ImportResults(_populationSimulation, A<IReadOnlyCollection<string>>._, A<CancellationToken>._))
            .Returns(_importedResults);

         A.CallTo(() => _importedResults.SimulationResults).Returns(_simulationResults);
         A.CallTo(() => _importedResults.SimulationResultsFiles).Returns(_simulationResultsFiles);


         sut.ImportResultsFor(_populationSimulation);
      }
   }

   public class When_import_simulation_results_for_a_given_simulation_population : concern_for_ImportSimulationResultsPresenter
   {
      [Observation]
      public void should_start_the_view_that_will_allow_the_user_to_select_the_files_to_import()
      {
         A.CallTo(() => _view.BindTo(A<ImportSimulationResultsDTO>._)).MustHaveHappened();
      }
   }

   public class When_the_presenter_is_being_notified_that_the_view_status_has_changed : concern_for_ImportSimulationResultsPresenter
   {
      [Observation]
      public void should_enable_the_import_button_if_a_file_was_defiend()
      {
         _dto.AddFile("A_file");
         sut.ViewChanged();
         _view.ImportEnabled.ShouldBeTrue();
      }

      [Observation]
      public void should_disable_the_import_button_if_no_file_is_defiend()
      {
         sut.ViewChanged();
         _view.ImportEnabled.ShouldBeFalse();
      }
   }

   public class When_adding_a_file_to_import_and_the_user_cancels_the_action : concern_for_ImportSimulationResultsPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns(string.Empty);
      }

      protected override void Because()
      {
         sut.AddFile();
      }

      [Observation]
      public void should_not_do_anything_if_the_user_cancels_the_action()
      {
         _dto.SimulationResultsFile.Count.ShouldBeEqualTo(0);
      }
   }

   public class When_the_user_selects_a_file_to_import : concern_for_ImportSimulationResultsPresenter
   {
      private string _fileToImport;

      protected override void Context()
      {
         base.Context();
         _fileToImport = "File_to_import";
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns(_fileToImport);
      }

      protected override void Because()
      {
         sut.AddFile();
      }

      [Observation]
      public void should_not_do_anything_if_the_user_cancels_the_action()
      {
         _dto.SimulationResultsFile.Count.ShouldBeEqualTo(1);
         _dto.SimulationResultsFile[0].FilePath.ShouldBeEqualTo(_fileToImport);
      }
   }

   public class When_the_user_decides_to_remove_a_file : concern_for_ImportSimulationResultsPresenter
   {
      private SimulationResultsFileSelectionDTO _addedFile;

      protected override void Context()
      {
         base.Context();
         _addedFile = _dto.AddFile("A_file");
      }

      protected override void Because()
      {
         sut.RemoveFile(_addedFile);
      }

      [Observation]
      public void should_have_removed_the_file_from_the_results_to_import()
      {
         _dto.SimulationResultsFile.Count.ShouldBeEqualTo(0);
      }
   }

   public class When_importing_some_files_and_the_import_is_successful : concern_for_ImportSimulationResultsPresenter
   {
      private readonly SimulationResultsFile _file1 = new SimulationResultsFile {FilePath = "Path1"};
      private readonly SimulationResultsFile _file2 = new SimulationResultsFile {FilePath = "Path2"};

      protected override void Context()
      {
         base.Context();
         _simulationResultsFiles.Add(_file1);
         _simulationResultsFiles.Add(_file2);
      }

      protected override void Because()
      {
         sut.StartImportProcess().Wait();
      }

      [Observation]
      public void should_enable_the_ok_button()
      {
         _view.OkEnabled.ShouldBeTrue();
      }

      [Observation]
      public void should_return_the_results_imported()
      {
         _dto.SimulationResultsFile[0].FilePath.ShouldBeEqualTo("Path1");
         _dto.SimulationResultsFile[1].FilePath.ShouldBeEqualTo("Path2");
      }
   }

   public class When_importing_some_files_and_the_import_is_not_successful : concern_for_ImportSimulationResultsPresenter
   {
      private readonly SimulationResultsFile _file1 = new SimulationResultsFile {FilePath = "Path1"};
      private readonly SimulationResultsFile _file2 = new SimulationResultsFile {FilePath = "Path2"};

      protected override void Context()
      {
         base.Context();
         _simulationResultsFiles.Add(_file1);
         _simulationResultsFiles.Add(_file2);
         A.CallTo(() => _importedResults.Status).Returns(NotificationType.Error);
      }

      protected override void Because()
      {
         sut.StartImportProcess().Wait();
      }

      [Observation]
      public void should_disable_the_ok_button()
      {
         _view.OkEnabled.ShouldBeFalse();
      }
   }
}