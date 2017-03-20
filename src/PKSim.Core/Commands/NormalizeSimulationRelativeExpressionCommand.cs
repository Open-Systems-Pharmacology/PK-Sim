using System.Linq;
using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;

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
         context.UpdateBuildinBlockProperties(this, context.BuildingBlockContaining(parameter));
         //This command is necessary to insure consitency but does not need to be seen
         Visible = false;
      }

      public string ParameterId { get; private set; }

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
         var proteinContainer = _parameter.ParentContainer;
         var rootContainer = _parameter.RootContainer;
         var allParameters = rootContainer.GetAllChildren<IParameter>(x => string.Equals(x.GroupName, CoreConstants.Groups.RELATIVE_EXPRESSION))
            .Where(x => x.BuildingBlockType == PKSimBuildingBlockType.Individual)
            .Where(x => string.Equals(proteinContainer.Name, x.ParentContainer.Name))
            .Where(x => x.Name.StartsWith(CoreConstants.Parameter.RelExp))
            .Where(x => x.Formula.IsConstant());

         var relNormExpressions = parameterTask.GroupExpressionParameters(allParameters);

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