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

   public class When_checking_if_a_transporter_container_as_a_polarized_membrane : concern_for_TransporterExpressionContainer
   {
      [Observation]
      public void should_return_true_for_kidney()
      {
         sut.Name = CoreConstants.Organ.Kidney;
         sut.HasPolarizedMembrane.ShouldBeTrue();
      }

      [Observation]
      public void should_return_true_for_brain()
      {
         sut.Name = CoreConstants.Organ.Brain;
         sut.HasPolarizedMembrane.ShouldBeTrue();
      }

      [Observation]
      public void should_return_true_for_pericentral()
      {
         sut.Name = CoreConstants.Compartment.Pericentral;
         sut.HasPolarizedMembrane.ShouldBeTrue();
      }

      [Observation]
      public void should_return_true_for_periportal()
      {
         sut.Name = CoreConstants.Compartment.Periportal;
         sut.HasPolarizedMembrane.ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_for_other_organs()
      {
         sut.Name = CoreConstants.Organ.Muscle;
         sut.HasPolarizedMembrane.ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_for_gi_mucosa()
      {
         sut.GroupName = CoreConstants.Groups.GI_MUCOSA;
         sut.Name = CoreConstants.Organ.Muscle;
         sut.HasPolarizedMembrane.ShouldBeTrue();
      }
   }
}	