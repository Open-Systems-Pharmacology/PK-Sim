using OSPSuite.Core.Commands.Core;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   public class SetAdvancedParameterValueCommand : SetParameterValueCommand
   {
      public SetAdvancedParameterValueCommand(IParameter parameter, double valueToSet)
         : base(parameter, valueToSet)
      {
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         base.PerformExecuteWith(context);
         var advancedParameterUpdater = context.Resolve<IAdvancedParameterInPopulationUpdater>();
         advancedParameterUpdater.UpdatePopulationContaining(_parameter);
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetAdvancedParameterValueCommand(_parameter, _oldValue).AsInverseFor(this);
      }
   }
}