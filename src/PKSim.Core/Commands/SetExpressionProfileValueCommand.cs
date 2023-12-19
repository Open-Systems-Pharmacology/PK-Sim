using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Commands
{
   public class SetExpressionProfileValueCommand : SetParameterValueCommand
   {
      private readonly bool _updateSimulationSubjects;

      public SetExpressionProfileValueCommand(IParameter parameter, double valueToSet, bool updateSimulationSubjects = true)
         : base(parameter, valueToSet)
      {
         _updateSimulationSubjects = updateSimulationSubjects;
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
         if (!_updateSimulationSubjects)
            return;

         var expressionProfile = context.BuildingBlockContaining(parameter) as ExpressionProfile;
         if (expressionProfile == null) return;
         var updateTask = context.Resolve<IExpressionProfileUpdater>();
         updateTask.SynchronizeAllSimulationSubjectsWithExpressionProfile(expressionProfile);
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetExpressionProfileValueCommand(_parameter, _oldValue, _updateSimulationSubjects).AsInverseFor(this);
      }
   }
}