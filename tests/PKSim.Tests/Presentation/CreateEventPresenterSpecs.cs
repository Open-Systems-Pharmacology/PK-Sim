using OSPSuite.BDDHelper;
using OSPSuite.Core.Services;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Core;
using PKSim.Presentation.DTO.Mappers;

using PKSim.Presentation.Presenters.Events;
using PKSim.Presentation.Views.Events;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation
{
   public abstract class concern_for_CreateEventPresenter : ContextSpecification<ICreateEventPresenter>
   {
      protected ICreateEventView _view;
      protected ISubPresenterItemManager<IEventItemPresenter> _subPresenterManager;
      protected IObjectBaseDTOFactory _dtoFactory;
      protected IEventFactory _eventFactory;
      protected IBuildingBlockPropertiesMapper _propertiesMapper;
      protected IEventSettingsPresenter _eventSettings;
      private IDialogCreator _dialogCreator;

      protected override void Context()
      {
         _view = A.Fake<ICreateEventView>();
         _subPresenterManager = SubPresenterHelper.Create<IEventItemPresenter>();
         _dtoFactory = A.Fake<IObjectBaseDTOFactory>();
         _eventFactory = A.Fake<IEventFactory>();
         _propertiesMapper = A.Fake<IBuildingBlockPropertiesMapper>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _eventSettings = _subPresenterManager.CreateFake(EventItems.Settings);
         sut = new CreateEventPresenter(_view, _subPresenterManager, _dialogCreator, _dtoFactory, _eventFactory, _propertiesMapper);
         sut.Initialize();
      }
   }

   public class When_the_create_event_presenter_is_creating_an_event : concern_for_CreateEventPresenter
   {
      private PKSimEvent _event;
      private ObjectBaseDTO _eventDTO;

      protected override void Context()
      {
         base.Context();
         _eventDTO = new ObjectBaseDTO();
         _event = A.Fake<PKSimEvent>();
         A.CallTo(() => _eventFactory.Create()).Returns(_event);
         A.CallTo(() => _dtoFactory.CreateFor<PKSimEvent>()).Returns(_eventDTO);
      }

      protected override void Because()
      {
         sut.Create();
      }

      [Observation]
      public void should_create_a_new_event()
      {
         A.CallTo(() => _eventFactory.Create()).MustHaveHappened();
      }

      [Observation]
      public void should_edit_the_settings_of_the_event()
      {
         A.CallTo(() => _eventSettings.EditEvent(_event)).MustHaveHappened();
      }
   }

   public class When_the_create_event_presenter_is_being_notified_that_the_user_changed_the_template : concern_for_CreateEventPresenter
   {
      private IEventGroupBuilder _eventTemplate;
      private PKSimEvent _event;

      protected override void Context()
      {
         base.Context();
         _event = A.Fake<PKSimEvent>();
         _eventTemplate = A.Fake<IEventGroupBuilder>();
         A.CallTo(() => _eventFactory.Create(_eventTemplate)).Returns(_event);
      }

      protected override void Because()
      {
         _eventSettings.TemplateChanged += Raise.With(new TemplateChangedEventArgs(_eventTemplate));
      }

      [Observation]
      public void should_create_a_new_event_based_on_the_selected_template()
      {
         A.CallTo(() => _eventFactory.Create(_eventTemplate)).MustHaveHappened();
      }

      [Observation]
      public void should_edit_the_new_tempalte()
      {
         A.CallTo(() => _eventSettings.EditEvent(_event)).MustHaveHappened();
      }
   }
}