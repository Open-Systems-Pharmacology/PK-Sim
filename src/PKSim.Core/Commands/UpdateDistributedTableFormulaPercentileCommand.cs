using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   public class UpdateDistributedTableFormulaPercentileCommand : EditParameterCommand
   {
      private readonly double _percentile;
      public double OldPercentile { get; private set; }

      public UpdateDistributedTableFormulaPercentileCommand(IParameter tableParameter, double percentile) : base(tableParameter)
      {
         _percentile = percentile;
         OldPercentile = tableParameter.Formula.DowncastTo<DistributedTableFormula>().Percentile;
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new UpdateDistributedTableFormulaPercentileCommand(_parameter, OldPercentile).AsInverseFor(this);
      }

      protected override void ExecuteUpdateParameter(IExecutionContext context)
      {
         var bbParameter = OriginParameterFor(_parameter, context);
         UpdateParameter(_parameter, context);
         UpdateParameter(bbParameter, context);
         Description = ParameterMessages.UpdateTableParameterFormula(context.DisplayNameFor(_parameter));
      }

      protected override void UpdateParameter(IParameter parameter, IExecutionContext context)
      {
         if (parameter == null) return;
         var distributedParameter = parameter as IDistributedParameter;
         if (distributedParameter != null)
         {
            //the building block parameter in that case is a distributed parameter.
            distributedParameter.Percentile = _percentile;
            return;
         }
         var distributedTableFormula = parameter.Formula.DowncastTo<DistributedTableFormula>();
         distributedTableFormula.Percentile = _percentile;
         var distributionMapper = context.Resolve<IDistributionMetaDataToDistributionMapper>();
         for (int i = 0; i < distributedTableFormula.AllDistributionMetaData().Count(); i++)
         {
            var distributionMetaData = distributedTableFormula.AllDistributionMetaData().ElementAt(i);
            var distribution = distributionMapper.MapFrom(distributionMetaData);
            distributedTableFormula.AllPoints().ElementAt(i).Y = distribution.CalculateValueFromPercentile(_percentile);
         }
      }
   }
}