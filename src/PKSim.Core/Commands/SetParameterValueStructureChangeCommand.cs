using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   /// <summary>
   ///    Wrapper around SetParameterValueCommand that should be a BuildingBlockStructureChange
   /// </summary>
   public class SetParameterValueStructureChangeCommand : SetParameterValueCommand, IBuildingBlockStructureChangeCommand
   {
      public SetParameterValueStructureChangeCommand(IParameter parameter, double valueToSet)
         : base(parameter, valueToSet)
      {
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetParameterValueStructureChangeCommand(_parameter, _oldValue).AsInverseFor(this);
      }
   }
}