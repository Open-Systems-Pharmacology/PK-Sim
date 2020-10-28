using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class RemoveMoleculeFromPopulationCommand : BuildingBlockIrreversibleStructureChangeCommand
   {
      private IndividualMolecule _molecule;
      private Population _population;

      public RemoveMoleculeFromPopulationCommand(IndividualMolecule molecule, Population population, IExecutionContext context)
      {
         _molecule = molecule;
         _population = population;
         BuildingBlockId = population.Id;

         CommandType = PKSimConstants.Command.CommandTypeDelete;
         ObjectType = context.TypeFor(molecule);
         Description = PKSimConstants.Command.RemoveEntityFromContainer(ObjectType, molecule.Name, context.TypeFor(population), population.Name);
         context.UpdateBuildingBlockPropertiesInCommand(this, population);
      }

      protected override void ClearReferences()
      {
         _molecule = null;
         _population = null;
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         _population.RemoveMolecule(_molecule);
         context.Unregister(_molecule);
         context.PublishEvent(new RemoveAdvancedParameterContainerFromPopulationEvent(_population));
         context.PublishEvent(new RemoveMoleculeFromSimulationSubjectEvent<Population> {Entity = _molecule, Container = _population});
      }
   }
}