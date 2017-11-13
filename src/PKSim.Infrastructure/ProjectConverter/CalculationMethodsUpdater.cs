using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.ProjectConverter
{
   public interface ICalculationMethodsUpdater
   {
      void AddMissingCalculationMethodsTo(Individual individual);
      void AddMissingCalculationMethodsTo(Simulation simulation);
   }

   public class CalculationMethodsUpdater : ICalculationMethodsUpdater
   {
      private readonly ICalculationMethodRepository _calculationMethodRepository;

      public CalculationMethodsUpdater(ICalculationMethodRepository calculationMethodRepository)
      {
         _calculationMethodRepository = calculationMethodRepository;
      }

      public void AddMissingCalculationMethodsTo(Individual individual)
      {
         if (individual == null)
            return;

         addRenalAgingCalculationMethodTo(individual.OriginData, individual.IsHuman);
         addDynamicFormulaCalculationMethodTo(individual.OriginData);
         addBSACalculationMethodTo(individual.OriginData, individual.IsHuman);
      }

      public void AddMissingCalculationMethodsTo(Simulation simulation)
      {
         var individual = simulation?.BuildingBlock<Individual>();
         if (individual == null)
            return;

         addRenalAgingCalculationMethodTo(simulation.ModelProperties, individual.IsHuman);
      }

      private void addBSACalculationMethodTo(OriginData originData, bool isHuman)
      {
         if (!isHuman)
            return;

         addMissingCalulationMethodTo(originData, ConverterConstants.CalculationMethod.BSA_DuBois);
      }

      private void addDynamicFormulaCalculationMethodTo(OriginData originData)
      {
         addMissingCalulationMethodTo(originData, ConverterConstants.CalculationMethod.DynamicSumFormulas);
      }

      private void addRenalAgingCalculationMethodTo(IWithCalculationMethods withCalculationMethods, bool isHuman)
      {
         var renalAgingCalculationMethodName = isHuman ? CoreConstants.CalculationMethod.RenalAgingHuman : CoreConstants.CalculationMethod.RenalAgingAnimals;
         addMissingCalulationMethodTo(withCalculationMethods, renalAgingCalculationMethodName);
      }

      private void addMissingCalulationMethodTo(IWithCalculationMethods withCalculationMethods, string calculationMethodName)
      {
         var calculationMethodCache = withCalculationMethods.CalculationMethodCache;
         if (calculationMethodCache.Contains(calculationMethodName))
            return;

         var calculationMethod = _calculationMethodRepository.FindBy(calculationMethodName);
         calculationMethodCache.AddCalculationMethod(calculationMethod);
      }
   }
}