using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IOntogenyTask<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      ICommand SetOntogenyForMolecule(IndividualMolecule molecule, Ontogeny ontogeny, TSimulationSubject simulationSubject);

      void ShowOntogenyData(Ontogeny ontogeny);

      ICommand LoadOntogenyForMolecule(IndividualMolecule molecule, TSimulationSubject simulationSubject);
   }
}