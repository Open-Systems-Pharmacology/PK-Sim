using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Parameters;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IIndividualMoleculeExpressionsPresenter : IEditParameterPresenter
   {
      bool OntogenyVisible { set; }
      bool MoleculeParametersVisible { set; }
      ISimulationSubject SimulationSubject { get; set; }
      void ActivateMolecule(IndividualMolecule molecule);
   }
}