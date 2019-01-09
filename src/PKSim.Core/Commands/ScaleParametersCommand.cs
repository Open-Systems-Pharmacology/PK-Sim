using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Core.Domain;
using PKSim.Core.Services;

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
         var parameterTask = context.Resolve<IParameterTask>();
         foreach (var parameter in _parameters.Where(parameterCanBeScaled))
         {
            Add(parameterTask.SetParameterValue(parameter, parameter.Value * _factor));
         }

         //update properties from first command
         this.UpdatePropertiesFrom(All().FirstOrDefault());

         //clear references
         _parameters = null;
      }

      private bool parameterCanBeScaled(IParameter parameter)
      {
         if (!parameter.Editable)
            return false;

         if (CoreConstants.Parameters.AllBooleanParameters.Contains(parameter.Name))
            return false;

         if (CoreConstants.Parameters.AllWithListOfValues.Contains(parameter.Name))
            return false;

         return true;
      }
   }
}