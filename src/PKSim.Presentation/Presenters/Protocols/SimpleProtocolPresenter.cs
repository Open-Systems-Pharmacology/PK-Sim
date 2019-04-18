using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Protocols;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Protocols
{
   public interface ISimpleProtocolPresenter : IPresenter<ISimpleProtocolView>, IProtocolItemPresenter
   {
      IEnumerable<DosingInterval> AllDosingIntervals();
      void SetApplicationType(ApplicationType applicationType);
      void SetDosingInterval(DosingInterval dosingInterval);
      IEnumerable<string> AllOrgans();
      string DisplayFor(string containerName);
      IEnumerable<string> AllCompartmentsFor(string organName);
      void SetTargetOrgan(string organName);
      void SetTargetCompartment(string compartmentName);
   }

   public class SimpleProtocolPresenter : ProtocolItemPresenter<ISimpleProtocolView, ISimpleProtocolPresenter>, ISimpleProtocolPresenter
   {
      private readonly ISimpleProtocolToSimpleProtocolDTOMapper _simpleProtocolDTOMapper;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IMultiParameterEditPresenter _dynamicParameterPresenter;
      private SimpleProtocol _protocol;
      private readonly Individual _defaultIndivdual;

      public SimpleProtocolPresenter(ISimpleProtocolView view,
         IMultiParameterEditPresenter dynamicParameterPresenter,
         ISimpleProtocolToSimpleProtocolDTOMapper simpleProtocolDTOMapper,
         IProtocolTask protocolTask, IParameterTask parameterTask,
         IIndividualFactory individualFactory, IRepresentationInfoRepository representationInfoRepository)
         : base(view, protocolTask, parameterTask)
      {
         _simpleProtocolDTOMapper = simpleProtocolDTOMapper;
         _representationInfoRepository = representationInfoRepository;
         _dynamicParameterPresenter = dynamicParameterPresenter;
         _dynamicParameterPresenter.IsSimpleEditor = true;
         _dynamicParameterPresenter.ValueOriginVisible = false;
         _dynamicParameterPresenter.HeaderVisible = false;
         _view.AddDynamicParameterView(_dynamicParameterPresenter.View);
         _defaultIndivdual = individualFactory.CreateParameterLessIndividual();
      }

      public override IEnumerable<ApplicationType> AllApplications()
      {
         return ApplicationTypes.All();
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

      public IEnumerable<DosingInterval> AllDosingIntervals()
      {
         return DosingIntervals.All();
      }

      public void SetApplicationType(ApplicationType applicationType)
      {
         AddCommand(_protocolTask.SetApplicationType(_protocol, applicationType));
         if (applicationType.UserDefined)
         {
            _protocol.TargetCompartment = CoreConstants.Compartment.Plasma;
            _protocol.TargetOrgan = CoreConstants.Organ.ArterialBlood;
            _view.RefreshCompartmentList();
         }
         else
         {
            _protocol.TargetCompartment = string.Empty;
            _protocol.TargetOrgan = string.Empty;
         }
         bindToDynamicParameters();
      }

      public void SetDosingInterval(DosingInterval dosingInterval)
      {
         AddCommand(_protocolTask.SetDosingInterval(_protocol, dosingInterval));
         updateViewLayout();
      }

      public string DisplayFor(string containerName)
      {
         return _representationInfoRepository.DisplayNameFor(RepresentationObjectType.CONTAINER, containerName);
      }

      public IEnumerable<string> AllOrgans() 
      {
         var organism = _defaultIndivdual.Organism;

         return organism.TissueContainers.Union(organism.OrgansByType(OrganType.VascularSystem | OrganType.Lumen))
            .Select(x => x.Name)
            .OrderBy(x => x);
      }

      public IEnumerable<string> AllCompartmentsFor(string organName)
      {
         if (string.IsNullOrEmpty(organName))
            return Enumerable.Empty<string>();

         var organ = _defaultIndivdual.Organism.Organ(organName);
         IEnumerable<IContainer> possibleCompartments;
         if (organ != null)
            possibleCompartments = organ.Compartments.Where(c => c.Visible);
         else
         {
            var liver = _defaultIndivdual.Organism.Organ(CoreConstants.Organ.Liver);
            var zone = liver.Container(organName);
            possibleCompartments = zone.GetChildren<IContainer>();

         }
         return possibleCompartments.Select(c => c.Name).OrderBy(x => x);
      }

      public void SetTargetOrgan(string organName)
      {
         var allCompartments = AllCompartmentsFor(organName).ToList();
         var targetCompartment = _protocol.TargetCompartment;
         if (!allCompartments.Contains(_protocol.TargetCompartment))
            targetCompartment = allCompartments.First();

         AddCommand(_protocolTask.SetTargetOrgan(_protocol, organName, targetCompartment));
         _view.RefreshCompartmentList();
      }

      public void SetTargetCompartment(string compartmentName)
      {
         AddCommand(_protocolTask.SetTargetCompartment(_protocol, compartmentName));
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
      }
   }
}