using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_SetProtocolDosingIntervalCommand : ContextSpecification<SetProtocolDosingIntervalCommand>
   {
      protected IExecutionContext _context;
      protected SimpleProtocol _protocol;
      protected DosingInterval _dosingInterval;

      protected override void Context()
      {
         _dosingInterval = DosingIntervals.DI_24;
         _context = A.Fake<IExecutionContext>();
         _protocol = new SimpleProtocol {DosingInterval = DosingIntervals.DI_8_8_8};
         sut = new SetProtocolDosingIntervalCommand(_protocol, _dosingInterval, _context);
      }
   }

   public class When_executing_the_set_protocol_dosing_interval_command : concern_for_SetProtocolDosingIntervalCommand
   {
      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_set_the_new_dosing_interval_into_the_protocol()
      {
         _protocol.DosingInterval.ShouldBeEqualTo(_dosingInterval);
      }
   }

   public class The_inverse_of_the_set_protocol_dosing_interval_command : concern_for_SetProtocolDosingIntervalCommand
   {
      private IReversibleCommand<IExecutionContext> _result;

      protected override void Because()
      {
         _result = sut.InverseCommand(_context);
      }

      [Observation]
      public void should_be_a_set_protocol_dosing_interval_command()
      {
         _result.ShouldBeAnInstanceOf<SetProtocolDosingIntervalCommand>();
      }

      [Observation]
      public void should_have_beeen_marked_as_inverse_for_the_add_command()
      {
         _result.IsInverseFor(sut).ShouldBeTrue();
      }
   }
}