using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.ProjectConverter
{
   public interface IIndividualCalculationMethodsUpdater
   {
      void AddMissingCalculationMethodsTo(Individual individual);
      void AddMissingCalculationMethodsTo(Simulation simulation);
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
         var renalAgingCalculationMethodName = isHuman ? CoreConstants.CalculationMethod.RenalAgingHuman : CoreConstants.CalculationMethod.RenalAgingAnimals;
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