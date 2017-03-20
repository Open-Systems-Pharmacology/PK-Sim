using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.IntegrationTests
{
   public class When_serializing_an_individual_transporter : ContextForSerialization<IndividualTransporter>
   {
      private IndividualTransporter _transporter;
      private IndividualTransporter _deserializedTransporter;

      protected override void Context()
      {
         base.Context();
         _transporter = new IndividualTransporter();
         _transporter.TransportType = TransportType.Efflux;
      }

      protected override void Because()
      {
         _deserializedTransporter = SerializeAndDeserialize(_transporter);
      }

      [Observation]
      public void should_have_saved_the_transport_type()
      {
         _deserializedTransporter.TransportType.ShouldBeEqualTo(_transporter.TransportType);
      }
   }
}