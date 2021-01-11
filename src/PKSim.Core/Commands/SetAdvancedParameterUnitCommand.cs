using OSPSuite.Core.Commands.Core;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Commands
{
   public class SetAdvancedParameterUnitCommand : SetParameterUnitCommand
   {
      public SetAdvancedParameterUnitCommand(IParameter parameter, Unit newDisplayUnit)
         : base(parameter, newDisplayUnit)
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
         return new SetAdvancedParameterUnitCommand(_parameter, _oldDisplayUnit).AsInverseFor(this);
      }
   }
}