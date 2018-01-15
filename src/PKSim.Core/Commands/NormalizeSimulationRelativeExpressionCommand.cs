using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Commands
{
   public class NormalizeSimulationRelativeExpressionCommand : BuildingBlockChangeCommand
   {
      private IParameter _parameter;

      public NormalizeSimulationRelativeExpressionCommand(IParameter parameter, IExecutionContext context)
      {
         _parameter = parameter;
         ParameterId = parameter.Id;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         ObjectType = PKSimConstants.ObjectTypes.Protein;
         BuildingBlockId = context.BuildingBlockIdContaining(parameter);
         context.UpdateBuildinBlockPropertiesInCommand(this, context.BuildingBlockContaining(parameter));
         //This command is necessary to insure consitency but does not need to be seen
         Visible = false;
      }

      public string ParameterId { get; }

      protected override void ClearReferences()
      {
         _parameter = null;
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new NormalizeSimulationRelativeExpressionCommand(_parameter, context).AsInverseFor(this);
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _parameter = context.Get<IParameter>(ParameterId);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         var parameterTask = context.Resolve<IParameterTask>();

         var allRelativeExpressionParameters = _parameter.AllRelatedRelativeExpressionParameters();

         var relNormExpressions = parameterTask.GroupExpressionParameters(allRelativeExpressionParameters);

         double max = relNormExpressions.Keys.Select(x => x.Value).Max();

         foreach (var parameter in relNormExpressions.KeyValues)
         {
            var parameterNorm = parameter.Value;
            var newValue = max == 0 ? 0 : parameter.Key.Value / max;
            if (ValueComparer.AreValuesEqual(parameterNorm.Value, newValue))
               continue;

            new SetParameterValueCommand(parameterNorm, newValue).Execute(context);
         }
      }
   }
}