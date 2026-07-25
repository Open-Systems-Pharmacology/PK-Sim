using System.Xml.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using CoreConverter121To130 = OSPSuite.Core.Converters.v13.Converter121To130;

namespace PKSim.Infrastructure.ProjectConverter.v13
{
   /// <summary>
   ///    Brings projects saved before v13 in line with the new oral absorption and particle dissolution model. Only the
   ///    individual and the population are converted: they are the building blocks whose structure is used directly to
   ///    build a simulation and whose parameters the user can have edited. Formulations, events and compounds are rebuilt
   ///    from the database templates when a simulation is created, so their saved structure never reaches the model and
   ///    does not need to be converted.
   /// </summary>
   public class Converter12To13 : IObjectConverter,
      IVisitor<Individual>,
      IVisitor<Population>,
      IVisitor<Simulation>
   {
      private readonly CoreConverter121To130 _coreConverter;
      private readonly IDefaultIndividualRetriever _defaultIndividualRetriever;
      private readonly ITemplateStructureUpdater _templateStructureUpdater;
      private readonly IIndividualCalculationMethodsUpdater _individualCalculationMethodsUpdater;
      private readonly IPopulationParameterValuesUpdater _populationParameterValuesUpdater;
      private bool _converted;

      public Converter12To13(
         CoreConverter121To130 coreConverter,
         IDefaultIndividualRetriever defaultIndividualRetriever,
         ITemplateStructureUpdater templateStructureUpdater,
         IIndividualCalculationMethodsUpdater individualCalculationMethodsUpdater,
         IPopulationParameterValuesUpdater populationParameterValuesUpdater)
      {
         _coreConverter = coreConverter;
         _defaultIndividualRetriever = defaultIndividualRetriever;
         _templateStructureUpdater = templateStructureUpdater;
         _individualCalculationMethodsUpdater = individualCalculationMethodsUpdater;
         _populationParameterValuesUpdater = populationParameterValuesUpdater;
      }

      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V12;

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         (_, _converted) = _coreConverter.Convert(objectToConvert);
         this.Visit(objectToConvert);
         return (ProjectVersions.V13, _converted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         (_, bool converted) = _coreConverter.ConvertXml(element);
         return (ProjectVersions.V13, converted);
      }

      public void Visit(Individual individual) => convertIndividual(individual);

      public void Visit(Population population) => convertPopulation(population);

      public void Visit(Simulation simulation) => convertSimulation(simulation);

      private void convertSimulation(Simulation simulation)
      {
         if (simulation == null)
            return;

         //A simulation keeps its own copy of the individual it was created from, so it is converted as well
         _individualCalculationMethodsUpdater.AddMissingCalculationMethodsTo(simulation);

         convertIndividual(simulation.BuildingBlock<Individual>());
         convertPopulation(simulation.BuildingBlock<Population>());
      }

      private void convertPopulation(Population population)
      {
         if (population == null)
            return;

         //The individual has to catch up first: its new parameters are the ones the population needs values for
         convertIndividual(population.FirstIndividual);

         if (_populationParameterValuesUpdater.AddMissingParameterValuesTo(population))
            _converted = true;
      }

      private void convertIndividual(Individual individual)
      {
         if (individual == null)
            return;

         _individualCalculationMethodsUpdater.AddMissingCalculationMethodsTo(individual);

         var templateIndividual = templateFor(individual);
         if (templateIndividual == null)
            return;

         if (_templateStructureUpdater.AddMissingStructureTo(individual.Organism, templateIndividual.Organism))
            _converted = true;

         if (refreshLumenDefinitions(individual, templateIndividual))
            _converted = true;
      }

      /// <summary>
      ///    The lumen is where the new absorption model redefined existing parameters, turning constants into formulas and
      ///    into distributions. Everywhere else the old definitions still hold, so only the lumen is refreshed.
      /// </summary>
      private bool refreshLumenDefinitions(Individual individual, Individual templateIndividual) =>
         _templateStructureUpdater.RefreshParameterDefinitionsIn(lumenOf(individual), lumenOf(templateIndividual));

      private static IContainer lumenOf(Individual individual) =>
         individual.Organism.GetSingleChildByName<IContainer>(CoreConstants.Organ.LUMEN);

      private Individual templateFor(Individual individual)
      {
         //The new parameters are species dependent, so the template has to match the population of the individual
         var speciesPopulation = individual.OriginData?.Population;
         return speciesPopulation == null ? null : _defaultIndividualRetriever.DefaultIndividualFor(speciesPopulation);
      }
   }
}
