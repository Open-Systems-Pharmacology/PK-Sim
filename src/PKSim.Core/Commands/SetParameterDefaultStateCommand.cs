using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using PKSim.Assets;

namespace PKSim.Core.Commands
{
   public class SetParameterDefaultStateCommand : EditParameterCommand
   {
      private readonly bool _isDefault;
      private readonly bool _oldIsDefault;

      public SetParameterDefaultStateCommand(IParameter parameter, bool isDefault) : base(parameter)
      {
         _isDefault = isDefault;
         _oldIsDefault = _parameter.IsDefault;
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetParameterDefaultStateCommand(_parameter, _oldIsDefault).AsInverseFor(this);
      }

      protected override void ExecuteUpdateParameter(IParameter parameter, IExecutionContext context)
      {
         UpdateParameter(context);
         Description = PKSimConstants.Command.SetParameterDefaultStateFrom(context.DisplayNameFor(parameter), _oldIsDefault, _isDefault);
      }

      protected override void UpdateParameter(IParameter parameter, IExecutionContext context)
      {
         if (parameter == null) return;
         parameter.IsDefault = _isDefault;
      }
   }
}