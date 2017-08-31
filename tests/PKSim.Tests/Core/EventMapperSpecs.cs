using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_EventMapper : ContextSpecification<EventMapper>
   {
      private ParameterMapper _parameterMapper;
      protected PKSimEvent _event;
      protected Snapshots.Event _snapshot;
      protected IParameter _parameter1;
      protected IParameter _parameter2;
      protected IParameter _hiddenParameter;

      protected override void Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();

         sut = new EventMapper(_parameterMapper);

         _parameter1 = DomainHelperForSpecs.ConstantParameterWithValue(5)
            .WithName("Param1");

         _parameter2 = DomainHelperForSpecs.ConstantParameterWithValue(5)
            .WithName("Param2");

         _hiddenParameter = DomainHelperForSpecs.ConstantParameterWithValue(5)
            .WithName("Param3");
         _hiddenParameter.Visible = false;

         _event = new PKSimEvent
         {
            TemplateName = "TemplateEventName",
            Description = "Amazing event"
         };

         _event.Add(_parameter1);
         _event.Add(_parameter2);
         _event.Add(_hiddenParameter);

         A.CallTo(() => _parameterMapper.MapToSnapshot(_parameter1)).Returns(new Snapshots.Parameter().WithName(_parameter1.Name));
         A.CallTo(() => _parameterMapper.MapToSnapshot(_parameter2)).Returns(new Snapshots.Parameter().WithName(_parameter2.Name));
         A.CallTo(() => _parameterMapper.MapToSnapshot(_hiddenParameter)).Returns(new Snapshots.Parameter().WithName(_hiddenParameter.Name));
      }
   }

   public class When_mapping_a_model_event_to_a_snapshot_event : concern_for_EventMapper
   {
      protected override void Because()
      {
         _snapshot = sut.MapToSnapshot(_event);
      }


      [Observation]
      public void should_save_the_event_properties()
      {
         _snapshot.Template.ShouldBeEqualTo(_event.TemplateName);
         _snapshot.Name.ShouldBeEqualTo(_event.Name);
         _snapshot.Description.ShouldBeEqualTo(_event.Description);
      }

      [Observation]
      public void should_save_the_visible_event_parameters_only()
      {
         _snapshot.Parameters.Count.ShouldBeEqualTo(_event.AllVisibleParameters().Count());
         _snapshot.Parameters.ExistsByName(_parameter1.Name).ShouldBeTrue();
         _snapshot.Parameters.ExistsByName(_parameter2.Name).ShouldBeTrue();
         _snapshot.Parameters.ExistsByName(_hiddenParameter.Name).ShouldBeFalse();
      }
   }
}	