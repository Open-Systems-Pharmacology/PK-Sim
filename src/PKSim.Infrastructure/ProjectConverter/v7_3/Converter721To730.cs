using System;
using System.Xml.Linq;
using FluentNHibernate.Utils;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.ProjectConverter.v7_3
{
   public class Converter721To730 : IObjectConverter,
      IVisitor<Compound>,
      IVisitor<Simulation>,
      IVisitor<PKSimProject>

   {
      private readonly ICompoundFactory _compoundFactory;
      private readonly ICloner _cloner;
      private readonly ICreationMetaDataFactory _creationMetaDataFactory;
      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V7_2_1;

      private bool _converted;

      public Converter721To730(ICompoundFactory compoundFactory, ICloner cloner, ICreationMetaDataFactory creationMetaDataFactory)
      {
         _compoundFactory = compoundFactory;
         _cloner = cloner;
         _creationMetaDataFactory = creationMetaDataFactory;
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
         convertCompound(compound);
      }

      public void Visit(Simulation simulation)
      {
         simulation.Compounds.Each(convertCompound);
      }

      public void Visit(PKSimProject project)
      {
         convertProject(project);
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
         var templateCompound = _compoundFactory.Create();
         var solubilityTable = templateCompound.Parameter(CoreConstants.Parameters.SOLUBILITY_TABLE);

         compound.Add(_cloner.Clone(solubilityTable));

         var solubilityGroup = compound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_SOLUBILITY);
         solubilityGroup.AllAlternatives.Each(x => x.Add(_cloner.Clone(solubilityTable)));

         _converted = true;
      }

   }
}