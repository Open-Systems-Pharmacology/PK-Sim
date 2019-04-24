using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;

using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_ImportPopulationSimulationPresenter : ContextSpecification<IImportPopulationSimulationPresenter>
   {
      protected IImportPopulationSimulationView _view;
      private IDialogCreator _dialogCreator;
      protected IImportSimulationTask _importSimulationTask;
      protected ImportPopulationSimulationDTO _dto;

      protected override void Context()
      {
         _view = A.Fake<IImportPopulationSimulationView>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _importSimulationTask = A.Fake<IImportSimulationTask>();
         A.CallTo(() => _view.BindTo(A<ImportPopulationSimulationDTO>._))
            .Invokes(x => _dto = x.GetArgument<ImportPopulationSimulationDTO>(0));

         sut = new ImportPopulationSimulationPresenter(_view, _dialogCreator, _importSimulationTask);
      }
   }

   public class When_importing_a_population_simulation_using_an_existing_pkml_file : concern_for_ImportPopulationSimulationPresenter
   {
      private string _existingFile;
      private Func<string, bool> _oldFileExists;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _oldFileExists = FileHelper.FileExists;
         FileHelper.FileExists = x => true;
      }

      protected override void Context()
      {
         base.Context();
         _existingFile = "blabla";
      }

      protected override void Because()
      {
         sut.CreateImportPopulationSimulation(_existingFile);
      }

      [Observation]
      public void should_hide_the_simulation_selection()
      {
         _view.SimulationSelectionVisible.ShouldBeFalse();
      }

      [Observation]
      public void should_have_set_the_file_into_the_view()
      {
         _dto.FilePath.ShouldBeEqualTo(_existingFile);
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         FileHelper.FileExists = _oldFileExists;
      }
   }

   public class When_importing_a_population_simulation_using_a_pkml_file_that_does_not_exist : concern_for_ImportPopulationSimulationPresenter
   {
      private string _simulationFileThatDoesNotExist;
      private Func<string, bool> _oldFileExists;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _oldFileExists = FileHelper.FileExists;
         FileHelper.FileExists = x => false;
      }

      protected override void Context()
      {
         base.Context();
         _simulationFileThatDoesNotExist = "blabla";
      }

      protected override void Because()
      {
         sut.CreateImportPopulationSimulation(_simulationFileThatDoesNotExist);
      }

      [Observation]
      public void should_show_the_simulation_selection()
      {
         _view.SimulationSelectionVisible.ShouldBeTrue();
      }

      [Observation]
      public void should_not_use_the_given_file_in_the_view()
      {
         _dto.FilePath.ShouldNotBeEqualTo(_simulationFileThatDoesNotExist);
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         FileHelper.FileExists = _oldFileExists;
      }
   }

   public class When_starting_the_import_using_the_building_block_mode : concern_for_ImportPopulationSimulationPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.CreateImportPopulationSimulation();
         _dto.PopulationImportMode = PopulationImportMode.BuildingBlock;
      }

      protected override void Because()
      {
         sut.StartImport();
      }

      [Observation]
      public void should_return_a_simulaiton_created_using_a_building_block()
      {
         A.CallTo(() => _importSimulationTask.ImportFromBuildingBlock(_dto.FilePath, A<Population>._)).MustHaveHappened();
      }
   }

   public class When_starting_the_import_using_the_population_import_from_csv_mode : concern_for_ImportPopulationSimulationPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.CreateImportPopulationSimulation();
         _dto.PopulationImportMode = PopulationImportMode.File;
         _dto.PopulationFile = "populationFile";
      }

      protected override void Because()
      {
         sut.StartImport();
      }

      [Observation]
      public void should_return_a_simulaiton_created_using_the_population_file()
      {
         A.CallTo(() => _importSimulationTask.ImportFromPopulationFile(_dto.FilePath, _dto.PopulationFile)).MustHaveHappened();
      }
   }

   public class When_starting_the_import_using_the_population_size_mode : concern_for_ImportPopulationSimulationPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.CreateImportPopulationSimulation();
         _dto.PopulationImportMode = PopulationImportMode.Size;
         _dto.NumberOfIndividuals = 50;
      }

      protected override void Because()
      {
         sut.StartImport();
      }

      [Observation]
      public void should_return_a_simulaiton_created_using_the_population_size()
      {
         A.CallTo(() => _importSimulationTask.ImportFromPopulationSize(_dto.FilePath, _dto.NumberOfIndividuals)).MustHaveHappened();
      }
   }
}