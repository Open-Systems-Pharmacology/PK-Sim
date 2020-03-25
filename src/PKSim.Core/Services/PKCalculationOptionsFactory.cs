using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Services
{
   public interface IPKCalculationOptionsFactory : OSPSuite.Core.Domain.Services.IPKCalculationOptionsFactory
   {
      PKCalculationOptions CreateFor(Simulation simulation, string moleculeName);

      /// <summary>
      ///    Creates the <see cref="PKCalculationOptions" /> based on the given <paramref name="populationSimulation" /> and
      ///    ensure that the dose is set to <c>null</c>
      /// </summary>
      PKCalculationOptions CreateFor(PopulationSimulation populationSimulation, string moleculeName);

      PKCalculationOptions CreateFor(IPopulationDataCollector populationDataCollector, string compoundName);
      PKCalculationOptions CreateForObservedData(IReadOnlyList<Simulation> simulations, string moleculeName);
   }

   public class PKCalculationOptionsFactory : OSPSuite.Core.Domain.Services.PKCalculationOptionsFactory, IPKCalculationOptionsFactory
   {
      private readonly ILazyLoadTask _lazyLoadTask;

      public PKCalculationOptionsFactory(ILazyLoadTask lazyLoadTask)
      {
         _lazyLoadTask = lazyLoadTask;
      }

      public PKCalculationOptions CreateFor(Simulation simulation, string moleculeName)
      {
         _lazyLoadTask.LoadResults(simulation);

         return base.CreateFor(simulation, moleculeName);
      }

      public PKCalculationOptions CreateFor(PopulationSimulation populationSimulation, string moleculeName)
      {
         var options = CreateFor((Simulation) populationSimulation, moleculeName);
         options.TotalDrugMassPerBodyWeight = null;
         return options;
      }

      public PKCalculationOptions CreateForObservedData(IReadOnlyList<Simulation> simulations, string moleculeName)
      {
         var allDosesForMolecules = simulations.Select(s =>
         {
            _lazyLoadTask.Load(s);
            return s.TotalDrugMassPerBodyWeightFor(moleculeName);
         }).Distinct().ToList();

         return new PKCalculationOptions
         {
            TotalDrugMassPerBodyWeight = allDosesForMolecules.Count == 1 ? allDosesForMolecules[0] : null
         };
      }

      public PKCalculationOptions CreateFor(IPopulationDataCollector populationDataCollector, string compoundName)
      {
         var simulation = populationDataCollector as PopulationSimulation;
         return simulation != null ? CreateFor(simulation, compoundName) : new PKCalculationOptions();
      }

      public override void UpdateTotalDrugMassPerBodyWeight(IModelCoreSimulation simulation, string moleculeName, PKCalculationOptions options, IReadOnlyList<ApplicationParameters> allApplicationParametersOrderedByStartTime)
      {
         base.UpdateTotalDrugMassPerBodyWeight(simulation, moleculeName, options, allApplicationParametersOrderedByStartTime);

         if (options.SingleDosing)
            return;

         //we have at least 2 applied applications at that stage since we are in multiple dosing mode
         var bodyWeight = simulation.DowncastTo<Simulation>().BodyWeight?.Value;
         var applicationCount = allApplicationParametersOrderedByStartTime.Count;
         options.FirstInterval.DrugMassPerBodyWeight = drugMassPerBodyWeightFor(allApplicationParametersOrderedByStartTime[0].DrugMass, bodyWeight);
         options.LastMinusOneInterval.DrugMassPerBodyWeight = drugMassPerBodyWeightFor(allApplicationParametersOrderedByStartTime[applicationCount - 2].DrugMass, bodyWeight);
         options.LastInterval.DrugMassPerBodyWeight = drugMassPerBodyWeightFor(allApplicationParametersOrderedByStartTime[applicationCount - 1].DrugMass, bodyWeight);
      }

      private double? drugMassPerBodyWeightFor(IParameter drugMass, double? bodyWeight)
      {
         if (drugMass ==null || bodyWeight == null || double.IsNaN(bodyWeight.Value))
            return null;

         return drugMass.Value / bodyWeight.Value;
      }
   }
}