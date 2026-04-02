using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
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
      IEnumerable<PKSimEvent> AllEvents();
      void SetEvent(PKSimEvent pkSimEvent);
   }

   public class SimpleProtocolPresenter : ProtocolItemPresenter<ISimpleProtocolView, ISimpleProtocolPresenter>, ISimpleProtocolPresenter
   {
      private readonly ISimpleProtocolToSimpleProtocolDTOMapper _simpleProtocolDTOMapper;
      private readonly IMultiParameterEditPresenter _dynamicParameterPresenter;
      private readonly IBuildingBlockRepository _buildingBlockRepository;
      private SimpleProtocol _protocol;

      public SimpleProtocolPresenter(ISimpleProtocolView view,
         IMultiParameterEditPresenter dynamicParameterPresenter,
         ISimpleProtocolToSimpleProtocolDTOMapper simpleProtocolDTOMapper,
         IProtocolTask protocolTask, IParameterTask parameterTask,
         IIndividualFactory individualFactory, IRepresentationInfoRepository representationInfoRepository,
         IBuildingBlockRepository buildingBlockRepository)
         : base(view, protocolTask, parameterTask, individualFactory, representationInfoRepository)
      {
         _simpleProtocolDTOMapper = simpleProtocolDTOMapper;
         _dynamicParameterPresenter = dynamicParameterPresenter;
         _buildingBlockRepository = buildingBlockRepository;
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

      public override IEnumerable<ApplicationType> AllApplications()
      {
         return ApplicationTypes.All().Where(x => x != ApplicationTypes.Event);
      }

      public IEnumerable<DosingInterval> AllDosingIntervals()
      {
         return DosingIntervals.All();
      }

      public IEnumerable<PKSimEvent> AllEvents()
      {
         return _buildingBlockRepository.All<PKSimEvent>();
      }

      public void SetEvent(PKSimEvent pkSimEvent)
      {
         var templateEventId = pkSimEvent?.Id;
         AddCommand(_protocolTask.SetSimpleProtocolEvent(_protocol, templateEventId));
         updateViewLayout();
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
      }
   }
}
