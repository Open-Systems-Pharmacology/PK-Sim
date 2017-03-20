using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_compartment : ContextSpecification<Compartment>
   {
      protected override void Context()
      {
         sut = new Compartment();
      }
   }

   
   public class When_a_compartment_is_asked_if_it_is_visibile : concern_for_compartment
   {
      private bool _resultVisible;
      private bool _resultHidden;

      protected override void Because()
      {
         sut.Visible = true;
         _resultVisible = sut.Visible;

         sut.Visible = false;
         _resultHidden = sut.Visible;
      }

      [Observation]
      public void should_return_true_if_the_compartment_is_visible()
      {
         _resultVisible.ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_the_compartment_is_hidden()
      {
         _resultHidden.ShouldBeFalse();
      }
   }
}