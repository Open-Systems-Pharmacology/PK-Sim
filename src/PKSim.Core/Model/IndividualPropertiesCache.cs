using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Populations;

namespace PKSim.Core.Model
{
   public class IndividualPropertiesCache : IParameterValueCache
   {
      //covariates names
      //gender, race, populationName 
      public List<IndividualCovariates> AllCovariates { get; }

      public ParameterValuesCache ParameterValuesCache { get; }

      public IndividualPropertiesCache() : this(new ParameterValuesCache(), new List<IndividualCovariates>())
      {
      }

      public IndividualPropertiesCache(ParameterValuesCache parameterValuesCache, IEnumerable<IndividualCovariates> allCovariates)
      {
         ParameterValuesCache = parameterValuesCache;
         AllCovariates = new List<IndividualCovariates>(allCovariates);
      }

      public void Add(IndividualProperties individualProperties)
      {
         AllCovariates.Add(individualProperties.Covariates);
         Add(individualProperties.ParameterValues.ToList());
      }

      public virtual bool Has(string parameterPath)
      {
         return ParameterValuesCache.Has(parameterPath);
      }

      public virtual IReadOnlyList<double> ValuesFor(string parameterPath)
      {
         return ParameterValuesCache.ValuesFor(parameterPath);
      }

      public virtual IReadOnlyList<double> PercentilesFor(string parameterPath)
      {
         return ParameterValuesCache.PercentilesFor(parameterPath);
      }

      /// <summary>
      ///    Returns the number of individuals
      /// </summary>
      public virtual int Count => ParameterValuesCache.Count;

      public virtual IReadOnlyList<Gender> Genders
      {
         get { return AllCovariates.Where(x => x.Gender != null).Select(x => x.Gender).ToList(); }
      }

      public virtual IReadOnlyList<SpeciesPopulation> Races
      {
         get { return AllCovariates.Where(x => x.Race != null).Select(x => x.Race).ToList(); }
      }

      public virtual string[] AllParameterPaths()
      {
         return ParameterValuesCache.AllParameterPaths();
      }

      public double[] GetValues(string parameterPath)
      {
         return ParameterValuesCache.GetValues(parameterPath);
      }

      public virtual IndividualPropertiesCache Clone()
      {
         return new IndividualPropertiesCache(ParameterValuesCache.Clone(), AllCovariates);
      }

      public virtual void Remove(string parameterPath)
      {
         ParameterValuesCache.Remove(parameterPath);
      }

      public virtual void SetValues(string parameterPath, IReadOnlyList<RandomValue> newValues)
      {
         ParameterValuesCache.SetValues(parameterPath, newValues);
      }

      public virtual void SetValues(string parameterPath, IReadOnlyList<double> newValues)
      {
         ParameterValuesCache.SetValues(parameterPath, newValues);
      }

      public virtual IReadOnlyCollection<ParameterValues> AllParameterValues => ParameterValuesCache.AllParameterValues;

      public virtual ParameterValues ParameterValuesFor(string parameterPath)
      {
         return ParameterValuesCache.ParameterValuesFor(parameterPath);
      }

      public virtual ParameterValues ParameterValuesAt(int index)
      {
         return ParameterValuesCache.ParameterValuesAt(index);
      }

      public virtual void Add(ParameterValues parameterValues)
      {
         ParameterValuesCache.Add(parameterValues);
      }

      public virtual void Add(IReadOnlyCollection<ParameterValue> parameterValues)
      {
         ParameterValuesCache.Add(parameterValues);
      }

      public virtual void RenamePath(string oldPath, string newPath)
      {
         ParameterValuesCache.RenamePath(oldPath, newPath);
      }

      public virtual void AddPopulations(IReadOnlyList<SpeciesPopulation> speciesPopulations)
      {
         addCovariates(CoreConstants.Covariates.RACE, speciesPopulations);
      }

      public virtual void AddGenders(IReadOnlyList<Gender> genders)
      {
         addCovariates(CoreConstants.Covariates.GENDER, genders);
      }

      public virtual void AddConvariate(string covariate, IReadOnlyList<string> values)
      {
         addCovariates(covariate, values);
      }

      private void addCovariates<T>(string covariate, IReadOnlyList<T> values)
      {
         addCovariatesIfRequired(values.Count);
         for (int i = 0; i < values.Count; i++)
         {
            AllCovariates[i].AddCovariate(covariate, values[i]);
         }
      }

      private void addCovariatesIfRequired(int numberOfItems)
      {
         for (int i = AllCovariates.Count; i < numberOfItems; i++)
         {
            AllCovariates.Add(new IndividualCovariates());
         }
      }

      public virtual void Merge(IndividualPropertiesCache individualPropertiesCache, PathCache<IParameter> parameterCache)
      {
         AllCovariates.AddRange(individualPropertiesCache.AllCovariates);
         ParameterValuesCache.Merge(individualPropertiesCache.ParameterValuesCache, parameterCache);
      }

      public IReadOnlyList<string> AllCovariatesNames()
      {
         var covariates = new List<string>();
         if (Genders.Count > 0)
            covariates.Add(CoreConstants.Covariates.GENDER);

         if (Races.Count > 0)
            covariates.Add(CoreConstants.Covariates.RACE);

         covariates.AddRange(AllCovariates.SelectMany(x => x.Attributes.Keys).Distinct());
         return covariates;
      }
   }
}