using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using OSPSuite.Core.Converter.v7_3;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
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
      IVisitor<SimpleProtocol>,
      IVisitor<AdvancedProtocol>,
      IVisitor<PKSimProject>

   {
      private readonly ICompoundFactory _compoundFactory;
      private readonly ICloner _cloner;
      private readonly ICreationMetaDataFactory _creationMetaDataFactory;
      private readonly IContainerTask _containerTask;
      private readonly IFormulationRepository _formulationRepository;
      private readonly IEventGroupRepository _eventGroupRepository;
      private readonly Converter710To730 _coreConverter;
      private readonly ICompoundProcessRepository _compoundProcessRepository;
      private readonly ISchemaItemRepository _schemaItemRepository;
      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V7_2_1;

      private bool _converted;

      public Converter721To730(
         ICompoundFactory compoundFactory,
         ICloner cloner,
         ICreationMetaDataFactory creationMetaDataFactory,
         IContainerTask containerTask,
         IFormulationRepository formulationRepository,
         IEventGroupRepository eventGroupRepository,
         Converter710To730 coreConverter,
         ICompoundProcessRepository compoundProcessRepository,
         ISchemaItemRepository schemaItemRepository
      )
      {
         _compoundFactory = compoundFactory;
         _cloner = cloner;
         _creationMetaDataFactory = creationMetaDataFactory;
         _containerTask = containerTask;
         _formulationRepository = formulationRepository;
         _eventGroupRepository = eventGroupRepository;
         _coreConverter = coreConverter;
         _compoundProcessRepository = compoundProcessRepository;
         _schemaItemRepository = schemaItemRepository;
      }

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _converted = false;
         this.Visit(objectToConvert);
         return (ProjectVersions.V7_3_0, _converted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         var (_, converted) = _coreConverter.ConvertXml(element);
         return (ProjectVersions.V7_3_0, converted);
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

      public void Visit(SimpleProtocol protocol)
      {
         adjustDefaultStateOfAllParametersIn(protocol);
         convertSchemaItem(protocol);
      }

      public void Visit(AdvancedProtocol protocol)
      {
         adjustDefaultStateOfAllParametersIn(protocol);
         protocol.AllSchemas.Each(convertSchema);
      }

      private void convertSchema(Schema schema)
      {
         schema.NumberOfRepetitions.IsDefault = false;
         schema.TimeBetweenRepetitions.IsDefault = false;
         schema.StartTime.IsDefault = false;
         convertSchemaItems(schema.SchemaItems);
      }

      private void convertSchemaItems(IEnumerable<ISchemaItem> schemaItems) => schemaItems.Each(convertSchemaItem);

      private void convertSchemaItem(ISchemaItem schemaItem)
      {
         if(schemaItem==null)
            return;

         var templateSchemaItem = _schemaItemRepository.SchemaItemBy(schemaItem.ApplicationType);
         updateIsInputStateByNameAndValue(schemaItem, templateSchemaItem);
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
         if (templateContainer == null)
            return;

         foreach (var templateParameter in templateContainer.AllParameters(x => x.Visible && x.ValueIsComputable()))
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

         foreach (var process in compound.AllProcesses())
         {
            var templateProcess = _compoundProcessRepository.ProcessByName(process.InternalName);
            updateIsInputStateByNameAndValue(process, templateProcess);
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