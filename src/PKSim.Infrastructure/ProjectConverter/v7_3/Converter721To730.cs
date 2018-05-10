using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FluentNHibernate.Utils;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.ProjectConverter.v7_3
{
   public class Converter721To730 : IObjectConverter,
      IVisitor<Compound>,
      IVisitor<Simulation>,
      IVisitor<IPKSimBuildingBlock>,
      IVisitor<PKSimProject>

   {
      private readonly ICompoundFactory _compoundFactory;
      private readonly ICloner _cloner;
      private readonly ICreationMetaDataFactory _creationMetaDataFactory;
      private readonly IContainerTask _containerTask;
      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V7_2_1;

      private bool _converted;

      public Converter721To730(
         ICompoundFactory compoundFactory,
         ICloner cloner,
         ICreationMetaDataFactory creationMetaDataFactory,
         IContainerTask containerTask)
      {
         _compoundFactory = compoundFactory;
         _cloner = cloner;
         _creationMetaDataFactory = creationMetaDataFactory;
         _containerTask = containerTask;
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
         resetAllParametersToIsDefaultIn(compound);
         convertCompound(compound);
      }

      public void Visit(Simulation simulation)
      {
         simulation.AllBuildingBlocks<IPKSimBuildingBlock>().Each(resetAllParametersToIsDefaultIn);
         simulation.Compounds.Each(convertCompound);
         resetAllParametersToIsDefault(simulation.All<IParameter>());
      }

      public void Visit(PKSimProject project)
      {
         convertProject(project);
      }

      public void Visit(IPKSimBuildingBlock buildingBlock)
      {
         resetAllParametersToIsDefaultIn(buildingBlock);
      }

      private void resetAllParametersToIsDefaultIn(IContainer container)
      {
         resetAllParametersToIsDefault(container.GetAllChildren<IParameter>());
      }

      private void resetAllParametersToIsDefault(IEnumerable<IParameter> allParameters)
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
            var alternative = templateAlternativeGroup.AllAlternatives.First();
            var compoundGroup = compound.ParameterAlternativeGroup(templateAlternativeGroup.Name);
            foreach (var parameter in alternative.AllParameters(x => !x.IsDefault))
            {
               var compoundParameters = compoundGroup.AllAlternatives.Select(x => x.Parameter(parameter.Name));
               compoundParameters.Each(setAsInput);
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