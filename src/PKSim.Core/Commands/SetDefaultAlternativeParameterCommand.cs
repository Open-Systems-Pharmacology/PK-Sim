using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class SetDefaultAlternativeParameterCommand : BuildingBlockChangeCommand
   {
      private readonly string _oldDefaultAlternativeId;
      private readonly string _parameterGroupId;
      private PKSim.Core.Model.ParameterAlternative _oldDefaultAlternative;
      private PKSim.Core.Model.ParameterAlternative _parameterAlternative;
      private PKSim.Core.Model.ParameterAlternativeGroup _parameterGroup;

      public SetDefaultAlternativeParameterCommand(PKSim.Core.Model.ParameterAlternativeGroup parameterGroup, PKSim.Core.Model.ParameterAlternative parameterAlternative, IExecutionContext context)
      {
         _parameterGroup = parameterGroup;
         _parameterGroupId = _parameterGroup.Id;
         _parameterAlternative = parameterAlternative;
         _oldDefaultAlternative = parameterGroup.DefaultAlternative;
         _oldDefaultAlternativeId = _oldDefaultAlternative.Id;
         ObjectType = PKSimConstants.ObjectTypes.Compound;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         BuildingBlockId = context.BuildingBlockIdContaining(parameterGroup);
         Description = PKSimConstants.Command.SetDefaultAlternativeParameterDescription(context.DisplayNameFor(parameterGroup), _oldDefaultAlternative.Name, parameterAlternative.Name);
         ShouldChangeVersion = false;
         context.UpdateBuildinBlockProperties(this, context.BuildingBlockContaining(parameterGroup));
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         _oldDefaultAlternative.IsDefault = false;
         _parameterAlternative.IsDefault = true;
      }

      protected override void ClearReferences()
      {
         _parameterGroup = null;
         _parameterAlternative = null;
         _oldDefaultAlternative = null;
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _parameterGroup = context.Get<PKSim.Core.Model.ParameterAlternativeGroup>(_parameterGroupId);
         _oldDefaultAlternative = context.Get<PKSim.Core.Model.ParameterAlternative>(_oldDefaultAlternativeId);
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetDefaultAlternativeParameterCommand(_parameterGroup, _oldDefaultAlternative, context).AsInverseFor(this);
      }
   }
}