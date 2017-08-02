using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using OSPSuite.Serializer.Xml.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.Xml;
using OSPSuite.Core.Converter.v5_2;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Serialization;
using IContainer = OSPSuite.Utility.Container.IContainer;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Infrastructure.ProjectConverter.v5_2
{
   /// <summary>
   ///    This class is used to convert population and population simulation from 5.0.4 to 5.1 so that percentile are available
   /// </summary>
   public interface IParameterValuesCacheConverter
   {
      void Convert(RandomPopulation randomPopulation);
      void Convert(PopulationSimulation populationSimulation);
      void Convert(XElement element);
   }

   public class ParameterValuesCacheConverter : IParameterValuesCacheConverter
   {
      private readonly IContainerTask _containerTask;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IParameterQuery _parameterQuery;
      private readonly IParameterFactory _parameterFactory;
      private readonly IGenderRepository _genderRepository;
      private readonly IContainer _container;
      private readonly IDimensionMapper _dimensionMapper;

      public ParameterValuesCacheConverter(IContainerTask containerTask, IEntityPathResolver entityPathResolver,
                                           IParameterQuery parameterQuery, IParameterFactory parameterFactory, 
                                           IGenderRepository genderRepository, IContainer container,IDimensionMapper dimensionMapper)
      {
         _containerTask = containerTask;
         _entityPathResolver = entityPathResolver;
         _parameterQuery = parameterQuery;
         _parameterFactory = parameterFactory;
         _genderRepository = genderRepository;
         _container = container;
         _dimensionMapper = dimensionMapper;
      }

      public void Convert(RandomPopulation randomPopulation)
      {
         var parameterCache = randomPopulation.IndividualPropertiesCache.ParameterValuesCache;
         var individual = randomPopulation.FirstIndividual;
         var parameterPathCache = _containerTask.CacheAllChildren<IParameter>(individual);

         foreach (var parameterPath in parameterCache.AllParameterPaths().ToList())
         {
            var updatedPath = updatePath(parameterPath, parameterCache);
            var parameter = parameterPathCache[updatedPath];
            if (parameter == null)
               continue;

            var parameterValues = parameterCache.ParameterValuesFor(updatedPath);
            convertValues(parameterValues,parameter);
            var distributedParameter = parameter as IDistributedParameter;
            if (distributedParameter == null)
               addDefaultPercentileFor(parameterValues);
            else
               addAgeDependentPercentileValues(parameterValues, randomPopulation, distributedParameter, individual);
         }
      }

      private void convertValues(ParameterValues parameterValues, IParameter parameter)
      {
         double factor = _dimensionMapper.ConversionFactor(parameter);
         if (factor == 1) return;

         var values = parameterValues.Values;
         for (int i = 0; i < values.Count; i++)
         {
            values[i] *= factor;
         }
         parameterValues.Values = values;
      }

      private string updatePath(string parameterPath, ParameterValuesCache parameterCache)
      {
         if (string.IsNullOrEmpty(parameterPath))
            return parameterPath;

         if (!parameterPath.Contains(CoreConstants.Parameter.ONTOGENY_FACTOR))
            return parameterPath;
      
         if (parameterPath.Contains("Lumen-Duodenum"))
         {
            parameterCache.Remove(parameterPath);
            return parameterPath;
         }

         if (parameterPath.Contains("Liver"))
         {
            var newPath = parameterPath.Replace("Liver|", "");
            parameterCache.RenamePath(parameterPath, newPath);
            return newPath;
         }

         if (parameterPath.Contains("Duodenum"))
         {
            var newPath = parameterPath.Replace("Duodenum|", "");
            newPath = newPath.Replace(CoreConstants.Parameter.ONTOGENY_FACTOR, CoreConstants.Parameter.ONTOGENY_FACTOR_GI);
            parameterCache.RenamePath(parameterPath, newPath);
            return newPath;
         }

         return parameterPath;
      }

      private void addAgeDependentPercentileValues(ParameterValues parameterValues, RandomPopulation randomPopulation, IDistributedParameter parameter, Individual individual)
      {
         var originData = individual.OriginData.Clone();
         var allAges = randomPopulation.AllValuesFor(_entityPathResolver.PathFor(individual.Organism.Parameter(CoreConstants.Parameter.AGE))).ToList();
         var allGender = randomPopulation.AllGenders.ToList();
         var allValues = randomPopulation.AllValuesFor(_entityPathResolver.PathFor(parameter)).ToList();
         var allPercentiles = new double[allValues.Count].InitializeWith(0);
         originData.GestationalAge = CoreConstants.NOT_PRETERM_GESTATIONAL_AGE_IN_WEEKS;

         //cache female and male distributions 
         originData.Gender = _genderRepository.Female;
         var allFemalesDistributions = _parameterQuery.ParameterDistributionsFor(parameter.ParentContainer, originData, p => string.Equals(p.ParameterName, parameter.Name)).ToList();

         originData.Gender = _genderRepository.Male;
         var allMaleDistributions = _parameterQuery.ParameterDistributionsFor(parameter.ParentContainer, originData, p => string.Equals(p.ParameterName, parameter.Name)).ToList();

         for (int individualIndex = 0; individualIndex < randomPopulation.NumberOfItems; individualIndex++)
         {
            //create orgin data for individual i
            originData.Age = allAges[individualIndex];
            var distributions = allFemalesDistributions;
            if (_genderRepository.Male == allGender[individualIndex])
               distributions = allMaleDistributions;

            if (distributions.Count == 0)
               allPercentiles[individualIndex] = CoreConstants.DEFAULT_PERCENTILE;
            else
               allPercentiles[individualIndex] = percentileFrom(distributions, originData, allValues[individualIndex]);
         }
         addPercentileValues(parameterValues, allPercentiles);
      }

      private double percentileFrom(IEnumerable<ParameterDistributionMetaData> allDistributionsForParameter, OriginData originData, double parameterValue)
      {
         var parameter = _parameterFactory.CreateFor(allDistributionsForParameter, originData);
         parameter.Value = parameterValue;
         return parameter.Percentile;
      }

      private void addDefaultPercentileFor(ParameterValues parameterValues)
      {
         var allPercentiles = new double[parameterValues.Values.Count].InitializeWith(CoreConstants.DEFAULT_PERCENTILE);
         addPercentileValues(parameterValues, allPercentiles);
      }

      private void addPercentileValues(ParameterValues parameterValues, double[] allPercentiles)
      {
         parameterValues.Percentiles = new List<double>(allPercentiles);
      }

      public void Convert(PopulationSimulation populationSimulation)
      {
         var parameterCache = populationSimulation.ParameterValuesCache;
         foreach (var parameterPath in parameterCache.AllParameterPaths())
         {
            addDefaultPercentileFor(parameterCache.ParameterValuesFor(parameterPath));
         }
      }

      public void Convert(XElement element)
      {
         foreach (var parameterCacheElement in element.Descendants("ParameterValuesCache").ToList())
         {
            var parameterValuesCache = new ParameterValuesCache();

            foreach (var dataColumnNode in parameterCacheElement.Descendants(ConverterConstants.Serialization.DATA_TABLE_COLUMN))
            {
               var parameterValues = new ParameterValues(dataColumnNode.GetAttribute(CoreConstants.Serialization.Attribute.Name));
               parameterValuesCache.Add(parameterValues);
            }

            var documentElement = parameterCacheElement.Descendants("DocumentElement").First();
            foreach (var parameterValuesElement in documentElement.Descendants("ParameterValues"))
            {
               int index = 0;
               foreach (var parameterValue in parameterValuesElement.Descendants())
               {
                  var parameterValues = parameterValuesCache.ParameterValuesAt(index);
                  parameterValues.Add(parameterValue.Value.ConvertedTo<double>());
                  index++;
               }
            }

            var writer = _container.Resolve<IXmlWriter<ParameterValuesCache>>();
            var newElement = writer.WriteFor(parameterValuesCache, SerializationTransaction.Create());

            var parent = parameterCacheElement.Parent;
            parameterCacheElement.Remove();
            parent.Add(newElement);
         }
      }
   }
}