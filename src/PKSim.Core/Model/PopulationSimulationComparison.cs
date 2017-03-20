﻿using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class PopulationSimulationComparison : ObjectBase, ISimulationComparison<PopulationSimulation>, IPopulationDataCollector
   {
      private readonly ICache<string, PopulationSimulation> _allSimulations = new Cache<string, PopulationSimulation>(x => x.Id);
      private readonly IList<ISimulationAnalysis> _allSimulationAnalyses = new List<ISimulationAnalysis>();
      public bool IsLoaded { get; set; }
      public virtual PopulationSimulation ReferenceSimulation { get; set; }
      public virtual GroupingItem ReferenceGroupingItem { get; set; }
      public virtual ParameterDistributionSettingsCache SelectedDistributions { get; private set; }

      public PopulationSimulationComparison()
      {
         SelectedDistributions = new ParameterDistributionSettingsCache();
      }

      public virtual void AddSimulation(PopulationSimulation populationSimulation)
      {
         if (populationSimulation == null) return;
         if (HasSimulation(populationSimulation))
            return;

         _allSimulations.Add(populationSimulation);
      }

      public bool HasReference => ReferenceSimulation != null;

      public virtual IEnumerable<PopulationSimulation> AllSimulations()
      {
         return _allSimulations;
      }

      public virtual bool HasSimulation(PopulationSimulation populationSimulation)
      {
         return _allSimulations.Contains(populationSimulation.Id);
      }

      public virtual void RemoveSimulation(PopulationSimulation populationSimulation)
      {
         if (!HasSimulation(populationSimulation))
            return;

         _allSimulations.Remove(populationSimulation.Id);
      }

      public void RemoveAllSimulations()
      {
         _allSimulations.Clear();
      }

      public virtual void RemoveAnalysis(ISimulationAnalysis simulationAnalysis)
      {
         _allSimulationAnalyses.Remove(simulationAnalysis);
      }

      public virtual IEnumerable<ISimulationAnalysis> Analyses => _allSimulationAnalyses;

      public virtual void AddAnalysis(ISimulationAnalysis simulationAnalysis)
      {
         simulationAnalysis.Analysable = this;
         _allSimulationAnalyses.Add(simulationAnalysis);
      }

      public virtual bool HasUpToDateResults
      {
         get { return _allSimulations.Aggregate(true, (upToDate, s) => upToDate && s.HasUpToDateResults); }
      }

      public virtual IReadOnlyList<SpeciesPopulation> AllRaces()
      {
         return concatenateValues(x => x.AllRaces());
      }

      private IReadOnlyList<T> concatenateValues<T>(Func<PopulationSimulation, IReadOnlyList<T>> simulationValuesRetriever)
      {
         var values = new List<T>();
         _allSimulations.Each(s => values.AddRange(simulationValuesRetriever(s)));
         return values;
      }

      private IReadOnlyList<T> intersectValues<T>(Func<PopulationSimulation, IReadOnlyList<T>> simulationValuesRetriever, IEqualityComparer<T> comparer)
      {
         var seed = new List<T>();
         if (_allSimulations.Count == 0)
            return seed;

         seed.AddRange(simulationValuesRetriever(_allSimulations.First()));

         return _allSimulations.Aggregate((IEnumerable<T>) (seed), (current, s) => current.Intersect(simulationValuesRetriever(s), comparer)).ToList();
      }

      public virtual IReadOnlyList<string> AllCovariateValuesFor(string covariateName)
      {
         return concatenateValues(x => x.AllCovariateValuesFor(covariateName));
      }

      public virtual IReadOnlyList<double> AllPKParameterValuesFor(string quantityPath, string pkParameter)
      {
         return concatenateValues(x => x.AllPKParameterValuesFor(quantityPath, pkParameter));
      }

      public virtual IReadOnlyList<string> AllCovariateNames()
      {
         var covariates = concatenateValues(x => x.AllCovariateNames()).Distinct().ToList();
         covariates.Add(CoreConstants.Covariates.SIMULATION_NAME);
         return covariates;
      }

      public virtual bool DisplayParameterUsingGroupStructure => ComesFromPKSim;

      public virtual IReadOnlyList<double> AllValuesFor(string parameterPath)
      {
         return concatenateValues(x => x.AllValuesFor(parameterPath));
      }

      public virtual IReadOnlyList<double> AllPercentilesFor(string parameterPath)
      {
         return concatenateValues(x => x.AllPercentilesFor(parameterPath));
      }

      public virtual IReadOnlyList<QuantityValues> AllOutputValuesFor(string quantityPath)
      {
         return concatenateValues(x => x.AllOutputValuesFor(quantityPath));
      }

      public virtual int NumberOfItems
      {
         get { return _allSimulations.Sum(x => x.NumberOfItems); }
      }

      public virtual IEnumerable<IParameter> AllVectorialParameters(IEntityPathResolver entityPathResolver)
      {
         return simulation.AllVectorialParameters(entityPathResolver);
      }

      public virtual PathCache<IParameter> AllParameters(IEntityPathResolver entityPathResolver)
      {
         return simulation.AllParameters(entityPathResolver);
      }

      public virtual IParameter ParameterByPath(string parameterPath, IEntityPathResolver entityPathResolver)
      {
         return simulation.ParameterByPath(parameterPath, entityPathResolver);
      }

      public virtual IReadOnlyList<Gender> AllGenders()
      {
         return concatenateValues(x => x.AllGenders());
      }

      private PopulationSimulation simulation => _allSimulations.FirstOrDefault() ?? new PopulationSimulation();

      public virtual bool ComesFromPKSim => simulation.ComesFromPKSim;

      public virtual QuantityPKParameter PKParameterFor(string quantityPath, string pkParameter)
      {
         return simulation.PKParameterFor(quantityPath, pkParameter);
      }

      public virtual IReadOnlyList<QuantityPKParameter> AllPKParametersFor(string quantityPath)
      {
         return intersectValues(x => x.AllPKParametersFor(quantityPath), new QuantityPKParameterComparerByName());
      }

      public virtual bool HasPKParameterFor(string quantityPath, string pkParameter)
      {
         return simulation.HasPKParameterFor(quantityPath, pkParameter);
      }

      public virtual double? MolWeightFor(string quantityPath)
      {
         var molWeights = AllSimulations().Select(x => x.MolWeightFor(quantityPath)).Distinct().ToList();
         if (molWeights.Count != 1)
            return null;

         return molWeights[0];
      }

      public virtual IReadOnlyList<string> AllSimulationNames()
      {
         return concatenateValues(x => x.AllSimulationNames());
      }

      public IReadOnlyList<Compound> Compounds
      {
         get { return _allSimulations.SelectMany(sim => sim.Compounds).ToList(); }
      }

      private class QuantityPKParameterComparerByName : IEqualityComparer<QuantityPKParameter>
      {
         public bool Equals(QuantityPKParameter x, QuantityPKParameter y)
         {
            return Equals(x.Name, y.Name);
         }

         public int GetHashCode(QuantityPKParameter pkParameter)
         {
            return pkParameter.Name.GetHashCode();
         }
      }
   }
}