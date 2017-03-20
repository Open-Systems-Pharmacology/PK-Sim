using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

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

      private IDistributedParameter distributedParameter
      {
         get { return _parameter.DowncastTo<IDistributedParameter>(); }
      }

      protected override void ExecuteUpdateParameter(IExecutionContext context)
      {
         _oldPercentile = distributedParameter.Percentile;
         UpdateParameter(_parameter, context);
         UpdateParameter(OriginParameterFor(_parameter, context), context);
         Description = ParameterMessages.SetParameterPercentile(context.DisplayNameFor(_parameter), _oldPercentile, _percentile);
      }

      protected override void UpdateParameter(IParameter parameter, IExecutionContext context)
      {
         var distParameter = parameter as IDistributedParameter;
         if (distParameter == null) return;
         distributedParameter.Percentile = _percentile;
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         SetParameterPercentileCommand inverseCommand = new SetParameterPercentileCommand(distributedParameter, _oldPercentile).AsInverseFor(this);
         inverseCommand.AlteredOn = AlteredOn;
         return inverseCommand;
      }
   }
}