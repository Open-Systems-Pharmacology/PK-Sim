using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   public class SetRelativeExpressionInSimulationAndNormalizedCommand : PKSimMacroCommand
   {
      private readonly double _newValue;
      private IParameter _parameter;

      public SetRelativeExpressionInSimulationAndNormalizedCommand(IParameter parameter, double value)
      {
         _parameter = parameter;
         _newValue = value;
      }

      public override void Execute(IExecutionContext context)
      {
         //First scale the value. This step in only necessary for the rollback command
         Add(new NormalizeSimulationRelativeExpressionCommand(_parameter, context));

         //Then set the new relative expression command
         Add(new SetRelativeExpressionCommand(_parameter, _newValue));

         //last scale wthe relative expression according to the new value
         Add(new NormalizeSimulationRelativeExpressionCommand(_parameter, context));

         base.Execute(context);

         //clear references
         _parameter = null;
      }
   }
}