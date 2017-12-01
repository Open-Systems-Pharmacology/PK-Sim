using System;
using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Core;

using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_ImportPopulationCommand : ContextSpecification<ImportPopulationCommand>
   {
      protected IBuildingBlockTask _buildingBlockTask;
      protected IImportPopulationPresenter _importPopulationPresenter;
      private IApplicationController _applicationController;

      protected override void Context()
      {
         _buildingBlockTask = A.Fake<IBuildingBlockTask>();
         _importPopulationPresenter = A.Fake<IImportPopulationPresenter>();
         _applicationController= A.Fake<IApplicationController>();
         A.CallTo(() => _applicationController.Start<IImportPopulationPresenter>()).Returns(_importPopulationPresenter);
         sut = new ImportPopulationCommand(_applicationController, _buildingBlockTask);
      }

      protected override void Because()
      {
         sut.Execute();
      }
   }

   public class When_the_import_population_command_is_being_executed : concern_for_ImportPopulationCommand
   {
      private ImportPopulation _population;

      protected override void Context()
      {
         base.Context();
         _population = A.Fake<ImportPopulation>();
         A.CallTo(() => _importPopulationPresenter.BuildingBlock).Returns(_population);
      }

      [Observation]
      public void it_should_start_the_import_population_from_file_workflow()
      {
         A.CallTo(() => _importPopulationPresenter.Create()).MustHaveHappened();
      }

      [Observation]
      public void it_should_add_the_created_population_to_the_project()
      {
         A.CallTo(() => _buildingBlockTask.AddToProject(_population, true, true)).MustHaveHappened();
      }
   }

   public class When_the_user_cancels_the_action_of_importing_a_population : concern_for_ImportPopulationCommand
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _importPopulationPresenter.Create()).Returns(new PKSimEmptyCommand());
      }

      [Observation]
      public void it_should_not_add_the_population_to_the_history()
      {
         A.CallTo(() => _buildingBlockTask.AddToProject(A<Population>._, true, true)).MustNotHaveHappened();
      }
   }
}