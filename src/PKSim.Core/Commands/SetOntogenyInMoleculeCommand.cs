using OSPSuite.Core.Commands.Core;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Commands
{
   public class SetOntogenyInMoleculeCommand : BuildingBlockChangeCommand
   {
      private readonly string _moleculeId;
      private readonly Ontogeny _newOntogeny;
      private readonly Ontogeny _oldOntogeny;

      private ISimulationSubject _simulationSubject;
      private IndividualMolecule _molecule;

      public SetOntogenyInMoleculeCommand(IndividualMolecule molecule, Ontogeny newOntogeny, ISimulationSubject simulationSubject, IExecutionContext context)
      {
         _molecule = molecule;
         _oldOntogeny = molecule.Ontogeny ?? new NullOntogeny();
         _newOntogeny = newOntogeny ?? new NullOntogeny();
         _moleculeId = molecule.Id;
         _simulationSubject = simulationSubject;
         BuildingBlockId = simulationSubject.Id;
         ObjectType = context.TypeFor(molecule);
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         var subjectName = string.IsNullOrEmpty(_simulationSubject.Name) ? CoreConstants.ContainerName.NameTemplate : _simulationSubject.Name;

         Description = PKSimConstants.Command.SetOntogenyInProteinDescription(subjectName, molecule.Name, _oldOntogeny.Name, _newOntogeny.Name);
         context.UpdateBuildingBlockPropertiesInCommand(this, simulationSubject);
      }


      protected override void PerformExecuteWith(IExecutionContext context)
      {
         var moleculeOntogenyVariabilityUpdater = context.Resolve<IMoleculeOntogenyVariabilityUpdater>();
         moleculeOntogenyVariabilityUpdater.UpdateMoleculeOntogeny(_molecule, _newOntogeny, _simulationSubject);

         var expressionProfileUpdater = context.Resolve<IExpressionProfileUpdater>();
         expressionProfileUpdater.SynchronizeExpressionProfileInAllSimulationSubjects(_simulationSubject);
      }

      protected override void ClearReferences()
      {
         //do not clear ontogeny that are needed to be restored for rollback
         _molecule = null;
         _simulationSubject = null;
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _simulationSubject = context.Get<ISimulationSubject>(BuildingBlockId);
         _molecule = context.Get<IndividualMolecule>(_moleculeId);
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetOntogenyInMoleculeCommand(_molecule, _oldOntogeny, _simulationSubject, context).AsInverseFor(this);
      }
   }
}