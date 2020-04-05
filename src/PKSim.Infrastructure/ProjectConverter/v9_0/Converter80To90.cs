using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using FluentNHibernate.Utils;
using OSPSuite.Core.Converters.v9;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Populations;
using OSPSuite.Core.Domain.SensitivityAnalyses;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Serialization;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Infrastructure.Serialization.Xml;
using IoC = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Infrastructure.ProjectConverter.v9_0
{
   public class Converter80To90 : IObjectConverter,
      IVisitor<PopulationSimulation>,
      IVisitor<PopulationAnalysisChart>,
      IVisitor<Population>,
      IVisitor<SensitivityAnalysis>,
      IVisitor<PopulationSimulationPKAnalyses>

   {
      private readonly IoC _container;
      private readonly Converter730To90 _converter730To90;
      private bool _converted;

      public Converter80To90(IoC container, Converter730To90 converter730To90)
      {
         _container = container;
         _converter730To90 = converter730To90;
      }

      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V8_0;

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _converted = false;
         this.Visit(objectToConvert);

         return (ProjectVersions.V9_0, _converted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         _converted = false;
         if (element.Name.IsOneOf("PopulationSimulation", "RandomPopulation", "MoBiPopulation", "ImportPopulation"))
         {
            convertIndividualValueCacheElement(element);
            _converted = true;
         }

         return (ProjectVersions.V9_0, _converted);
      }

      private void convertIndividualValueCacheElement(XElement element)
      {
         //With this release, the IndividualPropertiesCache was renamed to IndividualValuesCache
         //Covariates structure was changed completely  (saved by covariate as opposed to saved by individual)
         foreach (var individualPropertiesCacheElement in element.Descendants("IndividualPropertiesCache").ToList())
         {
            var parameterValueCacheElement = individualPropertiesCacheElement.Descendants("ParameterValuesCache");
            var allCovariatesElement = individualPropertiesCacheElement.Descendants("AllCovariates");

            var covariateValuesCacheElement = createCovariateValuesCacheElement(allCovariatesElement);
            convertParameterValueCacheElement(parameterValueCacheElement);

            var individualValuesCacheElement = new XElement("IndividualValuesCache");
            individualValuesCacheElement.Add(parameterValueCacheElement);
            individualValuesCacheElement.Add(covariateValuesCacheElement);
            var parent = individualPropertiesCacheElement.Parent;
            individualPropertiesCacheElement.Remove();
            parent.Add(individualValuesCacheElement);
         }
      }

      private void convertParameterValueCacheElement(IEnumerable<XElement> parameterValueCacheElement)
      {
         foreach (var percentilesListElement in parameterValueCacheElement.Descendants("DoubleList").ToList())
         {
            percentilesListElement.Name = Constants.Serialization.PERCENTILES;
         }
      }

      private XElement createCovariateValuesCacheElement(IEnumerable<XElement> allCovariatesElement)
      {
         var covariateValuesCache = new CovariateValuesCache();
         var individualCovariatesReader = _container.Resolve<IXmlReader<IndividualCovariates>>();
         var covariateValuesCacheWriter = _container.Resolve<IXmlWriter<CovariateValuesCache>>();

         var context = SerializationTransaction.Create(_container);

         // List of old covariates as defined in PKSim 8.x and below
         var allIndividualCovariates = new List<IndividualCovariates>();
         foreach (var individualCovariatesElement in allCovariatesElement.Descendants("IndividualCovariates"))
         {
            var individualCovariates = individualCovariatesReader.ReadFrom(individualCovariatesElement, context);
            allIndividualCovariates.Add(individualCovariates);
         }

         var allCovariateNames = allIndividualCovariates.FirstOrDefault()?.Attributes.Keys.ToList() ?? new List<string>();
         allCovariateNames.Add(Constants.Population.GENDER);
         allCovariateNames.Add(Constants.Population.POPULATION);

         // Transform the old covariates in the new structure
         foreach (var covariateName in allCovariateNames)
         {
            var covariateValues = new CovariateValues(covariateName);
            if (string.Equals(covariateName, Constants.Population.GENDER))
               covariateValues.Values = allIndividualCovariates.Select(x => x.Gender.Name).ToList();
            else if (string.Equals(covariateName, Constants.Population.POPULATION))
               covariateValues.Values = allIndividualCovariates.Select(x => x.Race.Name).ToList();
            else
               covariateValues.Values = allIndividualCovariates.Select(x => x.Covariate(covariateName)).ToList();

            covariateValuesCache.Add(covariateValues);
         }

         return covariateValuesCacheWriter.WriteFor(covariateValuesCache, context);
      }

      public void Visit(PopulationSimulation populationSimulation)
      {
         Visit(populationSimulation.Population);
         Visit(populationSimulation.PKAnalyses);
      }

      public void Visit(Population population)
      {
         if (population == null)
            return;

         convertIndividualValueCache(population.IndividualValuesCache);
      }

      private void convertIndividualValueCache(IndividualValuesCache individualValuesCache)
      {
         var firstParameterValue = individualValuesCache.AllParameterValues.FirstOrDefault();
         if (firstParameterValue == null)
            return;

         individualValuesCache.IndividualIds.AddRange(Enumerable.Range(0, firstParameterValue.Count));
         _converted = true;
      }

      public void Visit(PopulationAnalysisChart populationAnalysisChart)
      {
         var populationAnalysis = populationAnalysisChart.BasePopulationAnalysis;

         convertCovariates(populationAnalysis);
         convertPKAnalysisField(populationAnalysis);
         _converted = true;
      }

      private void convertPKAnalysisField(PopulationAnalysis populationAnalysis)
      {
         populationAnalysis?.All<PopulationAnalysisPKParameterField>().Each(pkField =>
            {
               pkField.PKParameter = _converter730To90.ConvertPKParameterName(pkField.PKParameter);
            });
      }

      private void convertCovariates(PopulationAnalysis populationAnalysis)
      {
         if (!(populationAnalysis.FieldByName(ConverterConstants.Population.RACE) is PopulationAnalysisCovariateField raceCovariateField))
            return;

         populationAnalysis.RenameField(ConverterConstants.Population.RACE, Constants.Population.POPULATION);
         raceCovariateField.Covariate = Constants.Population.POPULATION;

      }

      public void Visit(SensitivityAnalysis sensitivityAnalysis)
      {
         (_, _converted) = _converter730To90.Convert(sensitivityAnalysis);
      }

      public void Visit(PopulationSimulationPKAnalyses populationSimulationPKAnalyses)
      {
         (_, _converted) = _converter730To90.Convert(populationSimulationPKAnalyses);
      }
   }
}