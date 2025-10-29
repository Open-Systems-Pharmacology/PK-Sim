using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Commands;

public class ResetExpressionParameterCommand : ResetParameterCommand
{
   public ResetExpressionParameterCommand(IParameter parameter) : base(parameter)
   {
   }

   protected override void UpdateDependenciesOnParameter(IParameter parameter, IExecutionContext context)
   {
      base.UpdateDependenciesOnParameter(parameter, context);

      if (context.BuildingBlockContaining(parameter) is not ExpressionProfile expressionProfile)
         return;

      var updateTask = context.Resolve<IExpressionProfileUpdater>();
      updateTask.SynchronizeAllSimulationSubjectsWithExpressionProfile(expressionProfile);
   }

   protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
   {
      return new SetExpressionProfileValueCommand(_parameter, _oldValue).AsInverseFor(this);
   }
}