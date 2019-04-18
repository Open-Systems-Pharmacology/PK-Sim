using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;
using OSPSuite.Utility.Events;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Protocols;

namespace PKSim.Presentation.Presenters.Protocols
{
   public interface IProtocolItemPresenter : ISubPresenter, IListener, IEditParameterPresenter
   {
      IEnumerable<ApplicationType> AllApplications();
      IEnumerable<string> AllFormulationKeys();
      void EditProtocol(Protocol protocol);
      event Action StatusChanging;
      IEnumerable<string> AllOrgans();
      IEnumerable<string> AllCompartmentsFor(string organName);
      string DisplayFor(string containerName);
   }

   public abstract class ProtocolItemPresenter<TView, TPresenter> : AbstractSubPresenter<TView, TPresenter>, IProtocolItemPresenter
      where TView : IView<TPresenter>, IProtocolView
      where TPresenter : IProtocolItemPresenter
   {
      protected readonly IProtocolTask _protocolTask;
      private readonly IParameterTask _parameterTask;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      public event Action StatusChanging = delegate { };
      private readonly Individual _defaultIndividual;

      protected ProtocolItemPresenter(
         TView view, 
         IProtocolTask protocolTask, 
         IParameterTask parameterTask, 
         IIndividualFactory individualFactory,
         IRepresentationInfoRepository representationInfoRepository)
         : base(view)
      {
         _protocolTask = protocolTask;
         _parameterTask = parameterTask;
         _representationInfoRepository = representationInfoRepository;
         _defaultIndividual = individualFactory.CreateParameterLessIndividual();
      }

      public IEnumerable<ApplicationType> AllApplications()
      {
         return ApplicationTypes.All();
      }

      public IEnumerable<string> AllFormulationKeys()
      {
         return _protocolTask.AllFormulationKey();
      }

      public void SetParameterValue(IParameterDTO parameterDTO, double newValue)
      {
         AddCommand(_parameterTask.SetParameterDisplayValue(ParameterFrom(parameterDTO), newValue));
      }

      public void SetParameterUnit(IParameterDTO parameterDTO, Unit newUnit)
      {
         AddCommand(_parameterTask.SetParameterUnit(ParameterFrom(parameterDTO), newUnit));
      }

      public void SetParameterPercentile(IParameterDTO parameterDTO, double percentileInPercent)
      {
         AddCommand(_parameterTask.SetParameterPercentile(ParameterFrom(parameterDTO), percentileInPercent));
      }

      public void SetParameterValueOrigin(IParameterDTO parameterDTO, ValueOrigin valueOrigin)
      {
         AddCommand(_parameterTask.SetParameterValueOrigin(ParameterFrom(parameterDTO), valueOrigin));
      }

      public abstract void EditProtocol(Protocol protocol);

      protected IParameter ParameterFrom(IParameterDTO parameterDTO)
      {
         return parameterDTO.Parameter;
      }

      public string DisplayFor(string containerName)
      {
         return _representationInfoRepository.DisplayNameFor(RepresentationObjectType.CONTAINER, containerName);
      }
      
      protected void SetTargetOrgan(string organName, ISchemaItem schemaItem)
      {
         var allCompartments = AllCompartmentsFor(organName).ToList();
         var targetCompartment = schemaItem.TargetCompartment;
         if (!allCompartments.Contains(schemaItem.TargetCompartment))
            targetCompartment = allCompartments.First();

         AddCommand(_protocolTask.SetTargetOrgan(schemaItem, organName, targetCompartment));
      }

      protected void SetTargetCompartment(string compartmentName, ISchemaItem schemaItem)
      {
         AddCommand(_protocolTask.SetTargetCompartment(schemaItem, compartmentName));
      }

      protected void SetApplicationType(ApplicationType applicationType, ISchemaItem schemaItem)
      {
         AddCommand(_protocolTask.SetApplicationType(schemaItem, applicationType));
         if (applicationType.UserDefined)
         {
            schemaItem.TargetCompartment = CoreConstants.Compartment.Plasma;
            schemaItem.TargetOrgan = CoreConstants.Organ.ArterialBlood;
         }
         else
         {
            schemaItem.TargetCompartment = string.Empty;
            schemaItem.TargetOrgan = string.Empty;
         }
      }

      public IEnumerable<string> AllOrgans()
      {
         var organism = _defaultIndividual.Organism;

         return organism.TissueContainers.Union(organism.OrgansByType(OrganType.VascularSystem | OrganType.Lumen))
            .Select(x => x.Name)
            .OrderBy(x => x);
      }

      public IEnumerable<string> AllCompartmentsFor(string organName)
      {
         if (string.IsNullOrEmpty(organName))
            return Enumerable.Empty<string>();

         var organ = _defaultIndividual.Organism.Organ(organName);
         IEnumerable<IContainer> possibleCompartments;
         if (organ != null)
            possibleCompartments = organ.Compartments.Where(c => c.Visible);
         else
         {
            var liver = _defaultIndividual.Organism.Organ(CoreConstants.Organ.Liver);
            var zone = liver.Container(organName);
            possibleCompartments = zone.GetChildren<IContainer>();
         }

         return possibleCompartments.Select(c => c.Name).OrderBy(x => x);
      }

      public override void ViewChanged()
      {
         //We use the changing event in that case since the StatusChanged Event triggers a refresh of the plot
         StatusChanging();
      }
   }
}