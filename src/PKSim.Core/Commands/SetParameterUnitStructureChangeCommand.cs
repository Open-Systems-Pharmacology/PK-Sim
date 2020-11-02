using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Commands
{
   /// <summary>
   ///    Wrapper around SetParameterUnitCommand that should be a BuildingBlockStructureChange
   /// </summary>
   public class SetParameterUnitStructureChangeCommand : SetParameterUnitCommand, IBuildingBlockStructureChangeCommand
   {
      public SetParameterUnitStructureChangeCommand(IParameter parameter, Unit newDisplayUnit)
         : base(parameter, newDisplayUnit)
      {
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetParameterUnitStructureChangeCommand(_parameter, _oldDisplayUnit).AsInverseFor(this);
      }
   }
}