using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_TransporterExpressionContainer : ContextSpecification<TransporterExpressionContainer>
   {
      protected override void Context()
      {
         sut = new TransporterExpressionContainer();
      }
   }

   public class When_updating_from_a_transporter_template : concern_for_TransporterExpressionContainer
   {
      protected override void Because()
      {
         sut.UpdatePropertiesFrom(new TransporterContainerTemplate {TransportDirection = TransportDirections.Excretion});
      }

      [Observation]
      public void should_update_the_transporter_direction()
      {
         sut.TransportDirection.ShouldBeEqualTo(TransportDirections.Excretion);
      }
   }
}