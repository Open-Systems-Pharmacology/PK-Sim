using OSPSuite.Core.Commands.Core;

namespace PKSim.Core
{
   public static class CommandHelperForSpecs
   {
      public static ICommand<IExecutionContext> ExecuteAndInvokeInverse(this IReversibleCommand<IExecutionContext> command, IExecutionContext context)
      {
         command.Execute(context);
         return command.InvokeInverse(context);
      }

      public static ICommand<IExecutionContext> InvokeInverse(this IReversibleCommand<IExecutionContext> command, IExecutionContext context)
      {
         command.RestoreExecutionData(context);
         var inverse = command.InverseCommand(context);
         inverse.Execute(context);
         return inverse;
      }
   }
}
