using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Commands
{
   public class UpdateParametersValueOriginCommand : PKSimMacroCommand
   {
      private IEnumerable<IParameter> _parameters;
      private ValueOrigin _valueOrigin;
      private readonly bool _shouldChangeBuildingBlockVersion;

      public UpdateParametersValueOriginCommand(IEnumerable<IParameter> parameters, ValueOrigin valueOrigin, bool shouldChangeBuildingBlockVersion = true)
      {
         _parameters = parameters;
         _valueOrigin = valueOrigin;
         _shouldChangeBuildingBlockVersion = shouldChangeBuildingBlockVersion;
      }

      public override void Execute(IExecutionContext context)
      {
         _parameters.Each(p => Add(new UpdateParameterValueOriginCommand(p, _valueOrigin) {Visible = false, ShouldChangeVersion = _shouldChangeBuildingBlockVersion }));

         var firstCommand = All().FirstOrDefault();

         //show first command only to ensure a perfect display in history
         if (firstCommand != null)
            firstCommand.Visible = true;

         //update properties from first command
         this.UpdatePropertiesFrom(firstCommand);

         //now execute all commands
         base.Execute(context);

         //clear references
         _parameters = null;
         _valueOrigin = null;
      }
   }
}