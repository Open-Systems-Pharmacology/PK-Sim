using System.Xml.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Infrastructure.ProjectConverter.v6_4
{
   public class Converter632To641 : IObjectConverter,
      IVisitor<Individual>,
      IVisitor<Population>,
      IVisitor<Simulation>
   {
      private readonly IParameterFactory _parameterFactory;
      private readonly IIndividualCalculationMethodsUpdater _individualCalculationMethodsUpdater;
      private bool _converted;

      public Converter632To641(IParameterFactory parameterFactory, IIndividualCalculationMethodsUpdater individualCalculationMethodsUpdater)
      {
         _parameterFactory = parameterFactory;
         _individualCalculationMethodsUpdater = individualCalculationMethodsUpdater;
      }

      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V6_3_2;

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _converted = true;
         this.Visit(objectToConvert);
         return (ProjectVersions.V6_4_1, _converted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         return (ProjectVersions.V6_4_1, false);
      }

      public void Visit(Individual individual)
      {
         convertIndividual(individual);
         _converted = true;
      }

      private void convertIndividual(Individual individual)
      {
         if (individual == null) return;

         // create hidden Renal Aging factor-parameter and set it to 1
         var kidney = individual.Organism.Organ(CoreConstants.Organ.Kidney);

         var renalAgingParameter = _parameterFactory.CreateFor(ConverterConstants.Parameters.RenalAgingScaleFactor, 1.0, PKSimBuildingBlockType.Individual);
         renalAgingParameter.CanBeVaried = false;
         renalAgingParameter.CanBeVariedInPopulation = false;
         renalAgingParameter.Editable = false;
         renalAgingParameter.Visible = false;

         kidney.Add(renalAgingParameter);

         _individualCalculationMethodsUpdater.AddMissingCalculationMethodsTo(individual);
      }

      public void Visit(Population population)
      {
         convertIndividual(population.FirstIndividual);
         _converted = true;
      }

      public void Visit(Simulation simulation)
      {
         convertIndividual(simulation.BuildingBlock<Individual>());
         _individualCalculationMethodsUpdater.AddMissingCalculationMethodsTo(simulation);
         _converted = true;
      }
   }
}