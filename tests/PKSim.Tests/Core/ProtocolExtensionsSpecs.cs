using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_ProtocolExtensions : StaticContextSpecification
   {
      protected List<Protocol> _protocols;

      protected override void Context()
      {
         _protocols = new List<Protocol>();
      }
   }

   public class When_resolving_the_longest_protocol_in_an_enumeration_of_protocols : concern_for_ProtocolExtensions
   {
      private Protocol _p1;
      private Protocol _p2;

      protected override void Context()
      {
         base.Context();
         _p1 = A.Fake<Protocol>().WithId("p1");
         _p2 = A.Fake<Protocol>().WithId("p2");
         A.CallTo(() => _p1.EndTime).Returns(10);
         A.CallTo(() => _p2.EndTime).Returns(20);
      }

      [Observation]
      public void should_return_null_if_the_enumeration_is_empty()
      {
         _protocols.LongestProtocol().ShouldBeNull();
      }

      [Observation]
      public void should_return_the_protocol_with_the_longest_end_time_otherwise()
      {
         _protocols.AddRange(new[]{_p1,_p2});
         _protocols.LongestProtocol().ShouldBeEqualTo(_p2);

      }
   }
}	