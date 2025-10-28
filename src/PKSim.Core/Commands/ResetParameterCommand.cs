using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core.Commands
{
   public class ResetParameterCommand : EditParameterCommand
   {
      private double _oldValue;

      public ResetParameterCommand(IParameter parameter) : base(parameter)
      {
      }

      protected override void ExecuteUpdateParameter(IParameter parameter, IExecutionContext context)
      {
         _oldValue = _parameter.Value;
         UpdateParameter(context);
         Description = ParameterMessages.ResetParameterValue(parameter, context.DisplayNameFor(parameter), _oldValue);
      }

      protected override void UpdateParameter(IParameter parameter, IExecutionContext context)
      {
         if (parameter == null) return;
         parameter.ResetToDefault();
         resetValueOriginForDefaultParameter(parameter, context);
      }

      protected override void UpdateDependenciesOnParameter(IParameter parameter, IExecutionContext context)
      {
         base.UpdateDependenciesOnParameter(parameter, context);

         if (context.BuildingBlockContaining(parameter) is not ExpressionProfile expressionProfile) 
            return;
         
         var updateTask = context.Resolve<IExpressionProfileUpdater>();
         updateTask.SynchronizeAllSimulationSubjectsWithExpressionProfile(expressionProfile);
      }

      private void resetValueOriginForDefaultParameter(IParameter parameter, IExecutionContext context)
      {
         var valueOriginRepository = context.Resolve<IValueOriginRepository>();

         var valueOrigin = valueOriginRepository.ValueOriginFor(parameter);
         parameter.UpdateValueOriginFrom(valueOrigin);

         //reset only available for truly default parameter with a default value
         parameter.IsDefault = true;
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         //inverse of a reset command set the previous value back into the parameter
         return new SetParameterValueCommand(_parameter, _oldValue).AsInverseFor(this);
      }
   }
}