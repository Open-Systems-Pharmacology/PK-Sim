using OSPSuite.Presentation.Views;
using PKSim.Presentation.Presenters.Individuals;

namespace PKSim.Presentation.Views.Individuals
{
   public interface IIndividualMoleculePropertiesView : IView<IIndividualMoleculePropertiesPresenter>, IResizableView
   {
      void AddOntogenyView(IView view);
      void AddMoleculeParametersView(IView view);
      bool OntogenyVisible { set; }
      bool MoleculeParametersVisible { set; }
   }
}