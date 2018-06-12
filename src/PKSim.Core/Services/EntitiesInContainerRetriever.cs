using System;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Services
{
   public interface IEntitiesInContainerRetriever : IEntitiesInSimulationRetriever
   {
      /// <summary>
      ///    Returns all <see cref="IQuantity" /> that were selected when calculating the
      ///    <paramref name="populationSimulation" />  (e.g.
      ///    Persistable=true)
      /// </summary>
      PathCache<IQuantity> OutputsFrom(PopulationSimulation populationSimulation);

      /// <summary>
      ///    Returns all <see cref="IQuantity" /> that were selected when calculating the
      ///    <paramref name="populationDataCollector" />  (e.g.
      ///    Persistable=true). The search is performed using the intersectio nof all quantities by path
      /// </summary>
      PathCache<IQuantity> OutputsFrom(IPopulationDataCollector populationDataCollector);

      /// <summary>
      ///    Returns all <see cref="IQuantity" /> defined under the <paramref name="populationSimulation" />  (search performed
      ///    using
      ///    complete hierarchy)
      /// </summary>
      PathCache<IQuantity> QuantitiesFrom(PopulationSimulation populationSimulation);

      /// <summary>
      ///    Returns all <see cref="IQuantity" /> defined under the <paramref name="populationDataCollector" />  (search
      ///    performed using
      ///    intersection of all quantities by path)
      /// </summary>
      PathCache<IQuantity> QuantitiesFrom(IPopulationDataCollector populationDataCollector);

      /// <summary>
      ///    Returns all <see cref="IParameter" /> defined in the <paramref name="populationSimulation" /> (search performed
      ///    using
      ///    complete hierarchy)
      /// </summary>
      PathCache<IParameter> ParametersFrom(PopulationSimulation populationSimulation);

      /// <summary>
      ///    Returns all <see cref="IParameter" /> defined in the <paramref name="populationDataCollector" /> (search
      ///    performed using
      ///    complete hierarchy and results intersected using parameter path)
      /// </summary>
      PathCache<IParameter> ParametersFrom(IPopulationDataCollector populationDataCollector);
   }

   public class EntitiesInContainerRetriever : EntitiesInSimulationRetriever, IEntitiesInContainerRetriever
   {
      public EntitiesInContainerRetriever(IEntityPathResolver entityPathResolver, IContainerTask containerTask) : base(entityPathResolver, containerTask)
      {
      }

      public PathCache<IQuantity> OutputsFrom(PopulationSimulation populationSimulation)
      {
         return OutputsFrom(populationSimulation.DowncastTo<Simulation>());
      }

      public PathCache<IQuantity> QuantitiesFrom(PopulationSimulation populationSimulation)
      {
         return QuantitiesFrom(populationSimulation.DowncastTo<Simulation>());
      }

      public PathCache<IParameter> ParametersFrom(PopulationSimulation populationSimulation)
      {
         return ParametersFrom(populationSimulation.DowncastTo<ISimulation>());
      }

      public PathCache<IQuantity> QuantitiesFrom(IPopulationDataCollector populationDataCollector)
      {
         return intersect(populationDataCollector, QuantitiesFrom);
      }

      public PathCache<IQuantity> OutputsFrom(IPopulationDataCollector populationDataCollector)
      {
         return intersect(populationDataCollector, OutputsFrom);
      }

      public PathCache<IParameter> ParametersFrom(IPopulationDataCollector populationDataCollector)
      {
         return intersect(populationDataCollector, ParametersFrom);
      }

      private PathCache<T> intersect<T>(IPopulationDataCollector populationDataCollector, Func<PopulationSimulation, PathCache<T>> dataCollector) where T : class, IEntity
      {
         var populationSimulation = populationDataCollector as PopulationSimulation;
         if (populationSimulation != null)
            return dataCollector(populationSimulation);

         var comparison = populationDataCollector.DowncastTo<PopulationSimulationComparison>();

         var firstSimulation = comparison.AllSimulations.FirstOrDefault();
         if (firstSimulation == null)
            return new PathCache<T>(_entityPathResolver);

         var pathIntersect = dataCollector(firstSimulation);
         var allCommonPaths = comparison.AllSimulations.Aggregate(pathIntersect.Keys, (current, simulation) => current.Intersect(dataCollector(simulation).Keys)).ToList();

         foreach (var availalbePath in pathIntersect.Keys.ToList())
         {
            if (!allCommonPaths.Contains(availalbePath))
               pathIntersect.Remove(availalbePath);
         }

         return pathIntersect;
      }
   }
}