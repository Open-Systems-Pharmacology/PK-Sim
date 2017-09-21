using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Maths.Random;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Model
{
   public class PopulationSimulation : Simulation, IPopulationDataCollector, IAdvancedParameterContainer
   {
      public virtual PopulationSimulationPKAnalyses PKAnalyses { get; set; }

      /// <summary>
      ///    All the distributed parameters of the population simulation. This does not contain the distributed parameter of the
      ///    underlying population
      /// </summary>
      public virtual ParameterValuesCache ParameterValuesCache { get; private set; }

      public virtual AgingData AgingData { get; private set; }

      public PopulationSimulation()
      {
         ParameterValuesCache = new ParameterValuesCache();
         AgingData = new AgingData();
         PKAnalyses = new NullPopulationSimulationPKAnalyses();
      }

      /// <summary>
      ///    Add the TableFormula representing the aging data for the individual with index <paramref name="individualIndex" />
      ///    and the parameter
      ///    with path <paramref name="parameterPath" />.
      /// </summary>
      /// <param name="parameterPath">Parameter full path</param>
      /// <param name="individualIndex">Index of individual</param>
      /// <param name="tableFormula">TableFormula representing the growth function for the parameter</param>
      public virtual void AddAgingTableFormula(string parameterPath, int individualIndex, TableFormula tableFormula)
      {
         foreach (var point in tableFormula.AllPoints())
         {
            AgingData.Add(individualIndex, parameterPath, point.X, point.Y);
         }
      }

      public virtual RandomGenerator RandomGenerator => Population.RandomGenerator;

      public override TBuildingBlock BuildingBlock<TBuildingBlock>()
      {
         if (typeof (TBuildingBlock).IsAnImplementationOf<Individual>())
            return Population.FirstIndividual as TBuildingBlock;

         return AllBuildingBlocks<TBuildingBlock>().SingleOrDefault();
      }

      /// <summary>
      ///    Returns all parameters that could potentially be defined as advanced parameters in a simulation 
      /// </summary>
      public virtual IEnumerable<IParameter> AllPotentialAdvancedParameters => ParametersOfType(PKSimBuildingBlockType.Simulation | PKSimBuildingBlockType.Compound | PKSimBuildingBlockType.Event | PKSimBuildingBlockType.Formulation);

      /// <summary>
      ///    Returns all values defined for the organism parameter names <paramref name="parameterName" />
      /// </summary>
      public virtual IReadOnlyList<double> AllOrganismValuesFor(string parameterName, IEntityPathResolver entityPathResolver)
      {
         var parameterPath = entityPathResolver.PathFor(Individual.Organism.Parameter(parameterName));
         return AllValuesFor(parameterPath);
      }

      public virtual void AddAdvancedParameter(AdvancedParameter advancedParameter, bool generateRandomValues = true)
      {
         advancedParameterCollection.AddAdvancedParameter(advancedParameter);
         //force generation of parameters if no values was defined in the cache
         if (generateRandomValues || !ParameterValuesCache.Has(advancedParameter.ParameterPath))
            GenerateRandomValuesFor(advancedParameter);
      }

      public virtual void RemoveAdvancedParameter(AdvancedParameter advancedParameter)
      {
         advancedParameterCollection.RemoveAdvancedParameter(advancedParameter);
         ParameterValuesCache.Remove(advancedParameter.ParameterPath);
      }

      public virtual void GenerateRandomValuesFor(AdvancedParameter advancedParameter)
      {
         ParameterValuesCache.SetValues(advancedParameter.ParameterPath, advancedParameter.GenerateRandomValues(NumberOfItems));
      }

      public virtual IEnumerable<IParameter> AllAdvancedParameters(IEntityPathResolver entityPathResolver)
      {
         var allParameters = new PathCache<IParameter>(entityPathResolver).For(All<IParameter>());
         //The path might not be defined in the model when importing parameters. Simply ignore them
         return ParameterValuesCache.AllParameterPaths().Select(p => allParameters[p]).Where(p => p != null);
      }

      public IEnumerable<IParameter> AllConstantParameters(IEntityPathResolver entityPathResolver)
      {
         var allParameters = AllPotentialAdvancedParameters.ToList();
         AllAdvancedParameters(entityPathResolver).Each(p => allParameters.Remove(p));
         return allParameters;
      }

      public virtual void SetAdvancedParameters(AdvancedParameterCollection advancedParameters)
      {
         Add(advancedParameters);
      }

      private AdvancedParameterCollection advancedParameterCollection
      {
         get { return this.GetSingleChild<AdvancedParameterCollection>(x => true); }
      }

      public virtual IReadOnlyList<double> AllValuesFor(string parameterPath)
      {
         if (ParameterValuesCache.Has(parameterPath))
            return ParameterValuesCache.ValuesFor(parameterPath);

         return Population.AllValuesFor(parameterPath);
      }

      public virtual IReadOnlyList<double> AllPercentilesFor(string parameterPath)
      {
         if (ParameterValuesCache.Has(parameterPath))
            return ParameterValuesCache.PercentilesFor(parameterPath);

         return Population.AllPercentilesFor(parameterPath);
      }

      public virtual IReadOnlyList<QuantityValues> AllOutputValuesFor(string quantityPath)
      {
         if (Results.IsNull())
            return missingQuantityValuesArray();

         var allValuesForPath = Results.AllValuesFor(quantityPath);
         //we might not have the right number or the values might not exist for the path
         if (allValuesForPath.Count == NumberOfItems && allValuesForPath.All(x => x != null))
            return allValuesForPath;

         //In that case, the population size was reduced, does not make sense to keep any value
         if (NumberOfItems < allValuesForPath.Count)
            return missingQuantityValuesArray();

         return patchedUpResults(quantityPath);
      }

      //some values are missing. Patch the results up_with NullQuantityValues
      private IReadOnlyList<QuantityValues> patchedUpResults(string quantityPath)
      {
         var values = new List<QuantityValues>(missingQuantityValuesArray());
         var allExistingResults = Results.AllIndividualResults.Select(x => new {x.IndividualId, Values = x.ValuesFor(quantityPath)});
         allExistingResults.Each(existingValue => values[existingValue.IndividualId] = existingValue.Values);
         return values;
      }

      public virtual IReadOnlyList<double> AllPKParameterValuesFor(string quantityPath, string pkParameter)
      {
         var pk = PKAnalyses.PKParameterFor(quantityPath, pkParameter);
         if (pk == null)
            return missingDoubleValuesArray();

         var allValues = pk.Values.ToDoubleArray();
         if (allValues.Length == NumberOfItems)
            return allValues;

         return missingDoubleValuesArray();
      }

      private double[] missingDoubleValuesArray()
      {
         return missingValuesArray(double.NaN);
      }

      private IReadOnlyList<QuantityValues> missingQuantityValuesArray()
      {
         var missingQuantityValues = new NullQuantityValues();
         return missingValuesArray(missingQuantityValues);
      }

      private T[] missingValuesArray<T>(T defaultValue)
      {
         return new T[NumberOfItems].InitializeWith(defaultValue);
      }

      public virtual QuantityPKParameter PKParameterFor(string quantityPath, string pkParameter)
      {
         return PKAnalyses.PKParameterFor(quantityPath, pkParameter);
      }

      public virtual IReadOnlyList<QuantityPKParameter> AllPKParametersFor(string quantityPath)
      {
         return PKAnalyses.AllPKParametersFor(quantityPath);
      }

      public virtual bool HasPKParameterFor(string quantityPath, string pkParameter)
      {
         return PKAnalyses.HasPKParameterFor(quantityPath, pkParameter);
      }

      public double? MolWeightFor(string quantityPath)
      {
         var objectPath = new ObjectPath(quantityPath.ToPathArray());
         var quantity = objectPath.TryResolve<IQuantity>(Model.Root, out bool found);
         if (!found)
            return null;

         return Model.MolWeightFor(quantity);
      }

      public virtual IReadOnlyList<string> AllSimulationNames => new string[NumberOfItems].InitializeWith(Name);

      public virtual int NumberOfItems => Population.NumberOfItems;

      public virtual ParameterDistributionSettingsCache SelectedDistributions => Population.SelectedDistributions;

      public virtual IEnumerable<IParameter> AllVectorialParameters(IEntityPathResolver entityPathResolver)
      {
         var allParameters = new PathCache<IParameter>(entityPathResolver).For(All<IParameter>());
         return ParameterValuesCache.AllParameterPaths().Select(p => allParameters[p])
            .Union(Population.AllVectorialParameters(entityPathResolver)).Where(p => p != null);
      }

      public virtual PathCache<IParameter> AllParameters(IEntityPathResolver entityPathResolver)
      {
         var allParameters = new PathCache<IParameter>(entityPathResolver).For(AllPotentialAdvancedParameters);
         foreach (var populationParameters in Population.AllParameters(entityPathResolver).KeyValues)
         {
            if (!allParameters.Contains(populationParameters.Key))
               allParameters.Add(populationParameters.Key, populationParameters.Value);
         }
         return allParameters;
      }

      public virtual IParameter ParameterByPath(string parameterPath, IEntityPathResolver entityPathResolver)
      {
         // NOTE: Change implementation if time consuming
         return AllParameters(entityPathResolver)[parameterPath];
      }

      public virtual AdvancedParameter AdvancedParameterFor(IEntityPathResolver entityPathResolver, IParameter parameter)
      {
         return advancedParameterCollection.AdvancedParameterFor(entityPathResolver, parameter);
      }

      public virtual void RemoveAllAdvancedParameters() => advancedParameterCollection.Clear();

      public virtual IEnumerable<AdvancedParameter> AdvancedParameters => advancedParameterCollection.AdvancedParameters;

      public virtual IReadOnlyList<Gender> AllGenders => Population.AllGenders;

      public virtual IReadOnlyList<SpeciesPopulation> AllRaces => Population.AllRaces;

      public virtual IReadOnlyList<string> AllCovariateValuesFor(string covariateName)
      {
         if (string.Equals(covariateName, CoreConstants.Covariates.SIMULATION_NAME))
            return AllSimulationNames;

         return Population.AllCovariateValuesFor(covariateName);
      }

      public virtual IReadOnlyList<string> AllCovariateNames => Population.AllCovariateNames;

      public bool DisplayParameterUsingGroupStructure => ComesFromPKSim;

      public virtual Population Population => BuildingBlock<Population>();

      public virtual bool HasPKAnalyses => !PKAnalyses.IsNull();

      public override bool HasResults => !Results.IsNull() && Results.Any();

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourcePopSimulation = sourceObject as PopulationSimulation;
         if (sourcePopSimulation == null) return;
         ParameterValuesCache = sourcePopSimulation.ParameterValuesCache.Clone();
         //do not clone Aging Data as this will be updated according to the population selection
      }

      public override void UpdateFromOriginalSimulation(Simulation originalSimulation)
      {
         base.UpdateFromOriginalSimulation(originalSimulation);
         var sourcePopSimulation = originalSimulation as PopulationSimulation;
         sourcePopSimulation?.AdvancedParameters.Each(x => AddAdvancedParameter(x, generateRandomValues: true));
      }
   }
}