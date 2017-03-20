using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Individuals;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Individuals
{
   public interface IIndividualMoleculePropertiesView : IView<IIndividualMoleculePropertiesPresenter>, IResizableView
   {
      void AddOntogenyView(IView view);
      bool OntogenyVisible { set; }
      bool MoleculeParametersVisible { set; }
      void BindTo(MoleculePropertiesDTO moleculePropertiesDTO);
   }
}