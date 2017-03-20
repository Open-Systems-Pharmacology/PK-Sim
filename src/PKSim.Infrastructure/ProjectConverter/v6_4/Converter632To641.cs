using System.Xml.Linq;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Infrastructure.ProjectConverter.v6_4
{
   public class Converter632To641 : IObjectConverter, 
      IVisitor<Individual>,
      IVisitor<Population>,
      IVisitor<Simulation>
   {
      private readonly IParameterFactory _parameterFactory;
      private readonly IRenalAgingCalculationMethodUpdater _renalAgingCalculationMethodUpdater;

      public Converter632To641(IParameterFactory parameterFactory, IRenalAgingCalculationMethodUpdater renalAgingCalculationMethodUpdater)
      {
         _parameterFactory = parameterFactory;
         _renalAgingCalculationMethodUpdater = renalAgingCalculationMethodUpdater;
      }

      public bool IsSatisfiedBy(int version)
      {
         return version == ProjectVersions.V6_3_2;
      }

      public int Convert(object objectToConvert, int originalVersion)
      {
         this.Visit(objectToConvert);
         return ProjectVersions.V6_4_1;
      }

      public int ConvertXml(XElement element, int originalVersion)
      {
         return ProjectVersions.V6_4_1;
      }

      public void Visit(Individual individual)
      {
         convertIndividual(individual);
      }

      private void convertIndividual(Individual individual)
      {
         if (individual == null) return;

         // create hidden Renal Aging factor-parameter and set it to 1
         var kidney = individual.Organism.Organ(CoreConstants.Organ.Kidney);

         var renalAgingParameter = _parameterFactory.CreateFor(ConverterConstants.Parameter.RenalAgingScaleFactor, 1.0, PKSimBuildingBlockType.Individual);
         renalAgingParameter.CanBeVaried = false;
         renalAgingParameter.CanBeVariedInPopulation = false;
         renalAgingParameter.Editable = false;
         renalAgingParameter.Visible = false;

         kidney.Add(renalAgingParameter);

         _renalAgingCalculationMethodUpdater.AddRenalAgingCalculationMethodTo(individual);
      }

      public void Visit(Population population)
      {
         convertIndividual(population.FirstIndividual);
      }

      public void Visit(Simulation simulation)
      {
         convertIndividual(simulation.BuildingBlock<Individual>());
         _renalAgingCalculationMethodUpdater.AddRenalAgingCalculationMethodTo(simulation);
      }
   }
}
