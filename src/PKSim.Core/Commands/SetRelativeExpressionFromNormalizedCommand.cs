using System.Linq;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class SetRelativeExpressionFromNormalizedCommand : PKSimMacroCommand
   {
      private readonly double _newNormalizedValue;
      private IParameter _normalizedParameter;

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

         var relativeExpressionValue = getRelativeExpressionValue(_newNormalizedValue, relativeExpressionParameter);

         var setRelativeExpressionCommand = new SetRelativeExpressionCommand(relativeExpressionParameter, relativeExpressionValue)
         {
            Visible = false
         };

         Add(setRelativeExpressionCommand);
         Add(new SetParameterValueCommand(_normalizedParameter, _newNormalizedValue));

         base.Execute(context);

         //clear references
         _normalizedParameter = null;
      }

      private double getRelativeExpressionValue(double newNormalizedValue, IParameter relativeExpressionParameter)
      {
         return relativeExpressionParameter.AllRelatedRelativeExpressionParameters().Where(x => !x.IsExpressionNorm()).Max(x => x.Value) * newNormalizedValue;
      }

      private IParameter getRelativeExpressionFrom(IParameter normalizedParameter)
      {
         var nonNormalizedParameterName = normalizedParameter.Name.Replace(CoreConstants.Parameter.NORM_SUFFIX, string.Empty);
         return normalizedParameter.ParentContainer.Parameter(nonNormalizedParameterName);
      }
   }
}