using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.ProjectConverter
{
   public interface IRenalAgingCalculationMethodUpdater
   {
      void AddRenalAgingCalculationMethodTo(Individual individual);
      void AddRenalAgingCalculationMethodTo(Simulation simulation);
   }

   public class RenalAgingCalculationMethodUpdater : IRenalAgingCalculationMethodUpdater
   {
      private readonly ICalculationMethodRepository _calculationMethodRepository;

      public RenalAgingCalculationMethodUpdater(ICalculationMethodRepository calculationMethodRepository)
      {
         _calculationMethodRepository = calculationMethodRepository;
      }

      public void AddRenalAgingCalculationMethodTo(Individual individual)
      {
         if (individual == null)
            return;

         addRenalAgingCalculationMethodTo(individual.OriginData, individual.IsHuman);
      }

      public void AddRenalAgingCalculationMethodTo(Simulation simulation)
      {
         var individual = simulation?.BuildingBlock<Individual>();
         if (individual == null)
            return;

         addRenalAgingCalculationMethodTo(simulation.ModelProperties, individual.IsHuman);
      }

      private void addRenalAgingCalculationMethodTo(IWithCalculationMethods withCalculationMethods, bool isHuman)
      {
         var renalAgingCalcMethodName = isHuman ? CoreConstants.CalculationMethod.RenalAgingHuman : CoreConstants.CalculationMethod.RenalAgingAnimals;

         var calculationMethodCache = withCalculationMethods.CalculationMethodCache;
         if (!calculationMethodCache.Contains(renalAgingCalcMethodName))
         {
            var calcMethod = _calculationMethodRepository.FindBy(renalAgingCalcMethodName);
            calculationMethodCache.AddCalculationMethod(calcMethod);
         }
      }
   }
}