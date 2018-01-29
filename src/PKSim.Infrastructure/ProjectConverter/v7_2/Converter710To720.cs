using System.Linq;
using System.Xml.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.ProjectConverter.v7_2
{
   public class Converter710To720 : IObjectConverter,
      IVisitor<Individual>,
      IVisitor<Population>,
      IVisitor<IndividualSimulation>,
      IVisitor<PopulationSimulation>

   {
      private readonly IDefaultIndividualRetriever _defaultIndividualRetriever;
      private readonly ICloner _cloner;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IOrganTypeRepository _organTypeRepository;
      private readonly IIndividualCalculationMethodsUpdater _individualCalculationMethodsUpdater;
      private bool _converted;

      public Converter710To720(IDefaultIndividualRetriever defaultIndividualRetriever, ICloner cloner, IEntityPathResolver entityPathResolver, IOrganTypeRepository organTypeRepository, IIndividualCalculationMethodsUpdater individualCalculationMethodsUpdater)
      {
         _defaultIndividualRetriever = defaultIndividualRetriever;
         _cloner = cloner;
         _entityPathResolver = entityPathResolver;
         _organTypeRepository = organTypeRepository;
         _individualCalculationMethodsUpdater = individualCalculationMethodsUpdater;
      }

      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V7_1_0;

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _converted = false;
         this.Visit(objectToConvert);
         return (ProjectVersions.V7_2_0, _converted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         _converted = false;
         convertOrganTypes(element);
         return (ProjectVersions.V7_2_0, _converted);
      }

      private void convertOrganTypes(XElement element)
      {
         var allOrganTypesAttributes = from child in element.DescendantsAndSelf()
            where child.HasAttributes
            let attr = child.Attribute(ConverterConstants.Serialization.Attribute.ORGAN_TYPE)
            where attr != null
            select attr;

         foreach (var organTypesAttribute in allOrganTypesAttributes)
         {
            organTypesAttribute.SetValue(convertOrganType(organTypesAttribute.Value));
            _converted = true;
         }
      }

      private OrganType convertOrganType(string oldOrganType) => _organTypeRepository.OrganTypeFor(organName: oldOrganType);

      public void Visit(Individual individual)
      {
         convertIndividual(individual);
         _converted = true;
      }

      public void Visit(Population population)
      {
         Visit(population.FirstIndividual);
         addBSAParameterValues(population);
         _converted = true;
      }

      public void Visit(IndividualSimulation individualSimulation)
      {
         Visit(individualSimulation.Individual);
         _converted = true;
      }

      public void Visit(PopulationSimulation populationSimulation)
      {
         Visit(populationSimulation.Population);
         _converted = true;
      }

      private void convertIndividual(Individual individual)
      {
         if (individual == null)
            return;


         _individualCalculationMethodsUpdater.AddMissingCalculationMethodsTo(individual);

         if (!individual.IsHuman)
            return;

         if (individual.Organism.Parameter(CoreConstants.Parameters.BSA) != null)
            return;

         var defaultHuman = _defaultIndividualRetriever.DefaultHuman();
         var bsa = defaultHuman.Organism.Parameter(CoreConstants.Parameters.BSA);

         individual.Organism.Add(_cloner.Clone(bsa));
      }

      private void addBSAParameterValues(Population population)
      {
         if (!population.IsHuman)
            return;

         var defaultHuman = _cloner.Clone(_defaultIndividualRetriever.DefaultHuman());

         var heightParameter = defaultHuman.Organism.Parameter(CoreConstants.Parameters.HEIGHT);
         var weightParameter = defaultHuman.Organism.Parameter(CoreConstants.Parameters.WEIGHT);
         var bsaParameter = defaultHuman.Organism.Parameter(CoreConstants.Parameters.BSA);

         var allWeights = population.AllOrganismValuesFor(weightParameter.Name, _entityPathResolver);
         var allHeights = population.AllOrganismValuesFor(heightParameter.Name, _entityPathResolver);

         if (allHeights.Count != allWeights.Count)
            return;

         var parameterValues = new ParameterValues(_entityPathResolver.PathFor(bsaParameter));
         for (int i = 0; i < allWeights.Count; i++)
         {
            weightParameter.Value = allWeights[i];
            heightParameter.Value = allHeights[i];
            parameterValues.Add(bsaParameter.Value);
         }

         population.IndividualPropertiesCache.Add(parameterValues);
      }
   }
}