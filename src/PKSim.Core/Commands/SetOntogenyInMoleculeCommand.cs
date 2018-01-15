using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public abstract class SetOntogenyInMoleculeCommand<TSimulationSubject> : BuildingBlockChangeCommand where TSimulationSubject : class, ISimulationSubject
   {
      private readonly string _moleculeId;
      protected readonly Ontogeny _newOntogeny;
      protected readonly Ontogeny _oldOntogeny;

      protected TSimulationSubject _simulationSubject;
      protected IndividualMolecule _molecule;

      protected SetOntogenyInMoleculeCommand(IndividualMolecule molecule, Ontogeny newOntogeny, TSimulationSubject simulationSubject, IExecutionContext context)
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
         context.UpdateBuildinBlockPropertiesInCommand(this, simulationSubject);
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
         _simulationSubject = context.Get<TSimulationSubject>(BuildingBlockId);
         _molecule = context.Get<IndividualMolecule>(_moleculeId);
      }
   }
}