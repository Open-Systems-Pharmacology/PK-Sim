using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_swap_protocol_command : ContextSpecification<SwapProtocolCommand>
   {
      protected IExecutionContext _context;
      protected Protocol _oldProtocol;
      protected Protocol _newProtocol;
      protected PKSimProject _project;

      protected override void Context()
      {
         _context = A.Fake<IExecutionContext>();
         _oldProtocol = A.Fake<Protocol>();
         _newProtocol = A.Fake<Protocol>();
         _project = A.Fake<PKSimProject>();
         A.CallTo(() => _context.CurrentProject).Returns(_project);
         sut = new SwapProtocolCommand(_oldProtocol, _newProtocol, _context);
      }
   }

   public class When_executing_the_swap_protocol_command : concern_for_swap_protocol_command
   {
      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_remove_the_old_protocol_from_the_project()
      {
         A.CallTo(() => _project.RemoveBuildingBlock(_oldProtocol)).MustHaveHappened();
      }

      [Observation]
      public void should_added_the_new_protocol_to_the_project()
      {
         A.CallTo(() => _project.AddBuildingBlock(_newProtocol)).MustHaveHappened();
      }
   }

   public class The_inverse_of_the_swap_protocol_command : concern_for_swap_protocol_command
   {
      private IReversibleCommand<IExecutionContext> _result;

      protected override void Because()
      {
         _result = sut.InverseCommand(_context);
      }

      [Observation]
      public void should_be_a_swap_protocol_command()
      {
         _result.ShouldBeAnInstanceOf<SwapProtocolCommand>();
      }

      [Observation]
      public void should_have_beeen_marked_as_inverse_for_the_swap_protocol_command()
      {
         _result.IsInverseFor(sut).ShouldBeTrue();
      }
   }
}