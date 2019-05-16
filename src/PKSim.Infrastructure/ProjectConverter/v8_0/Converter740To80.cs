using System;
using System.Xml.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.ProjectConverter.v7_5
{
   public class Converter740To80 : IObjectConverter,
      IVisitor<Compound>,
      IVisitor<Simulation>
   {
      private readonly ICloner _cloner;
      private bool _converted;
      private readonly Lazy<Compound> _templateCompound;
      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V7_4_0;

      public Converter740To80(
         ICompoundFactory compoundFactory,
         ICloner cloner)
      {
         _templateCompound = new Lazy<Compound>(compoundFactory.Create);
         _cloner = cloner;
      }

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _converted = false;
         this.Visit(objectToConvert);
         return (ProjectVersions.V8_0, _converted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         return (ProjectVersions.V8_0, false);
      }

      public void Visit(Compound compound) => convertCompound(compound);

      public void Visit(Simulation simulation) => simulation.Compounds.Each(convertCompound);

      private void convertCompound(Compound compound)
      {
         var templateCompound = _templateCompound.Value;
         var enableSupersaturation = _cloner.Clone(templateCompound.Parameter(Constants.Parameters.ENABLE_SUPERSATURATION));
         compound.Add(enableSupersaturation);
         _converted = true;
      }
   }
}