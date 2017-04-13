using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   public class ScaleParametersCommand : PKSimMacroCommand
   {
      private readonly double _factor;
      private IEnumerable<IParameter> _parameters;

      public ScaleParametersCommand()
      {
         ObjectType = PKSimConstants.ObjectTypes.Parameter;
         CommandType = PKSimConstants.Command.CommandTypeReset;
      }

      public ScaleParametersCommand(IEnumerable<IParameter> parameters, double factor)
      {
         _parameters = parameters;
         _factor = factor;
      }

      public override void Execute(IExecutionContext context)
      {
         Description = PKSimConstants.Command.ScaleParametersDescription(_factor);
         foreach (var parameter in _parameters.Where(parameterCanBeScaled))
         {
            Add(new SetParameterValueCommand(parameter, parameter.Value * _factor));
         }

         //update properties from first command
         this.UpdatePropertiesFrom(All().FirstOrDefault());

         //now execute all commands
         base.Execute(context);

         //clear references
         _parameters = null;
      }

      private bool parameterCanBeScaled(IParameter parameter)
      {
         if (!parameter.Editable)
            return false;

         if (CoreConstants.Parameter.AllBooleanParameters.Contains(parameter.Name))
            return false;

         if (CoreConstants.Parameter.AllWithListOfValues.Contains(parameter.Name))
            return false;

         return true;
      }
   }
}