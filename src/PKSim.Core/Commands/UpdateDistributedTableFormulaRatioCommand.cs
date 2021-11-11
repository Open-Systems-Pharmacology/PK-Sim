using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class UpdateDistributedTableFormulaRatioCommand : EditParameterCommand
   {
      private readonly double _ratio;

      public UpdateDistributedTableFormulaRatioCommand(IParameter tableParameter, double ratio) : base(tableParameter)
      {
         _ratio = ratio;
      }

      protected override void ExecuteUpdateParameter(IParameter parameter, IExecutionContext context)
      {
         UpdateParameter(context);
         Description = ParameterMessages.UpdateTableParameterFormula(context.DisplayNameFor(parameter));
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new UpdateDistributedTableFormulaRatioCommand(_parameter, 1 / _ratio).AsInverseFor(this);
      }

      protected override void UpdateParameter(IParameter parameter, IExecutionContext context)
      {
         if (parameter == null) return;

         var distributedParameter = parameter as IDistributedParameter;
         
         //this is the template parameter
         if (distributedParameter != null)
            distributedParameter.Value *= _ratio;
         else
         {
            var tableFormula = parameter.Formula.DowncastTo<DistributedTableFormula>();
            tableFormula.AllPoints().Each(p => p.Y *= _ratio);
         }
      }
   }
}