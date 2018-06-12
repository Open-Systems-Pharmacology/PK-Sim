using System;
using System.Collections.Generic;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Events;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Events;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Events;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Events
{
   public class TemplateChangedEventArgs : EventArgs
   {
      public IEventGroupBuilder Template { get; private set; }

      public TemplateChangedEventArgs(IEventGroupBuilder template)
      {
         Template = template;
      }
   }

   public interface IEventSettingsPresenter : IEventItemPresenter
   {
      IEnumerable<IEventGroupBuilder> AllTemplates();
      string DisplayNameFor(IEventGroupBuilder eventTemplate);
      void OnTemplateChanged();
      event EventHandler<TemplateChangedEventArgs> TemplateChanged;
      bool CanEditEventTemplate { set; }
   }

   public class EventSettingsPresenter : AbstractSubPresenter<IEventSettingsView, IEventSettingsPresenter>, IEventSettingsPresenter
   {
      private readonly IEventGroupRepository _eventGroupRepository;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IParametersByGroupPresenter _eventParametersPresenter;
      private EventDTO _eventDTO;
      public event EventHandler<TemplateChangedEventArgs> TemplateChanged = delegate { };

      public bool CanEditEventTemplate
      {
         set => _view.EventTemplateVisible = value;
      }

      public EventSettingsPresenter(IEventSettingsView view, IEventGroupRepository eventGroupRepository,
         IRepresentationInfoRepository representationInfoRepository, IParametersByGroupPresenter eventParametersPresenter)
         : base(view)
      {
         _eventGroupRepository = eventGroupRepository;
         _representationInfoRepository = representationInfoRepository;
         _eventParametersPresenter = eventParametersPresenter;
         _eventParametersPresenter.HeaderVisible = true;
         _view.AddParameterView(_eventParametersPresenter.View);
      }

      public void EditEvent(PKSimEvent pkSimEvent)
      {
         var template = _eventGroupRepository.FindByName(pkSimEvent.TemplateName);
         _eventDTO = new EventDTO {Description = _representationInfoRepository.InfoFor(template).Description, Template = template};

         _eventParametersPresenter.EditParameters(pkSimEvent.AllParameters(p => !p.IsNamed(Constants.Parameters.START_TIME)));
         _view.BindTo(_eventDTO);
      }

      public override void InitializeWith(ICommandCollector commandCollector)
      {
         base.InitializeWith(commandCollector);
         _eventParametersPresenter.InitializeWith(commandCollector);
      }

      public override void ReleaseFrom(IEventPublisher eventPublisher)
      {
         base.ReleaseFrom(eventPublisher);
         _eventParametersPresenter.ReleaseFrom(eventPublisher);
      }

      public IEnumerable<IEventGroupBuilder> AllTemplates()
      {
         return _eventGroupRepository.AllForCreationByUser();
      }

      public string DisplayNameFor(IEventGroupBuilder eventTemplate)
      {
         return _representationInfoRepository.DisplayNameFor(eventTemplate);
      }

      public void OnTemplateChanged()
      {
         TemplateChanged(this, new TemplateChangedEventArgs(_eventDTO.Template));
      }
   }
}