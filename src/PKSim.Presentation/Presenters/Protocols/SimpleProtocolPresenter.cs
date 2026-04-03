using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Protocols;

namespace PKSim.Presentation.Presenters.Protocols
{
   public interface ISimpleProtocolPresenter : IPresenter<ISimpleProtocolView>, IProtocolItemPresenter
   {
      IEnumerable<DosingInterval> AllDosingIntervals();
      void SetApplicationType(ApplicationType applicationType);
      void SetDosingInterval(DosingInterval dosingInterval);
      void SetTargetOrgan(string organName);
      void SetTargetCompartment(string compartmentName);
      void SetEvent(bool hasEvent);
      void SetSimpleProtocolEventKey(string eventKey);
   }

   public class SimpleProtocolPresenter : ProtocolItemPresenter<ISimpleProtocolView, ISimpleProtocolPresenter>, ISimpleProtocolPresenter
   {
      private readonly ISimpleProtocolToSimpleProtocolDTOMapper _simpleProtocolDTOMapper;
      private readonly IMultiParameterEditPresenter _dynamicParameterPresenter;
      private SimpleProtocol _protocol;

      public SimpleProtocolPresenter(ISimpleProtocolView view,
         IMultiParameterEditPresenter dynamicParameterPresenter,
         ISimpleProtocolToSimpleProtocolDTOMapper simpleProtocolDTOMapper,
         IProtocolTask protocolTask, IParameterTask parameterTask,
         IIndividualFactory individualFactory, IRepresentationInfoRepository representationInfoRepository)
         : base(view, protocolTask, parameterTask, individualFactory, representationInfoRepository)
      {
         _simpleProtocolDTOMapper = simpleProtocolDTOMapper;
         _dynamicParameterPresenter = dynamicParameterPresenter;
         _dynamicParameterPresenter.IsSimpleEditor = true;
         _dynamicParameterPresenter.ValueOriginVisible = false;
         _dynamicParameterPresenter.HeaderVisible = false;
         _view.AddDynamicParameterView(_dynamicParameterPresenter.View);
      }

      public override void EditProtocol(Protocol protocol)
      {
         _protocol = protocol.DowncastTo<SimpleProtocol>();
         _view.BindTo(_simpleProtocolDTOMapper.MapFrom(_protocol));
         bindToDynamicParameters();
      }

      public override void InitializeWith(ICommandCollector commandCollector)
      {
         base.InitializeWith(commandCollector);
         _dynamicParameterPresenter.InitializeWith(commandCollector);
      }

      public override void ReleaseFrom(IEventPublisher eventPublisher)
      {
         base.ReleaseFrom(eventPublisher);
         _dynamicParameterPresenter.ReleaseFrom(eventPublisher);
      }

      public override IEnumerable<ApplicationType> AllApplications() =>
         ApplicationTypes.All().Where(x => x != ApplicationTypes.Event);

      public IEnumerable<DosingInterval> AllDosingIntervals() => DosingIntervals.All();

      public void SetEvent(bool hasEvent)
      {
         if (hasEvent == _protocol.HasEvent) return;

         var eventKey = hasEvent ? CoreConstants.DEFAULT_EVENT_KEY : string.Empty;
         AddCommand(_protocolTask.SetEventKey(_protocol, eventKey));
         _view.BindTo(_simpleProtocolDTOMapper.MapFrom(_protocol));
         bindToDynamicParameters();
      }

      public void SetSimpleProtocolEventKey(string eventKey)
      {
         AddCommand(_protocolTask.SetEventKey(_protocol, eventKey));
      }

      public void SetApplicationType(ApplicationType applicationType)
      {
         SetApplicationType(applicationType, _protocol);
         if (applicationType.UserDefined)
         {
            _view.RefreshCompartmentList();
         }

         bindToDynamicParameters();
      }

      public void SetDosingInterval(DosingInterval dosingInterval)
      {
         AddCommand(_protocolTask.SetDosingInterval(_protocol, dosingInterval));
         updateViewLayout();
      }

      public void SetTargetOrgan(string organName)
      {
         SetTargetOrgan(organName, _protocol);
         _view.RefreshCompartmentList();
      }

      public void SetTargetCompartment(string compartmentName)
      {
         SetTargetCompartment(compartmentName, _protocol);
      }

      private void bindToDynamicParameters()
      {
         var allDynamicParameters = _protocolTask.AllDynamicParametersFor(_protocol);
         _dynamicParameterPresenter.Edit(allDynamicParameters);
         updateViewLayout();
      }

      private void updateViewLayout()
      {
         var allDynamicParameters = _protocolTask.AllDynamicParametersFor(_protocol);
         _view.EndTimeVisible = !_protocol.IsSingleDosing;
         _view.DynamicParameterVisible = allDynamicParameters.Any();
         _view.TargetDefinitionVisible = _protocol.ApplicationType.UserDefined;
         _view.EventVisible = _protocol.HasEvent;
         _view.AdjustLayout();
      }
   }
}
