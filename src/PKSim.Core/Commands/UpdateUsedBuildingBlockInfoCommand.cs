using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class UpdateUsedBuildingBlockInfoCommand : PKSimMacroCommand
   {
      private Simulation _simulation;
      private IPKSimBuildingBlock _templateBuildingBlock;
      private UsedBuildingBlock _usedBuildingBlock;

      public UpdateUsedBuildingBlockInfoCommand(Simulation simulation, UsedBuildingBlock usedBuildingBlock, IPKSimBuildingBlock templateBuildingBlock, IExecutionContext context)
      {
         _simulation = simulation;
         _usedBuildingBlock = usedBuildingBlock;
         _templateBuildingBlock = templateBuildingBlock;
         ObjectType = PKSimConstants.ObjectTypes.Simulation;
         CommandType = PKSimConstants.Command.CommandTypeUpdate;
         Description = PKSimConstants.Command.UpdateBuildingBlockInfoCommandDescription;
         context.UpdateBuildinBlockProperties(this, _simulation);
         Visible = false;
      }

      public override void Execute(IExecutionContext context)
      {
         Add(new SetUsedBuildingBlockVersionCommand(_simulation, _usedBuildingBlock, _templateBuildingBlock.Version, context));
         Add(new SetUsedBuildingBlockAlteredFlagCommand(_simulation, _usedBuildingBlock, false, context));
         base.Execute(context);

         _simulation = null;
         _usedBuildingBlock = null;
         _templateBuildingBlock = null;
      }
   }
}