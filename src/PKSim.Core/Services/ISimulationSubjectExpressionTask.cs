using OSPSuite.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationSubjectExpressionTask<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      IOSPSuiteCommand RemoveMoleculeFrom(IndividualMolecule molecule, TSimulationSubject simulationSubject);
      IOSPSuiteCommand AddMoleculeTo(IndividualMolecule molecule, TSimulationSubject simulationSubject);
   }
}