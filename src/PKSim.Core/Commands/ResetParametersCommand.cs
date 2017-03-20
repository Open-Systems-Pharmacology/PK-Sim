using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   public class ResetParametersCommand : PKSimMacroCommand
   {
      private IEnumerable<IParameter> _parameters;

      public ResetParametersCommand(IEnumerable<IParameter> parameters)
      {
         ObjectType = PKSimConstants.ObjectTypes.Parameter;
         CommandType = PKSimConstants.Command.CommandTypeReset;
         Description = PKSimConstants.Command.ResetParametersDescription;
         _parameters = parameters;
      }

      public override void Execute(IExecutionContext context)
      {
         allParametersToReset().Each(p => Add(new ResetParameterCommand(p)));

         //update properties from first command
         this.UpdatePropertiesFrom(All().FirstOrDefault());

         //now execute all commands
         base.Execute(context);

         //clear references
         _parameters = null;
      }

      private IEnumerable<IParameter> allParametersToReset()
      {
         return from p in _parameters
                where p.ValueDiffersFromDefault()
                select p;
      }
   }
}