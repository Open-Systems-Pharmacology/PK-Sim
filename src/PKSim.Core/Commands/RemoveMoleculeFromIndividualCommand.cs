using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class RemoveMoleculeFromIndividualCommand : BuildingBlockIrreversibleStructureChangeCommand
   {
      private IndividualMolecule _molecule;
      private Individual _individual;

      public RemoveMoleculeFromIndividualCommand(IndividualMolecule molecule, Individual individual, IExecutionContext context)
      {
         _molecule = molecule;
         _individual = individual;
         BuildingBlockId = individual.Id;

         CommandType = PKSimConstants.Command.CommandTypeDelete;
         ObjectType = context.TypeFor(molecule);
         Description = PKSimConstants.Command.RemoveEntityFromContainer(ObjectType, molecule.Name, context.TypeFor(individual), individual.Name);
         context.UpdateBuildingBlockPropertiesInCommand(this, individual);
      }

      protected override void ClearReferences()
      {
         _molecule = null;
         _individual = null;
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         var removedContainer = _individual.RemoveMolecule(_molecule);
         removedContainer.Each(context.Unregister);
         context.PublishEvent(new RemoveMoleculeFromSimulationSubjectEvent<Individual> {Entity = _molecule, Container = _individual});
      }
   }
}