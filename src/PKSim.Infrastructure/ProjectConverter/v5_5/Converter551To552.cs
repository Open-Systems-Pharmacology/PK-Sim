using System.Linq;
using System.Xml.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Infrastructure.ProjectConverter.v5_5
{
   public class Converter551To552 : IObjectConverter,
      IVisitor<IndividualSimulation>,
      IVisitor<PopulationSimulation>,
      IVisitor<Individual>,
      IVisitor<Population>
   {
      private readonly IDefaultIndividualRetriever _defaultIndividualRetriever;
      private readonly ICloner _cloner;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly ICalculationMethodsUpdater _calculationMethodsUpdater;
      private readonly IIndividualUpdater _individualUpdater;
      private readonly Cache<Species, Individual> _defaultCache;
      private bool _converted;

      public Converter551To552(IDefaultIndividualRetriever defaultIndividualRetriever, ICloner cloner, 
                               IEntityPathResolver entityPathResolver, ICalculationMethodsUpdater calculationMethodsUpdater,
                               IIndividualUpdater individualUpdater)

      {
         _defaultIndividualRetriever = defaultIndividualRetriever;
         _cloner = cloner;
         _entityPathResolver = entityPathResolver;
         _calculationMethodsUpdater = calculationMethodsUpdater;
         _individualUpdater = individualUpdater;
         _defaultCache = new Cache<Species, Individual>(x => x.Species, x => null);
      }

      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V5_5_1;

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _converted = false;
         this.Visit(objectToConvert);
         return (ProjectVersions.V5_5_2, _converted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         //nothing to do here
         return (ProjectVersions.V5_5_2, false);
      }

      public void Visit(IndividualSimulation simulation)
      {
         Visit(simulation.BuildingBlock<Individual>());
         _converted = true;
      }

      public void Visit(PopulationSimulation simulation)
      {
         Visit(simulation.Population);
         _converted = true;
      }

      public void Visit(Individual individual)
      {
         if (individual == null) return;

         _calculationMethodsUpdater.AddMissingCalculationMethodsTo(individual);
         _individualUpdater.AddScalingExponentForFluidFlowTo(individual);

         addSurfaceAreaParametersTo(individual);
         updateSurfaceAreaIntCellMucosa(individual);
         _converted = true;
      }

      private void updateSurfaceAreaIntCellMucosa(Individual individual)
      {
         var templateIndividual = templateIndividualFor(individual);
         var allMucosaIntCellNeighborhoods = new[]
         {
            "Duodenum_int_Duodenum_cell",
            "UpperJejunum_int_UpperJejunum_cell",
            "LowerJejunum_int_LowerJejunum_cell",
            "UpperIleum_int_UpperIleum_cell",
            "LowerIleum_int_LowerIleum_cell",
            "Caecum_int_Caecum_cell",
            "ColonAscendens_int_ColonAscendens_cell",
            "ColonTransversum_int_ColonTransversum_cell",
            "ColonDescendens_int_ColonDescendens_cell",
            "ColonSigmoid_int_ColonSigmoid_cell",
            "Rectum_int_Rectum_cell"
         };

         foreach (var neighborhoodName in allMucosaIntCellNeighborhoods)
         {
            var templateNeighborhood = templateIndividual.Neighborhoods.Container(neighborhoodName);
            var currentNeighborhood = individual.Neighborhoods.Container(neighborhoodName);
            var templateSA_int_cell = templateNeighborhood.Parameter(ConverterConstants.Parameter.SA_int_cell);
            var currentSA_int_cell = currentNeighborhood.Parameter(ConverterConstants.Parameter.SA_int_cell);

            var templateFormula = templateSA_int_cell.Formula as ExplicitFormula;
            var currentFormula = currentSA_int_cell.Formula as ExplicitFormula;

            if (templateFormula == null || currentFormula == null)
               continue;

            currentFormula.FormulaString = templateFormula.FormulaString;
            currentFormula.ClearObjectPaths();
            templateFormula.ObjectPaths.Each(op => currentFormula.AddObjectPath(op.Clone<IFormulaUsablePath>()));
         }
      }

      private void addSurfaceAreaParametersTo(Individual individual)
      {
         var templateIndividual = templateIndividualFor(individual);
         var templateLumen = lumenIn(templateIndividual);
         var currentLumen = lumenIn(individual);
         var defaultFactor = effectiveSurfaceAreaVariabilityParameterIn(templateIndividual);
         currentLumen.Add(_cloner.Clone(defaultFactor));

         foreach (var segment in currentLumen.GetChildren<IContainer>())
         {
            var templateSegment = templateLumen.Container(segment.Name);

            var templateEffectiveSurfaceArea = templateSegment?.Parameter(ConverterConstants.Parameter.EffectiveSurfaceArea);
            if(templateEffectiveSurfaceArea == null)
               continue;

            //clone surface area parameter
            segment.Add(_cloner.Clone(templateEffectiveSurfaceArea));
         }
      }

      private Individual templateIndividualFor(Individual individual)
      {
         var species = individual.Species;
         if (!_defaultCache.Contains(species))
            _defaultCache.Add(_defaultIndividualRetriever.DefaultIndividualFor(species));

         var defaultIndividual = _defaultCache[species];
         return defaultIndividual;
      }

      private IParameter effectiveSurfaceAreaVariabilityParameterIn(Individual defaultIndividual)
      {
         return lumenIn(defaultIndividual).Parameter(ConverterConstants.Parameter.EffectiveSurfaceAreaVariabilityFactor);
      }

      private IContainer lumenIn(Individual individual)
      {
         return individual.EntityAt<IContainer>(Constants.ORGANISM, CoreConstants.Organ.Lumen);
      }

      public void Visit(Population population)
      {
         if (population == null) return;
         Visit(population.FirstIndividual);
         addSurfaceAreaVariabilityTo(population);
      }

      private void addSurfaceAreaVariabilityTo(Population population)
      {
         var individual = population.FirstIndividual;
         if (individual == null) return;
         var factor = effectiveSurfaceAreaVariabilityParameterIn(individual) as IDistributedParameter;
         if (factor == null) return;
         var parameterPath = _entityPathResolver.PathFor(factor);
         //use a clone as this parameter will be used to create the advanced parameter
         var clone = _cloner.Clone(factor);
         var advancedParameter = new AdvancedParameter {DistributedParameter = clone, Seed = population.Seed};
         var randomValues = advancedParameter.GenerateRandomValues(population.NumberOfItems);
         population.IndividualPropertiesCache.Add(randomValues.Select(rv => new ParameterValue(parameterPath, rv.Value, rv.Percentile)).ToList());
      }
   }
}