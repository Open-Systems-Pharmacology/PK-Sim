using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IOntogenyTask
   {
      ICommand SetOntogenyForMolecule(IndividualMolecule molecule, Ontogeny ontogeny, ISimulationSubject simulationSubject);

      void ShowOntogenyData(Ontogeny ontogeny);

      ICommand LoadOntogenyForMolecule(IndividualMolecule molecule, ISimulationSubject simulationSubject);
   }
}