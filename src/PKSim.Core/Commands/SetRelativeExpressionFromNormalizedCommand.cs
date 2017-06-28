using System.Linq;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class SetRelativeExpressionFromNormalizedCommand : PKSimMacroCommand
   {
      private readonly double _newNormalizedValue;
      private readonly IParameter _normalizedParameter;

      public SetRelativeExpressionFromNormalizedCommand(IParameter normalizedParameter, double normalizedValue)
      {
         _normalizedParameter = normalizedParameter;
         _newNormalizedValue = normalizedValue;
      }

      public override void Execute(IExecutionContext context)
      {
         var relativeExpressionParameter = getRelativeExpressionFrom(_normalizedParameter);

         if (relativeExpressionParameter == null)
            return;

         var relativeExpressionValue = getRelativeExpressionValue(_normalizedParameter, _newNormalizedValue, relativeExpressionParameter);

         Add(new SetRelativeExpressionCommand(relativeExpressionParameter, relativeExpressionValue));
         Add(new SetParameterValueCommand(_normalizedParameter, _newNormalizedValue));

         base.Execute(context);
      }

      private double getRelativeExpressionValue(IParameter normalizedParameter, double newNormalizedValue, IParameter relativeExpressionParameter)
      {
         return relativeExpressionParameter.Value * newNormalizedValue / normalizedParameter.Value;
      }

      private IParameter getRelativeExpressionFrom(IParameter normalizedParameter)
      {
         return normalizedParameter.ParentContainer.GetAllChildren<IParameter>().FirstOrDefault(isNonNormalizedRelativeExpression);
      }

      private static bool isNonNormalizedRelativeExpression(IParameter parameter)
      {
         return isRelativeExpression(parameter) && !parameter.IsExpressionNorm() && !isExpressionOut(parameter);
      }

      private static bool isExpressionOut(IParameter parameter)
      {
         return string.Equals(parameter.Name, CoreConstants.Parameter.RelExpOut);
      }

      private static bool isRelativeExpression(IParameter parameter)
      {
         return string.Equals(parameter.GroupName, CoreConstants.Groups.RELATIVE_EXPRESSION) && parameter.Name.StartsWith(CoreConstants.Parameter.RelExp);
      }
   }
}