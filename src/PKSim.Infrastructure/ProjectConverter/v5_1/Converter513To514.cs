using System.Xml.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ProjectConverter.v5_1
{
   public class Converter513To514 : IObjectConverter,
      IVisitor<Individual>,
      IVisitor<RandomPopulation>,
      IVisitor<Simulation>

   {
      private readonly IDefaultIndividualRetriever _defaultIndividualRetriever;
      private readonly ICloner _cloner;
      private readonly IRenalAgingCalculationMethodUpdater _renalAgingCalculationMethodUpdater;
      private Species _currentSpecies;
      private readonly ICache<Species, Individual> _defaultCache;

      public Converter513To514(IDefaultIndividualRetriever defaultIndividualRetriever, ICloner cloner, IRenalAgingCalculationMethodUpdater renalAgingCalculationMethodUpdater)
      {
         _defaultIndividualRetriever = defaultIndividualRetriever;
         _cloner = cloner;
         _renalAgingCalculationMethodUpdater = renalAgingCalculationMethodUpdater;
         _defaultCache = new Cache<Species, Individual>(x => x.Species, x => null);
      }

      public bool IsSatisfiedBy(int version)
      {
         return version == ProjectVersions.V5_1_3;
      }

      public int Convert(object objectToConvert, int originalVersion)
      {
         this.Visit(objectToConvert);
         return ProjectVersions.V5_1_4;
      }

      public int ConvertXml(XElement element, int originalVersion)
      {
         return ProjectVersions.V5_1_4;
      }

      public void Visit(Individual individual)
      {
         _currentSpecies = individual.Species;
         convertTransitTimeFor(individual);
         _renalAgingCalculationMethodUpdater.AddRenalAgingCalculationMethodTo(individual);
      }

      private void convertTransitTimeFor(IContainer individualToConvert)
      {
         if (!_defaultCache.Contains(_currentSpecies))
         {
            _defaultCache.Add(_defaultIndividualRetriever.DefaultIndividualFor(_currentSpecies));
         }

         var defaultIndividual = _defaultCache[_currentSpecies];
         var defaultSITT = smallIntestineIn(defaultIndividual).Parameter(ConverterConstants.Parameter.SITT);
         var defaultSITT_factor = smallIntestineIn(defaultIndividual).Parameter(ConverterConstants.Parameter.SITT_factor);
         var defaultSITT_factor_slope = smallIntestineIn(defaultIndividual).Parameter(ConverterConstants.Parameter.SITT_factor_slope);
         var defaultSITT_factor_intercept = smallIntestineIn(defaultIndividual).Parameter(ConverterConstants.Parameter.SITT_factor_intercept);

         var defaultLITT = largeIntestineIn(defaultIndividual).Parameter(ConverterConstants.Parameter.LITT);
         var defaultLITT_factor = largeIntestineIn(defaultIndividual).Parameter(ConverterConstants.Parameter.LITT_factor);
         var defaultLITT_factor_slope = largeIntestineIn(defaultIndividual).Parameter(ConverterConstants.Parameter.LITT_factor_slope);
         var defaultLITT_factor_intercept = largeIntestineIn(defaultIndividual).Parameter(ConverterConstants.Parameter.LITT_factor_intercept);

         var defaultGET = GET(defaultIndividual);
         var currentGET = GET(individualToConvert);
         currentGET.Info.MaxValue = defaultGET.MaxValue;

         var smallIntestine = smallIntestineIn(individualToConvert);
         var largeIntestine = largeIntestineIn(individualToConvert);

         smallIntestine.Add(_cloner.Clone(defaultSITT));
         smallIntestine.Add(_cloner.Clone(defaultSITT_factor_slope));
         smallIntestine.Add(_cloner.Clone(defaultSITT_factor_intercept));
         largeIntestine.Add(_cloner.Clone(defaultLITT));
         largeIntestine.Add(_cloner.Clone(defaultLITT_factor_slope));
         largeIntestine.Add(_cloner.Clone(defaultLITT_factor_intercept));

         var SITT = smallIntestine.Parameter(ConverterConstants.Parameter.SITT);
         var SITT_fact_old = smallIntestine.Parameter(ConverterConstants.Parameter.SITT_factor).Value;
         var P1_sin = smallIntestine.Parameter(ConverterConstants.Parameter.SITT_factor_slope).Value;
         var P2_sin = smallIntestine.Parameter(ConverterConstants.Parameter.SITT_factor_intercept).Value;

         var LITT = largeIntestine.Parameter(ConverterConstants.Parameter.LITT);
         var LITT_fact_old = largeIntestine.Parameter(ConverterConstants.Parameter.LITT_factor).Value;
         var P1_lin = largeIntestine.Parameter(ConverterConstants.Parameter.LITT_factor_slope).Value;
         var P2_lin = largeIntestine.Parameter(ConverterConstants.Parameter.LITT_factor_intercept).Value;

         SITT.Value = (SITT_fact_old - P2_sin) / P1_sin;
         LITT.Value = (LITT_fact_old - P2_lin) / P1_lin;

         //last but not least remove parameter to update formula
         smallIntestine.RemoveChild(smallIntestine.Parameter(ConverterConstants.Parameter.SITT_factor));
         largeIntestine.RemoveChild(largeIntestine.Parameter(ConverterConstants.Parameter.LITT_factor));

         smallIntestine.Add(_cloner.Clone(defaultSITT_factor));
         largeIntestine.Add(_cloner.Clone(defaultLITT_factor));

         //removed unused parameters
         smallIntestine.RemoveChild(smallIntestine.Parameter(ConverterConstants.Parameter.ColonArrivalTime));
         largeIntestine.RemoveChild(largeIntestine.Parameter(ConverterConstants.Parameter.ExcretionTime));
      }

      public void Visit(Simulation simulation)
      {
         var individual = simulation.Individual;
         Visit(individual);
         convertTransitTimeFor(simulation.Model.Root);
         updateParametersOrigin(individual, simulation.Model.Root);
      }

      private void updateParametersOrigin(Individual individual, IContainer root)
      {
         var indSmallIntestine = smallIntestineIn(individual);
         var indLargeIntestine = largeIntestineIn(individual);

         var simSmallIntestine = smallIntestineIn(root);
         var simLargeIntestine = largeIntestineIn(root);

         updateParameterOrigin(indSmallIntestine, simSmallIntestine, ConverterConstants.Parameter.SITT, individual.Id);
         updateParameterOrigin(indSmallIntestine, simSmallIntestine, ConverterConstants.Parameter.SITT_factor, individual.Id);
         updateParameterOrigin(indSmallIntestine, simSmallIntestine, ConverterConstants.Parameter.SITT_factor_slope, individual.Id);
         updateParameterOrigin(indSmallIntestine, simSmallIntestine, ConverterConstants.Parameter.SITT_factor_intercept, individual.Id);

         updateParameterOrigin(indLargeIntestine, simLargeIntestine, ConverterConstants.Parameter.LITT, individual.Id);
         updateParameterOrigin(indLargeIntestine, simLargeIntestine, ConverterConstants.Parameter.LITT_factor, individual.Id);
         updateParameterOrigin(indLargeIntestine, simLargeIntestine, ConverterConstants.Parameter.LITT_factor_slope, individual.Id);
         updateParameterOrigin(indLargeIntestine, simLargeIntestine, ConverterConstants.Parameter.LITT_factor_intercept, individual.Id);
      }

      private void updateParameterOrigin(IContainer individualOrgan, IContainer simulationOrgan, string parameter, string individualId)
      {
         var indParameter = individualOrgan.Parameter(parameter);
         var simParameter = simulationOrgan.Parameter(parameter);
         simParameter.Origin.BuilingBlockId = individualId;
         simParameter.Origin.ParameterId = indParameter.Id;
      }

      private IContainer smallIntestineIn(IContainer container)
      {
         return container.Container(Constants.ORGANISM).Container(CoreConstants.Organ.SmallIntestine);
      }

      private IContainer largeIntestineIn(IContainer container)
      {
         return container.Container(Constants.ORGANISM).Container(CoreConstants.Organ.LargeIntestine);
      }

      private IParameter GET(IContainer container)
      {
         return container.Container(Constants.ORGANISM)
            .Container(CoreConstants.Organ.Lumen).Container(CoreConstants.Organ.Stomach).Parameter(ConverterConstants.Parameter.GastricEmptyingTime);
      }

      public void Visit(RandomPopulation randomPopulation)
      {
         Visit(randomPopulation.FirstIndividual);
      }
   }
}