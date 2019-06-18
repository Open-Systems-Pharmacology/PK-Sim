using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class RenameMoleculeInPartialProcessesCommand : PKSimMacroCommand
   {
      private readonly string _moleculeName;
      private IEnumerable<PartialProcess> _partialProcesses;

      public RenameMoleculeInPartialProcessesCommand(IEnumerable<PartialProcess> partialProcesses, string moleculeName, IExecutionContext context)
      {
         _partialProcesses = partialProcesses.ToList();
         _moleculeName = moleculeName;
         ObjectType = PKSimConstants.ObjectTypes.Parameter;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         Description = PKSimConstants.Command.RenameMoleculeInPartialProcessesCommandDescription(moleculeName);

         context.UpdateBuildingBlockPropertiesInCommand(this, context.BuildingBlockContaining(_partialProcesses.FirstOrDefault()));
      }

      public override void Execute(IExecutionContext context)
      {
         _partialProcesses.Each(p => Add(new RenameMoleculeInPartialProcessCommand(p, _moleculeName, context)));

         //now execute all commands
         base.Execute(context);

         //clear references
         _partialProcesses = null;
      }
   }
}