using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Commands
{
   public class SetSchemaItemApplicationTypeCommand : PKSimMacroCommand
   {
      private readonly ApplicationType _newApplicationType;
      private ISchemaItem _schemaItem;

      public SetSchemaItemApplicationTypeCommand(ISchemaItem schemaItem, ApplicationType newApplicationType)
      {
         ObjectType = PKSimConstants.ObjectTypes.AdministrationProtocol;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         _schemaItem = schemaItem;
         _newApplicationType = newApplicationType;
      }

      public override void Execute(IExecutionContext context)
      {
         var oldApplicationType = _schemaItem.ApplicationType;

         Description = PKSimConstants.Command.SetApplicationSchemaItemApplicationTypeDescription(oldApplicationType.ToString(), _newApplicationType.ToString());
         context.UpdateBuildinBlockProperties(this, context.BuildingBlockContaining(_schemaItem));
         Add(new ChangeApplicationTypeCommand(_schemaItem, _newApplicationType, context));

         if (shouldResetFormulation(oldApplicationType))
            Add(new SetSchemaItemFormulationKeyCommand(_schemaItem, string.Empty, context));

         else if (shouldSetDefaultFormulation())
            Add(new SetSchemaItemFormulationKeyCommand(_schemaItem, CoreConstants.DEFAULT_FORMULATION_KEY, context));

         //update parameters according to application
         var applicationParameterRetriever = context.Resolve<ISchemaItemParameterRetriever>();

         //First delete all dynamic parameters
         foreach (var parameterToDelete in applicationParameterRetriever.AllDynamicParametersFor(_schemaItem))
         {
            Add(new RemoveParameterFromContainerCommand(parameterToDelete, _schemaItem, context) {Visible = false});
         }

         //Then add all new dynamic parameters
         foreach (var parameter in applicationParameterRetriever.AllDynamicParametersFor(_newApplicationType))
         {
            Add(new AddParameterToContainerCommand(parameter, _schemaItem, context) {Visible = false});
         }

         //now execute all commands
         base.Execute(context);

         //clear references
         _schemaItem = null;
      }

      private bool shouldSetDefaultFormulation()
      {
         return _newApplicationType.NeedsFormulation && string.IsNullOrEmpty(_schemaItem.FormulationKey);
      }

      private bool shouldResetFormulation(ApplicationType oldApplicationType)
      {
         //Reset formulation only if switching from an application with formulation to one without
         //and also with a formulation not already empty
         if (string.IsNullOrEmpty(_schemaItem.FormulationKey))
            return false;

         return oldApplicationType.NeedsFormulation && !_newApplicationType.NeedsFormulation;
      }
   }
}