using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.ProjectConverter
{
   public interface IIndividualCalculationMethodsUpdater
   {
      void AddMissingCalculationMethodsTo(Individual individual);
      void AddMissingCalculationMethodsTo(Simulation simulation);

      //Adds one calculation method by name, without touching the legacy methods, so it cannot fill a category twice.
      void AddCalculationMethodTo(Individual individual, string calculationMethodName);

      //Adds the model calculation methods the current default defines but the saved simulation is missing.
      void AddMissingModelCalculationMethodsTo(Simulation simulation);
   }

   public class IndividualCalculationMethodsUpdater : IIndividualCalculationMethodsUpdater
   {
      private readonly ICalculationMethodRepository _calculationMethodRepository;
      private readonly IModelPropertiesTask _modelPropertiesTask;

      public IndividualCalculationMethodsUpdater(ICalculationMethodRepository calculationMethodRepository, IModelPropertiesTask modelPropertiesTask)
      {
         _calculationMethodRepository = calculationMethodRepository;
         _modelPropertiesTask = modelPropertiesTask;
      }

      public void AddMissingModelCalculationMethodsTo(Simulation simulation)
      {
         var individual = simulation?.Individual;
         var modelConfiguration = simulation?.ModelProperties?.ModelConfiguration;
         if (individual?.OriginData == null || modelConfiguration == null)
            return;

         var defaultModelProperties = _modelPropertiesTask.DefaultFor(individual.OriginData, modelConfiguration.ModelName);
         var calculationMethodCache = simulation.ModelProperties.CalculationMethodCache;

         defaultModelProperties.AllCalculationMethods()
            .Where(x => calculationMethodCache.CalculationMethodFor(x.Category) == null)
            .Each(calculationMethodCache.AddCalculationMethod);
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