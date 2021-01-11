using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class SetParameterPercentileCommand : EditParameterCommand
   {
      private readonly double _percentile;
      private double _oldPercentile;

      public SetParameterPercentileCommand(IDistributedParameter parameter, double percentile) : base(parameter)
      {
         _percentile = percentile;
      }

      private IDistributedParameter distributedParameter => _parameter.DowncastTo<IDistributedParameter>();

      protected override void ExecuteUpdateParameter(IExecutionContext context)
      {
         _oldPercentile = distributedParameter.Percentile;
         UpdateParameter(context);
         Description = ParameterMessages.SetParameterPercentile(context.DisplayNameFor(_parameter), _oldPercentile, _percentile);
      }

      protected override void UpdateParameter(IParameter parameter, IExecutionContext context)
      {
         var distParameter = parameter as IDistributedParameter;
         if (distParameter == null) return;
         distributedParameter.Percentile = _percentile;
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetParameterPercentileCommand(distributedParameter, _oldPercentile).AsInverseFor(this);
      }
   }
}