using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationSubjectExpressionTask<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      ICommand RemoveMoleculeFrom(IndividualMolecule molecule, TSimulationSubject simulationSubject);
      ICommand AddMoleculeTo(IndividualMolecule molecule, TSimulationSubject simulationSubject);
      ICommand EditMolecule(IndividualMolecule moleculeToEdit, IndividualMolecule editedMolecule, QueryExpressionResults queryResults, TSimulationSubject simulationSubject);
      ICommand AddMoleculeTo(IndividualMolecule molecule, TSimulationSubject simulationSubject, QueryExpressionResults queryExpressionResults);
      ICommand RenameMolecule(IndividualMolecule molecule, TSimulationSubject simulationSubject, string newName);
   }
}