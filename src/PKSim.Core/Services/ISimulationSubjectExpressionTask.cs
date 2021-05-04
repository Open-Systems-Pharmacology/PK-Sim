using OSPSuite.Core.Commands;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationSubjectExpressionTask<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      ICommand RemoveMoleculeFrom(IndividualMolecule molecule, TSimulationSubject simulationSubject);
      ICommand AddMoleculeTo(IndividualMolecule molecule, TSimulationSubject simulationSubject);
      IOSPSuiteCommand EditMolecule(IndividualMolecule moleculeToEdit, QueryExpressionResults queryResults, TSimulationSubject simulationSubject);
      ICommand AddMoleculeTo(IndividualMolecule molecule, TSimulationSubject simulationSubject, QueryExpressionResults queryExpressionResults);
      ICommand RenameMolecule(IndividualMolecule molecule, string newName, TSimulationSubject simulationSubject);
   }
}