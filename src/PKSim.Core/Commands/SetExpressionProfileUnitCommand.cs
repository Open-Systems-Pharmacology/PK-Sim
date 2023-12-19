using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Commands;

public class SetExpressionProfileUnitCommand: SetParameterUnitCommand
{
   private readonly bool _updateSimulationSubjects;

   public SetExpressionProfileUnitCommand(IParameter parameter, Unit newDisplayUnit, bool updateSimulationSubjects = true) : base(parameter, newDisplayUnit)
   {
      _updateSimulationSubjects = updateSimulationSubjects;
      ObjectType = PKSimConstants.ObjectTypes.Molecule;
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
      return new SetExpressionProfileUnitCommand(_parameter, _oldDisplayUnit, _updateSimulationSubjects).AsInverseFor(this);
   }
}