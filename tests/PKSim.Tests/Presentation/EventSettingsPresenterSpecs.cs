using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

using PKSim.Presentation.DTO.Events;
using PKSim.Presentation.Presenters.Events;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Events;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Presentation
{
   public abstract class concern_for_EventSettingsPresenter : ContextSpecification<IEventSettingsPresenter>
   {
      protected IEventSettingsView _view;
      protected IEventGroupRepository _eventGroupRepository;
      protected IRepresentationInfoRepository _representationInfoRepository;
      protected IParametersByGroupPresenter _eventParametersPresenter;

      protected override void Context()
      {
         _view = A.Fake<IEventSettingsView>();
         _eventGroupRepository = A.Fake<IEventGroupRepository>();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         _eventParametersPresenter = A.Fake<IParametersByGroupPresenter>();
         sut = new EventSettingsPresenter(_view, _eventGroupRepository, _representationInfoRepository, _eventParametersPresenter);
      }
   }

   public class When_the_event_settings_presenter_is_editing_an_event : concern_for_EventSettingsPresenter
   {
      private PKSimEvent _event;
      private IParameter _para1;
      private IParameter _para2;
      private IEventGroupBuilder _template;
      private EventDTO _eventDTO;
      private IEnumerable<IParameter> _parameters;

      protected override void Context()
      {
         base.Context();
         _event = new PKSimEvent();
         _template = new EventGroupBuilder().WithName("toto");
         _event.TemplateName = _template.Name;
         A.CallTo(() => _eventGroupRepository.All()).Returns(new[] {_template});
         A.CallTo(() => _representationInfoRepository.InfoFor(_template)).Returns(new RepresentationInfo {Description = "blah"});
         _para1 = new PKSimParameter().WithName("Para1");
         _para2 = new PKSimParameter().WithName("Para2");
         _event.Add(_para1);
         _event.Add(_para2);
         A.CallTo(() => _view.BindTo(A<EventDTO>._)).Invokes(x => _eventDTO = x.GetArgument<EventDTO>(0));
         A.CallTo(() => _eventParametersPresenter.EditParameters(A<IEnumerable<IParameter>>._)).Invokes(x => _parameters = x.GetArgument<IEnumerable<IParameter>>(0));
      }

      protected override void Because()
      {
         sut.EditEvent(_event);
      }

      [Observation]
      public void should_edit_the_parameters_of_this_event()
      {
         _parameters.ShouldOnlyContain(_para1, _para2);
      }

      [Observation]
      public void should_set_the_selected_template_into_the_view()
      {
         _eventDTO.Template.ShouldBeEqualTo(_template);
         _eventDTO.Description.ShouldBeEqualTo("blah");
      }
   }

   public class When_retrieving_the_list_of_all_available_event_templates : concern_for_EventSettingsPresenter
   {
      private IList<IEventGroupBuilder> _allTemplates;
      private IEventGroupBuilder _temp1;
      private IEventGroupBuilder _temp2;

      protected override void Context()
      {
         base.Context();
         _temp1 = new EventGroupBuilder();
         _temp2 = new EventGroupBuilder();
         _allTemplates = new List<IEventGroupBuilder> {_temp1, _temp2};
         A.CallTo(() => _eventGroupRepository.All()).Returns(_allTemplates);
         A.CallTo(() => _eventGroupRepository.AllForCreationByUser()).Returns(_allTemplates);
      }

      [Observation]
      public void should_return_the_list_of_templates_define_in_the_database()
      {
         sut.AllTemplates().ShouldOnlyContain(_temp1, _temp2);
      }
   }

   public class When_retrieving_the_display_name_for_a_template : concern_for_EventSettingsPresenter
   {
      private IEventGroupBuilder _template;

      protected override void Context()
      {
         base.Context();
         _template = new EventGroupBuilder();
         A.CallTo(() => _representationInfoRepository.DisplayNameFor(_template)).Returns("blah");
      }

      [Observation]
      public void should_return_the_display_name_defined_for_the_template()
      {
         sut.DisplayNameFor(_template).ShouldBeEqualTo("blah");
      }
   }

   public class When_the_event_settings_presenter_is_told_to_that_template_cannot_be_edited : concern_for_EventSettingsPresenter
   {
      protected override void Because()
      {
         sut.CanEditEventTemplate = false;
      }

      [Observation]
      public void should_set_the_visibility_of_the_selection_in_the_view_to_false()
      {
         _view.EventTemplateVisible.ShouldBeFalse();
      }
   }

   public class When_the_event_settings_presenter_is_told_to_that_template_can_be_edited : concern_for_EventSettingsPresenter
   {
      protected override void Because()
      {
         sut.CanEditEventTemplate = true;
      }

      [Observation]
      public void should_set_the_visibility_of_the_selection_in_the_view_to_true()
      {
         _view.EventTemplateVisible.ShouldBeTrue();
      }
   }

   public class When_the_event_template_selection_has_changed : concern_for_EventSettingsPresenter
   {
      private bool _eventRaised;
      private PKSimEvent _event;
      private IEventGroupBuilder _template;
      private IEventGroupBuilder _newTemplate;

      protected override void Context()
      {
         base.Context();
         _event = new PKSimEvent();
         _template = new EventGroupBuilder().WithName("toto");
         _event.TemplateName = _template.Name;
         A.CallTo(() => _eventGroupRepository.All()).Returns(new[] {_template});
         A.CallTo(() => _representationInfoRepository.InfoFor(_template)).Returns(new RepresentationInfo {Description = "blah"});
         sut.TemplateChanged += (o, e) =>
            {
               _eventRaised = true;
               _newTemplate = e.Template;
            };
         sut.EditEvent(_event);
      }

      protected override void Because()
      {
         sut.OnTemplateChanged();
      }

      [Observation]
      public void should_notify_the_template_selection_changed_with_the_accurate_event()
      {
         _eventRaised.ShouldBeTrue();
         _newTemplate.ShouldBeEqualTo(_template);
      }
   }
}