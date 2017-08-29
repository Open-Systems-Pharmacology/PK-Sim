using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Presentation.Core;

namespace PKSim.Core
{
   public abstract class concern_for_AddProcessToCompoundCommand : ContextSpecification<AddProcessToCompoundCommand>
   {
      protected Compound _compound;
      protected IExecutionContext _executionContext;
      protected IWorkspace _workspace;
      protected PKSimProject _project;
      protected CompoundProcess _proc;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _project = new PKSimProject();
         A.CallTo(() => _executionContext.CurrentProject).Returns(_project);

         _compound = new Compound().WithId("Tralala").WithName("Tralala");
         _project.AddBuildingBlock(_compound);

         _proc = new EnzymaticProcess().WithName("ActProc1");

         var serializedStream = new byte[1];
         A.CallTo(() => _executionContext.Serialize(_proc)).Returns(serializedStream);
         A.CallTo(() => _executionContext.Deserialize<CompoundProcess>(serializedStream)).Returns(_proc);

         A.CallTo(() => _executionContext.Get<Compound>(_compound.Id)).Returns(_compound);
         A.CallTo(() => _executionContext.Get<CompoundProcess>(_proc.Id)).Returns(_proc);

         sut = new AddProcessToCompoundCommand(_proc, _compound, _executionContext);
      }
   }

   public class When_adding_a_process_to_a_compound : concern_for_AddProcessToCompoundCommand
   {
      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_have_added_the_process_to_the_available_processes_of_the_compound()
      {
         _compound.AllProcesses<EnzymaticProcess>().First().ShouldBeEqualTo(_proc);
      }
   }

   public class When_removing_a_process_from_a_compound : concern_for_AddProcessToCompoundCommand
   {
      protected IReversibleCommand<IExecutionContext> _removePartialStabiCommand;

      protected override void Context()
      {
         base.Context();
         //add enzymatic stability
         sut.Execute(_executionContext);
      }

      protected override void Because()
      {
         sut.RestoreExecutionData(_executionContext);
         _removePartialStabiCommand = sut.InverseCommand(_executionContext);
         _removePartialStabiCommand.Execute(_executionContext);
      }

      [Observation]
      public void should_remove_partial_proc()
      {
         _compound.AllProcesses<EnzymaticProcess>().Count().ShouldBeEqualTo(0);
      }
   }

   public class When_restoring_a_process_from_a_compound : concern_for_AddProcessToCompoundCommand
   {
      protected IReversibleCommand<IExecutionContext> _restorePartialStabiCommand;
      protected IReversibleCommand<IExecutionContext> _removePartialStabiCommand;

      protected override void Context()
      {
         base.Context();
         //Add
         sut.Execute(_executionContext);
         //Remove
         sut.RestoreExecutionData(_executionContext);
         _removePartialStabiCommand = sut.InverseCommand(_executionContext);
         _removePartialStabiCommand.Execute(_executionContext);
      }

      protected override void Because()
      {
         _removePartialStabiCommand.RestoreExecutionData(_executionContext);

         _restorePartialStabiCommand = _removePartialStabiCommand.InverseCommand(_executionContext);
         _restorePartialStabiCommand.Execute(_executionContext);
      }

      [Observation]
      public void should_restore_partial_act_proc()
      {
         _compound.AllProcesses<EnzymaticProcess>().First().ShouldBeEqualTo(_proc);
      }
   }
}