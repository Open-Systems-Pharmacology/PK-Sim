using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Commands
{
   public class SetOntogenyInMoleculeForIndividualCommand : SetOntogenyInMoleculeCommand<Individual>
   {
      public SetOntogenyInMoleculeForIndividualCommand(IndividualMolecule molecule, Ontogeny newOntogeny, Individual individual, IExecutionContext context)
         : base(molecule, newOntogeny, individual, context)
      {
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         var moleculeOntogenyVariabilityUpdater = context.Resolve<IMoleculeOntogenyVariabilityUpdater>();
         moleculeOntogenyVariabilityUpdater.UpdateMoleculeOntogeny(_molecule, _newOntogeny, _simulationSubject);
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetOntogenyInMoleculeForIndividualCommand(_molecule, _oldOntogeny, _simulationSubject, context).AsInverseFor(this);
      }
   }
}