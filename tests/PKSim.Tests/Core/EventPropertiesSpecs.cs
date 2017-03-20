using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_EventProperties : ContextSpecification<EventProperties>
   {
      protected override void Context()
      {
         sut = new EventProperties();
      }
   }

   
   public class When_cloning_an_event_properties : concern_for_EventProperties
   {
      private EventProperties _clone;
      private ICloneManager _cloneManager;

      protected override void Context()
      {
         base.Context();
         _cloneManager =A.Fake<ICloneManager>();
      }

      protected override void Because()
      {
         _clone = sut.Clone(_cloneManager);
      }

      [Observation]
      public void should_return_an_event_properties_object_with_the_same_properties()
      {
         _clone.ShouldNotBeNull();   
      }
   }
}	