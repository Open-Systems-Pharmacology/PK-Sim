using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FluentNHibernate.Utils;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.ProjectConverter.v7_3
{
   public class Converter721To730 : IObjectConverter,
      IVisitor<Compound>,
      IVisitor<Simulation>,
      IVisitor<Formulation>,
      IVisitor<PKSimEvent>,
      IVisitor<IPKSimBuildingBlock>,
      IVisitor<PKSimProject>

   {
      private readonly ICompoundFactory _compoundFactory;
      private readonly ICloner _cloner;
      private readonly ICreationMetaDataFactory _creationMetaDataFactory;
      private readonly IContainerTask _containerTask;
      private readonly IFormulationRepository _formulationRepository;
      private readonly IEventGroupRepository _eventGroupRepository;
      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V7_2_1;

      private bool _converted;

      public Converter721To730(
         ICompoundFactory compoundFactory,
         ICloner cloner,
         ICreationMetaDataFactory creationMetaDataFactory,
         IContainerTask containerTask,
         IFormulationRepository formulationRepository,
         IEventGroupRepository eventGroupRepository)
      {
         _compoundFactory = compoundFactory;
         _cloner = cloner;
         _creationMetaDataFactory = creationMetaDataFactory;
         _containerTask = containerTask;
         _formulationRepository = formulationRepository;
         _eventGroupRepository = eventGroupRepository;
      }

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _converted = false;
         this.Visit(objectToConvert);
         return (ProjectVersions.V7_3_0, _converted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         //nothing to do here
         return (ProjectVersions.V7_3_0, false);
      }

      public void Visit(Compound compound)
      {
         adjustDefaultStateOfAllParametersIn(compound);
         convertCompound(compound);
      }

      public void Visit(Simulation simulation)
      {
         simulation.AllBuildingBlocks<IPKSimBuildingBlock>().Each(adjustDefaultStateOfAllParametersIn);
         simulation.Compounds.Each(convertCompound);
         adjustDefaultStateOfAllParameters(simulation.All<IParameter>());
      }

      public void Visit(PKSimProject project)
      {
         convertProject(project);
      }

      public void Visit(Formulation formulation)
      {
         adjustDefaultStateOfAllParametersIn(formulation);
         convertFormulation(formulation);
      }

      public void Visit(IPKSimBuildingBlock buildingBlock)
      {
         adjustDefaultStateOfAllParametersIn(buildingBlock);
      }

      public void Visit(PKSimEvent pkSimEvent)
      {
         adjustDefaultStateOfAllParametersIn(pkSimEvent);
         convertEvent(pkSimEvent);
      }

      private void adjustDefaultStateOfAllParametersIn(IContainer container)
      {
         adjustDefaultStateOfAllParameters(container.GetAllChildren<IParameter>());
      }

      private void adjustDefaultStateOfAllParameters(IEnumerable<IParameter> allParameters)
      {
         allParameters.Each(p =>
         {
            p.IsDefault = true;
            if (p.ValueDiffersFromDefault())
               p.IsDefault = false;
         });

         _converted = true;
      }

      private void convertProject(PKSimProject project)
      {
         //We do not know when the project was really created so we simply set the internal version to null
         project.Creation = _creationMetaDataFactory.Create();
         project.Creation.Version = string.Empty;
         project.Creation.InternalVersion = null;
         _converted = true;
      }

      private void convertFormulation(Formulation formulation)
      {
         if (formulation == null)
            return;

         var templateFormulation = _formulationRepository.FormulationBy(formulation.FormulationType);
         updateIsInputStateByNameAndValue(formulation, templateFormulation);
      }

      private void convertEvent(PKSimEvent pkSimEvent)
      {
         if (pkSimEvent == null)
            return;

         var templateEvent = _eventGroupRepository.FindByName(pkSimEvent.TemplateName);
         updateIsInputStateByNameAndValue(pkSimEvent, templateEvent);
      }

      private void updateIsInputStateByNameAndValue(IContainer containerToUpdate, IContainer templateContainer)
      {
         foreach (var templateParameter in templateContainer.AllParameters(x => x.Visible && x.Editable && x.ValueIsDefined()))
         {
            var parameter = containerToUpdate.Parameter(templateParameter.Name);
            if (parameter != null && !ValueComparer.AreValuesEqual(parameter, templateParameter))
               setAsInput(parameter);
         }
      }

      private void convertCompound(Compound compound)
      {
         if (compound == null)
            return;

         var templateCompound = _compoundFactory.Create().WithName(compound.Name);
         var solubilityTable = templateCompound.Parameter(CoreConstants.Parameters.SOLUBILITY_TABLE);

         compound.Add(_cloner.Clone(solubilityTable));

         var solubilityGroup = compound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_SOLUBILITY);
         solubilityGroup.AllAlternatives.Each(x => x.Add(_cloner.Clone(solubilityTable)));

         var templateParameterCache = _containerTask.CacheAllChildrenSatisfying<IParameter>(templateCompound, x => !x.IsDefault);
         var compoundParameterCache = _containerTask.CacheAllChildren<IParameter>(compound);

         foreach (var parameterPath in templateParameterCache.Keys)
         {
            setAsInput(compoundParameterCache[parameterPath]);
         }

         foreach (var templateAlternativeGroup in templateCompound.AllParameterAlternativeGroups())
         {
            var templateAlternative = templateAlternativeGroup.AllAlternatives.First();
            var compoundGroup = compound.ParameterAlternativeGroup(templateAlternativeGroup.Name);
            foreach (var alternative in compoundGroup.AllAlternatives)
            {
               updateIsInputStateByNameAndValue(alternative, templateAlternative);
            }
         }

         _converted = true;
      }

      private void setAsInput(IParameter parameter)
      {
         if (parameter == null)
            return;

         parameter.IsDefault = false;
      }
   }
}