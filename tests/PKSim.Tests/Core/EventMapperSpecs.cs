using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;
using Event = PKSim.Core.Snapshots.Event;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_EventMapper : ContextSpecification<EventMapper>
   {
      protected ParameterMapper _parameterMapper;
      protected PKSimEvent _event;
      protected Event _snapshot;
      protected IParameter _parameter1;
      protected IParameter _parameter2;
      protected IParameter _hiddenParameter;
      protected IEventFactory _eventFactory;

      protected override void Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _eventFactory = A.Fake<IEventFactory>();

         sut = new EventMapper(_parameterMapper, _eventFactory);

         _parameter1 = DomainHelperForSpecs.ConstantParameterWithValue(5)
            .WithName("Param1");

         _parameter2 = DomainHelperForSpecs.ConstantParameterWithValue(5)
            .WithName("Param2");

         _hiddenParameter = DomainHelperForSpecs.ConstantParameterWithValue(5)
            .WithName("Param3");
         _hiddenParameter.Visible = false;

         _event = new PKSimEvent
         {
            Name = "Event",
            TemplateName = "TemplateEventName",
            Description = "Amazing event"
         };

         _event.Add(_parameter1);
         _event.Add(_parameter2);
         _event.Add(_hiddenParameter);

         A.CallTo(() => _parameterMapper.MapToSnapshot(_parameter1)).Returns(new Parameter().WithName(_parameter1.Name));
         A.CallTo(() => _parameterMapper.MapToSnapshot(_parameter2)).Returns(new Parameter().WithName(_parameter2.Name));
         A.CallTo(() => _parameterMapper.MapToSnapshot(_hiddenParameter)).Returns(new Parameter().WithName(_hiddenParameter.Name));
      }
   }

   public class When_mapping_a_model_event_to_a_snapshot_event : concern_for_EventMapper
   {
      protected override async void Because()
      {
         _snapshot = await sut.MapToSnapshot(_event);
      }

      [Observation]
      public void should_save_the_event_properties()
      {
         _snapshot.Template.ShouldBeEqualTo(_event.TemplateName);
         _snapshot.Name.ShouldBeEqualTo(_event.Name);
         _snapshot.Description.ShouldBeEqualTo(_event.Description);
      }

      [Observation]
      public void should_save_the_event_parameters_changed_by_the_user_only()
      {
         _snapshot.Parameters.Length.ShouldBeEqualTo(_event.AllVisibleParameters().Count());
         _snapshot.Parameters.ExistsByName(_parameter1.Name).ShouldBeTrue();
         _snapshot.Parameters.ExistsByName(_parameter2.Name).ShouldBeTrue();
         _snapshot.Parameters.ExistsByName(_hiddenParameter.Name).ShouldBeFalse();
      }
   }

   public class When_mapping_a_valid_event_snapshot_to_an_event : concern_for_EventMapper
   {
      private PKSimEvent _newEvent;

      protected override async void Context()
      {
         base.Context();
         _snapshot = await sut.MapToSnapshot(_event);
         A.CallTo(() => _eventFactory.Create(_snapshot.Template)).Returns(_event);

         _snapshot.Name = "New Event";
         _snapshot.Description = "The description that will be deserialized";
      }

      protected override async void Because()
      {
         _newEvent = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_have_created_an_event_with_the_expected_properties()
      {
         _newEvent.Name.ShouldBeEqualTo(_snapshot.Name);
         _newEvent.Description.ShouldBeEqualTo(_snapshot.Description);
      }

      [Observation]
      public void should_have_updated_all_visible_parameters()
      {
         A.CallTo(() => _parameterMapper.MapParameters(_snapshot.Parameters, _newEvent, _event.TemplateName)).MustHaveHappened();
      }
   }
}