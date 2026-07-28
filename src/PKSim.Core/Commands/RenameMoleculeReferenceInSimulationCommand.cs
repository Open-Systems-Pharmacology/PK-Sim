using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   /// <summary>
   ///    Updates the molecule referenced by the compound process and interaction selections of a simulation. This is
   ///    required when the molecule of an expression profile used in the simulation was renamed. Without it, the simulation
   ///    could not be created from its building blocks anymore since the referenced molecule would not exist
   /// </summary>
   public class RenameMoleculeReferenceInSimulationCommand : BuildingBlockStructureChangeCommand
   {
      private Simulation _simulation;
      private readonly string _oldMoleculeName;
      private readonly string _newMoleculeName;

      public RenameMoleculeReferenceInSimulationCommand(Simulation simulation, string oldMoleculeName, string newMoleculeName, IExecutionContext context)
      {
         _simulation = simulation;
         _oldMoleculeName = oldMoleculeName;
         _newMoleculeName = newMoleculeName;
         BuildingBlockId = simulation.Id;
         ObjectType = context.TypeFor(simulation);
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         Description = PKSimConstants.Command.RenameEntityCommandDescripiton(ObjectType, _oldMoleculeName, _newMoleculeName);

         //Command is hidden as it only deals with internals
         Visible = false;
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         allMoleculeReferences()
            .Where(x => string.Equals(x.MoleculeName, _oldMoleculeName))
            .Each(x => x.MoleculeName = _newMoleculeName);
      }

      private IEnumerable<IProcessMapping> allMoleculeReferences() =>
         _simulation.CompoundPropertiesList.SelectMany(x => x.Processes.AllProcesses())
            .Concat<IProcessMapping>(_simulation.InteractionProperties.Interactions);

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new RenameMoleculeReferenceInSimulationCommand(_simulation, _newMoleculeName, _oldMoleculeName, context).AsInverseFor(this);
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _simulation = context.Get<Simulation>(BuildingBlockId);
      }

      protected override void ClearReferences()
      {
         _simulation = null;
      }
   }
}
