using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_SetProtocolCommand : ContextSpecification<SetProtocolModeCommand>
   {
      protected IExecutionContext _context;
      protected IProtocolFactory _protocolFactory;
      protected IProtocolUpdater _protocolUpdater;
      protected Protocol _oldProtocol;
      protected ProtocolMode _oldProtocolMode;
      protected ProtocolMode _newProtocolMode;
      protected PKSimProject _project;
      protected Protocol _newProtocol;

      protected override void Context()
      {
         _context = A.Fake<IExecutionContext>();
         _protocolUpdater = A.Fake<IProtocolUpdater>();
         _protocolFactory = A.Fake<IProtocolFactory>();
         _project = A.Fake<PKSimProject>();
         _oldProtocol = A.Fake<Protocol>();
         _oldProtocolMode = ProtocolMode.Simple;
         _newProtocolMode = ProtocolMode.Advanced;

         A.CallTo(() => _context.CurrentProject).Returns(_project);
         A.CallTo(() => _context.Resolve<IProtocolUpdater>()).Returns(_protocolUpdater);
         A.CallTo(() => _context.Resolve<IProtocolFactory>()).Returns(_protocolFactory);

         _newProtocol = A.Fake<Protocol>();
         A.CallTo(() => _protocolFactory.Create(_newProtocolMode)).Returns(_newProtocol);
         sut = new SetProtocolModeCommand(_oldProtocol, _oldProtocolMode, _newProtocolMode, _context);
      }
   }

   public class When_executing_the_set_protocol_command : concern_for_SetProtocolCommand
   {
      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_update_the_protocol_settings_from_the_old_protocol_to_the_new_protocol()
      {
         A.CallTo(() => _protocolUpdater.UpdateProtocol(_oldProtocol, _newProtocol)).MustHaveHappened();
      }

      [Observation]
      public void should_change_the_protocol_mode_of_the_given_protocol()
      {
         A.CallTo(() => _project.AddBuildingBlock(_newProtocol)).MustHaveHappened();
         A.CallTo(() => _project.RemoveBuildingBlock(_oldProtocol)).MustHaveHappened();
      }
   }

   public class The_inverse_of_the_set_protocol_command : concern_for_SetProtocolCommand
   {
      private IReversibleCommand<IExecutionContext> _result;

      protected override void Because()
      {
         _result = sut.InverseCommand(_context);
      }

      [Observation]
      public void should_be_a_macro_command_command()
      {
         _result.ShouldBeAnInstanceOf<IMacroCommand>();
      }

      [Observation]
      public void should_have_beeen_marked_as_inverse_for_the_add_command()
      {
         _result.IsInverseFor(sut).ShouldBeTrue();
      }
   }
}