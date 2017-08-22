using System.Xml.Linq;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.ProjectConverter.v5_6
{
   public class Converter561To562 : IObjectConverter,
      IVisitor<Individual>

   {
      private readonly ICloner _cloner;
      private readonly IndividualMolecule _defaultMolecule;
      private bool _converted;

      public Converter561To562(ICloner cloner, IIndividualEnzymeFactory individualEnzymeFactory)
      {
         _cloner = cloner;
         _defaultMolecule = individualEnzymeFactory.CreateEmpty();
      }

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _converted = false;
         this.Visit(objectToConvert);
         return (ProjectVersions.V5_6_2, _converted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         return (ProjectVersions.V5_6_2, false);
      }

      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V5_6_1;

      public void Visit(Individual individual)
      {
         individual.AllMolecules().Each(addHalfLifeToMolecule);
         _converted = true;
      }

      private void addHalfLifeToMolecule(IndividualMolecule individualMolecule)
      {
         individualMolecule.Add(_cloner.Clone(_defaultMolecule.HalfLifeLiver));
         individualMolecule.Add(_cloner.Clone(_defaultMolecule.HalfLifeIntestine));
      }
   }
}