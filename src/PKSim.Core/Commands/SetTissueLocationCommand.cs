using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class SetTissueLocationCommand : BuildingBlockStructureChangeCommand
   {
      private readonly TissueLocation _newTissuleLocation;
      private readonly string _proteinId;
      private IndividualProtein _protein;

      public SetTissueLocationCommand(IndividualProtein protein, TissueLocation tissueLocation, IExecutionContext context)
      {
         _protein = protein;
         BuildingBlockId = context.BuildingBlockIdContaining(protein);
         _newTissuleLocation = tissueLocation;
         ObjectType = PKSimConstants.ObjectTypes.Protein;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         _proteinId = _protein.Id;
         context.UpdateBuildingBlockPropertiesInCommand(this, context.BuildingBlockContaining(protein));
      }

      public TissueLocation OldTissuleLocation { get; private set; }

      protected override void ClearReferences()
      {
         _protein = null;
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _protein = context.Get<IndividualProtein>(_proteinId);
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetTissueLocationCommand(_protein, OldTissuleLocation, context).AsInverseFor(this);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         OldTissuleLocation = _protein.TissueLocation;
         _protein.TissueLocation = _newTissuleLocation;
         Description = PKSimConstants.Command.SetProteinTissueLocationDescription(OldTissuleLocation.ToString(), _newTissuleLocation.ToString());
      }
   }
}