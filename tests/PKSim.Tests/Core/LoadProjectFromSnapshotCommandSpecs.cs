using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.Core;
using IWorkspace = PKSim.Presentation.Core.IWorkspace;

namespace PKSim.Core
{
   public abstract class concern_for_LoadProjectFromSnapshotCommand : ContextSpecification<LoadProjectFromSnapshotCommand>
   {
      protected IWorkspace _workspace;
      protected PKSimProject _project;
      protected string _snapshotFile = "SnapshotFile";
      protected IExecutionContext _context;
      protected IApplicationConfiguration _configuration;

      protected override void Context()
      {
         _context= A.Fake<IExecutionContext>();
         _workspace = A.Fake<IWorkspace>();
         _configuration= A.Fake<IApplicationConfiguration>();
         _project = new PKSimProject {Creation = {Version = "CreationVersion"}};
         A.CallTo(() => _context.Resolve<IApplicationConfiguration>()).Returns(_configuration);
         A.CallTo(() => _configuration.Version).Returns("1.2.3");
         sut = new LoadProjectFromSnapshotCommand(_workspace,_project, _snapshotFile);
      }
   }

   public class When_executing_the_load_project_from_snapshot_command : concern_for_LoadProjectFromSnapshotCommand
   {

      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_load_the_project_into_the_workspace()
      {
         A.CallTo(() => _workspace.LoadProject(_project)).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_command_description_to_use_the_file_name_and_the_current_project_version()
      {
         sut.Description.ShouldBeEqualTo(PKSimConstants.Command.LoadProjectFromSnapshotDescription(_snapshotFile, _configuration.Version, _project.Creation.Version));
      }
   }
}	