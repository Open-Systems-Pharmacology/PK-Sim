using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   public class UpdateDistributedTableFormulaRatioCommand : EditParameterCommand
   {
      private readonly double _ratio;

      public UpdateDistributedTableFormulaRatioCommand(IParameter tableParameter, double ratio) : base(tableParameter)
      {
         this._ratio = ratio;
      }

      protected override void ExecuteUpdateParameter(IExecutionContext context)
      {
         var bbParameter = OriginParameterFor(_parameter, context);
         UpdateParameter(_parameter, context);
         UpdateParameter(bbParameter, context);
         Description = ParameterMessages.UpdateTableParameterFormula(context.DisplayNameFor(_parameter));
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
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