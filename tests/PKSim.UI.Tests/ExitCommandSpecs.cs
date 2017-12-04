using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Services;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using PKSim.UI.UICommands;

namespace PKSim.UI.Tests
{
   public abstract class concern_for_ExitCommand : ContextSpecification<IExitCommand>
   {
      protected IProjectTask _projectTask;
      protected IUserSettingsPersistor _userSettingsPersitor;
      protected IApplicationSettingsPersistor _applicationSettingsPersistor;

      protected override void Context()
      {
         _projectTask = A.Fake<IProjectTask>();
         _userSettingsPersitor = A.Fake<IUserSettingsPersistor>();
         _applicationSettingsPersistor = A.Fake<IApplicationSettingsPersistor>();
         sut = new ExitCommand(_projectTask, _userSettingsPersitor, _applicationSettingsPersistor);
      }

      protected override void Because()
      {
         sut.Execute();
      }
   }

   public class When_the_exit_command_is_told_to_execute_and_the_user_confirms_the_action : concern_for_ExitCommand
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _projectTask.CloseCurrentProject()).Returns(true);
      }

      [Observation]
      public void should_leverage_the_project_task_to_close_the_project()
      {
         A.CallTo(() => _projectTask.CloseCurrentProject()).MustHaveHappened();
      }

      [Observation]
      public void should_save_the_user_settings()
      {
         A.CallTo(() => _userSettingsPersitor.SaveCurrent()).MustHaveHappened();
      }

      [Observation]
      public void should_save_the_application_settings()
      {
         A.CallTo(() => _applicationSettingsPersistor.SaveCurrent()).MustHaveHappened();
      }
   }

   public class When_the_exit_command_is_told_to_execute_and_the_user_decides_to_cancel_the_action : concern_for_ExitCommand
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _projectTask.CloseCurrentProject()).Returns(false);
      }

      [Observation]
      public void should_not_close_the_application()
      {
         sut.Canceled.ShouldBeTrue();
      }

      [Observation]
      public void should_not_save_the_user_settings()
      {
         A.CallTo(() => _userSettingsPersitor.SaveCurrent()).MustNotHaveHappened();
      }
   }
}