using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ProjectConverter.v5_3
{
   public class Converter531To532 : IObjectConverter,
      IVisitor<PopulationSimulation>,
      IVisitor<Individual>,
      IVisitor<Population>
   {
      private readonly IRenalAgingCalculationMethodUpdater _renalAgingCalculationMethodUpdater;

      public Converter531To532(IRenalAgingCalculationMethodUpdater renalAgingCalculationMethodUpdater)
      {
         _renalAgingCalculationMethodUpdater = renalAgingCalculationMethodUpdater;
      }

      public bool IsSatisfiedBy(int version)
      {
         return version == ProjectVersions.V5_3_1;
      }

      public int Convert(object objectToConvert, int originalVersion)
      {
         this.Visit(objectToConvert);
         return ProjectVersions.V5_3_2;
      }

      public int ConvertXml(XElement element, int originalVersion)
      {
         //Remove the favorite node from the project element that would corrupt the project
         if (element.Name == "Project")
         {
            removeFavoritesNode(element);
         }
         return ProjectVersions.V5_3_2;
      }

      private void removeFavoritesNode(XElement element)
      {
         var favoriteElements = element.Descendants("Favorites").ToList();
         favoriteElements.Each(e => e.Remove());
      }

      public void Visit(PopulationSimulation populationSimulation)
      {
         updateVariableInPopulationFlag(populationSimulation.Individual);
         updateVariableInPopulationFlag(populationSimulation.AllPotentialAdvancedParameters.ToList());
      }

      public void Visit(Individual individual)
      {
         _renalAgingCalculationMethodUpdater.AddRenalAgingCalculationMethodTo(individual);
         updateVariableInPopulationFlag(individual);
      }

      public void Visit(Population population)
      {
         updateVariableInPopulationFlag(population.FirstIndividual);
      }

      private void updateVariableInPopulationFlag(Individual individual)
      {
         if (individual == null) return;
         updateVariableInPopulationFlag(individual.GetAllChildren<IParameter>());
      }

      /// <summary>
      /// This conversion is only required for project that where created before 5.3.1 but were converted to 5.3.1
      /// The conversion to 5.3.1 forgot to update the CanBeVaried flag 
      /// </summary>
      /// <param name="parameters"></param>
      private void updateVariableInPopulationFlag(IReadOnlyList<IParameter> parameters)
      {
         var allCanBeVaried = parameters.Where(x => x.CanBeVaried).ToList();
         var allCanBeVariedInPopulation = parameters.Where(x => x.CanBeVariedInPopulation).ToList();
         //only can be varied parameter but not one can be varied in population? Conversion error
         
         //That number (10) should be big enough so that it is more than the number of parameters creating during previous conversion.
         //But it should also be less that the number of can be varied in population with have in a typical simulation/individual
         if (allCanBeVariedInPopulation.Count <= 10)
         {
            allCanBeVaried.Where(p=>p.Visible).Each(x => x.CanBeVariedInPopulation = true);
         }
      }
   }
}