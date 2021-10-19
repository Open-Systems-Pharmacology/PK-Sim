using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class UpdateDistributedTableFormulaPercentileCommand : EditParameterCommand
   {
      private readonly double _percentile;
      public double OldPercentile { get; }

      public UpdateDistributedTableFormulaPercentileCommand(IParameter tableParameter, double percentile) : base(tableParameter)
      {
         _percentile = percentile;
         OldPercentile = tableParameter.Formula.DowncastTo<DistributedTableFormula>().Percentile;
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new UpdateDistributedTableFormulaPercentileCommand(_parameter, OldPercentile).AsInverseFor(this);
      }

      protected override void ExecuteUpdateParameter(IExecutionContext context)
      {
         UpdateParameter(context);
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
         var allPoints = distributedTableFormula.AllPoints().ToArray();
         var distributionMapper = context.Resolve<IDistributionMetaDataToDistributionMapper>();

         for (int i = 0; i < distributedTableFormula.AllDistributionMetaData().Count; i++)
         {
            var distributionMetaData = distributedTableFormula.AllDistributionMetaData().ElementAt(i);
            var distribution = distributionMapper.MapFrom(distributionMetaData);
            allPoints[i].Y = distribution.CalculateValueFromPercentile(_percentile);
         }
      }
   }
}