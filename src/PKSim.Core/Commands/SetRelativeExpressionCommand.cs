using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Commands
{
   public class SetRelativeExpressionCommand : SetParameterValueCommand
   {
      private readonly bool _updateIndividuals;

      public SetRelativeExpressionCommand(IParameter parameter, double valueToSet, bool updateIndividuals = true)
         : base(parameter, valueToSet)
      {
         _updateIndividuals = updateIndividuals;
         ObjectType = PKSimConstants.ObjectTypes.Molecule;
      }

      protected override void UpdateParameter(IParameter parameter, IExecutionContext context)
      {
         base.UpdateParameter(parameter, context);
         if (parameter == null) return;
         parameter.IsDefault = (parameter.Value == 0);
      }

      protected override void UpdateDependenciesOnParameter(IParameter parameter, IExecutionContext context)
      {
         base.UpdateDependenciesOnParameter(parameter, context);
         if (!_updateIndividuals)
            return;

         var expressionProfile = context.BuildingBlockContaining(parameter) as ExpressionProfile;
         if (expressionProfile == null) return;
         var updateTask = context.Resolve<IExpressionProfileUpdater>();
         updateTask.SynchronizeExpressionProfileInAllIndividuals(expressionProfile);
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetRelativeExpressionCommand(_parameter, _oldValue).AsInverseFor(this);
      }
   }
}