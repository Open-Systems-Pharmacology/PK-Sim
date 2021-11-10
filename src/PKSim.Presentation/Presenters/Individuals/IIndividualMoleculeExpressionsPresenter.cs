using OSPSuite.Presentation.Presenters;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IIndividualMoleculeExpressionsPresenter : ICommandCollectorPresenter
   {
      bool OntogenyVisible { set; }
      bool MoleculeParametersVisible { set; }
      ISimulationSubject SimulationSubject { get; set; }
      void ActivateMolecule(IndividualMolecule molecule);
      void DisableEdit();
   }
}