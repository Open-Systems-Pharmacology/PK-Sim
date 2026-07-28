using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.ProjectConverter
{
   public interface IIndividualCalculationMethodsUpdater
   {
      void AddMissingCalculationMethodsTo(Individual individual);
      void AddMissingCalculationMethodsTo(Simulation simulation);

      /// <summary>
      ///    Adds a single calculation method by name if the individual does not reference it yet. Unlike
      ///    <see cref="AddMissingCalculationMethodsTo(Individual)" /> this does not touch the legacy body-surface-area,
      ///    renal-aging and dynamic-formula methods, so it will not add a second method into a category the individual
      ///    already fills with a different option.
      /// </summary>
      void AddCalculationMethodTo(Individual individual, string calculationMethodName);

      void AddCalculationMethodTo(Simulation simulation, string calculationMethodName);
   }

   public class IndividualCalculationMethodsUpdater : IIndividualCalculationMethodsUpdater
   {
      private readonly ICalculationMethodRepository _calculationMethodRepository;

      public IndividualCalculationMethodsUpdater(ICalculationMethodRepository calculationMethodRepository)
      {
         _calculationMethodRepository = calculationMethodRepository;
      }

      public void AddMissingCalculationMethodsTo(Individual individual)
      {
         if (individual == null)
            return;

         addMissingCalculationMethods(individual.OriginData, individual.IsHuman);
      }

      public void AddMissingCalculationMethodsTo(Simulation simulation)
      {
         var individual = simulation?.BuildingBlock<Individual>();
         if (individual == null)
            return;

         addMissingCalculationMethods(simulation.ModelProperties, individual.IsHuman);
      }

      public void AddCalculationMethodTo(Individual individual, string calculationMethodName)
      {
         if (individual == null)
            return;

         addMissingCalculationMethodTo(individual.OriginData, calculationMethodName);
      }

      public void AddCalculationMethodTo(Simulation simulation, string calculationMethodName)
      {
         if (simulation?.BuildingBlock<Individual>() == null)
            return;

         addMissingCalculationMethodTo(simulation.ModelProperties, calculationMethodName);
      }

      private void addMissingCalculationMethods(IWithCalculationMethods withCalculationMethods, bool isHuman)
      {
         addRenalAgingCalculationMethodTo(withCalculationMethods, isHuman);
         addDynamicFormulaCalculationMethodTo(withCalculationMethods);
         addBSACalculationMethodTo(withCalculationMethods, isHuman);
      }

      private void addBSACalculationMethodTo(IWithCalculationMethods withCalculationMethods, bool isHuman)
      {
         if (!isHuman)
            return;

         addMissingCalculationMethodTo(withCalculationMethods, ConverterConstants.CalculationMethod.BSA_Mosteller);
      }

      private void addDynamicFormulaCalculationMethodTo(IWithCalculationMethods withCalculationMethods)
      {
         addMissingCalculationMethodTo(withCalculationMethods, ConverterConstants.CalculationMethod.DynamicSumFormulas);
      }

      private void addRenalAgingCalculationMethodTo(IWithCalculationMethods withCalculationMethods, bool isHuman)
      {
         var renalAgingCalculationMethodName = isHuman ? CoreConstants.CalculationMethod.RENAL_AGING_HUMAN : CoreConstants.CalculationMethod.RENAL_AGING_ANIMALS;
         addMissingCalculationMethodTo(withCalculationMethods, renalAgingCalculationMethodName);
      }

      private void addMissingCalculationMethodTo(IWithCalculationMethods withCalculationMethods, string calculationMethodName)
      {
         var calculationMethodCache = withCalculationMethods.CalculationMethodCache;
         if (calculationMethodCache.Contains(calculationMethodName))
            return;

         var calculationMethod = _calculationMethodRepository.FindBy(calculationMethodName);
         calculationMethodCache.AddCalculationMethod(calculationMethod);
      }
   }
}