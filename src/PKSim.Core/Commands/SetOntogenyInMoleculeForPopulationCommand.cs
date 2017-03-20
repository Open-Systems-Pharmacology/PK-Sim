using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Commands
{
   public class SetOntogenyInMoleculeForPopulationCommand : SetOntogenyInMoleculeCommand<Population>
   {
      public SetOntogenyInMoleculeForPopulationCommand(IndividualMolecule molecule, Ontogeny newOntogeny, Population population, IExecutionContext context)
         : base(molecule, newOntogeny, population, context)
      {
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         var moleculeOntogenyVariabilityUpdater = context.Resolve<IMoleculeOntogenyVariabilityUpdater>();
         moleculeOntogenyVariabilityUpdater.UpdateMoleculeOntogeny(_molecule, _newOntogeny, _simulationSubject);
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetOntogenyInMoleculeForPopulationCommand(_molecule, _oldOntogeny, _simulationSubject, context).AsInverseFor(this);
      }
   }
}