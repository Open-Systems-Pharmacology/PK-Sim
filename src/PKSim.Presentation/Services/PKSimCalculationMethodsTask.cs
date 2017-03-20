using OSPSuite.Core.Commands.Core;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.Services
{
   public interface IPKSimCalculationMethodsTask
   {
      IPKSimCommand SetCalculationMethod<T>(T objectWithCalculationMethods, string category, CalculationMethod newCalculationMethod, CalculationMethod oldCalculationMethod) 
         where T : class, IWithCalculationMethods, IPKSimBuildingBlock;
   }

   public class PKSimCalculationMethodsTask : IPKSimCalculationMethodsTask
   {
      private readonly IExecutionContext _context;

      public PKSimCalculationMethodsTask(IExecutionContext context)
      {
         _context = context;
      }

      public IPKSimCommand SetCalculationMethod<T>(T objectWithCalculationMethods, string category, CalculationMethod newCalculationMethod, CalculationMethod oldCalculationMethod) 
         where T : class, IWithCalculationMethods, IPKSimBuildingBlock
      {
         return getSetCalculationMethodCommand(objectWithCalculationMethods, category, newCalculationMethod, oldCalculationMethod).Run(_context);
      }

      private IPKSimCommand getSetCalculationMethodCommand<T>(T objectWithCalculationMethods, string category, CalculationMethod newCalculationMethod, CalculationMethod oldCalculationMethod) 
         where T : class, IWithCalculationMethods, IPKSimBuildingBlock
      {
         return new SetCalculationMethodCommand<T>(objectWithCalculationMethods, category, newCalculationMethod, oldCalculationMethod);
      }
   }
}
