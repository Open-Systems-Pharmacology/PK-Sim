using System.Xml.Linq;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.ProjectConverter.v7_2
{
   public class Converter720To721 : IObjectConverter,
      IVisitor<Individual>,
      IVisitor<Population>,
      IVisitor<IndividualSimulation>,
      IVisitor<PopulationSimulation>
   {
      private readonly IIndividualCalculationMethodsUpdater _individualCalculationMethodsUpdater;

      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V7_2_0;

      private bool _converted;

      public Converter720To721(IIndividualCalculationMethodsUpdater individualCalculationMethodsUpdater)
      {
         _individualCalculationMethodsUpdater = individualCalculationMethodsUpdater;
      }

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _converted = false;
         this.Visit(objectToConvert);
         return (ProjectVersions.V7_2_1, _converted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         //nothing to do here
         return (ProjectVersions.V7_2_1, false);
      }

      private void convertIndividual(Individual individual)
      {
         if (individual == null)
            return;

         _individualCalculationMethodsUpdater.AddMissingCalculationMethodsTo(individual);
      }

      public void Visit(Individual individual)
      {
         convertIndividual(individual);
         _converted = true;
      }

      public void Visit(Population population)
      {
         Visit(population.FirstIndividual);
      }

      public void Visit(IndividualSimulation individualSimulation)
      {
         Visit(individualSimulation.Individual);
      }

      public void Visit(PopulationSimulation populationSimulation)
      {
         Visit(populationSimulation.Population);
      }
   }
}