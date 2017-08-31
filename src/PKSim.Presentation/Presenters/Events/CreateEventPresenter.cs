using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Services;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Core;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.Events;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Events
{
   public interface ICreateEventPresenter : ICreateBuildingBlockPresenter<PKSimEvent>, IContainerPresenter
   {
      PKSimEvent Event { get; }
   }

   public class CreateEventPresenter : AbstractSubPresenterContainerPresenter<ICreateEventView, ICreateEventPresenter, IEventItemPresenter>, ICreateEventPresenter
   {
      private readonly IObjectBaseDTOFactory _objectBaseDTOFactory;
      private readonly IEventFactory _eventFactory;
      private readonly IBuildingBlockPropertiesMapper _propertiesMapper;
      private ObjectBaseDTO _eventPropertiesDTO;
      public PKSimEvent Event { get; private set; }

      public CreateEventPresenter(ICreateEventView view, ISubPresenterItemManager<IEventItemPresenter> subPresenterItemManager, IDialogCreator dialogCreator,
                                  IObjectBaseDTOFactory objectBaseDTOFactory, IEventFactory eventFactory, IBuildingBlockPropertiesMapper propertiesMapper)
         : base(view, subPresenterItemManager, EventItems.All, dialogCreator)
      {
         _objectBaseDTOFactory = objectBaseDTOFactory;
         _eventFactory = eventFactory;
         _propertiesMapper = propertiesMapper;
      }

      private IEventSettingsPresenter eventSettingsPresenter => _subPresenterItemManager.PresenterAt(EventItems.Settings);

      public override void InitializeWith(ICommandCollector commandCollector)
      {
         base.InitializeWith(commandCollector);
         eventSettingsPresenter.TemplateChanged += (o, e) => templateChanged(e);
      }

      private void templateChanged(TemplateChangedEventArgs e)
      {
         _macroCommand.Clear();
         Event = _eventFactory.Create(e.Template);
         eventSettingsPresenter.EditEvent(Event);
      }

      public IPKSimCommand Create()
      {
         _eventPropertiesDTO = _objectBaseDTOFactory.CreateFor<PKSimEvent>();
         _view.BindToProperties(_eventPropertiesDTO);
         Event = _eventFactory.Create();
         eventSettingsPresenter.EditEvent(Event);
         _view.Display();

         if (_view.Canceled)
            return new PKSimEmptyCommand();

         _propertiesMapper.MapProperties(_eventPropertiesDTO, Event);

         return _macroCommand;
      }

      public PKSimEvent BuildingBlock => Event;
   }
}