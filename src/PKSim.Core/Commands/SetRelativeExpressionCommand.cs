using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   public class SetRelativeExpressionCommand : SetParameterValueCommand
   {
      public SetRelativeExpressionCommand(IParameter parameter, double valueToSet)
         : base(parameter, valueToSet)
      {
         ObjectType = PKSimConstants.ObjectTypes.Molecule;
      }

      protected override void UpdateParameter(IParameter parameter, IExecutionContext context)
      {
         base.UpdateParameter(parameter, context);
         if (parameter == null) return;
         parameter.IsDefault = (parameter.Value == 0);
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetRelativeExpressionCommand(_parameter, _oldValue).AsInverseFor(this);
      }
   }
}