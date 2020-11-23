using OSPSuite.Presentation.Views;
using PKSim.Presentation.Presenters.Individuals;

namespace PKSim.Presentation.Views.Individuals
{
   public interface IIndividualMoleculePropertiesView : IView<IIndividualMoleculePropertiesPresenter>, IResizableView
   {
      void AddOntogenyView(IResizableView view);
      void AddMoleculeParametersView(IResizableView view);
      bool OntogenyVisible { set; }
      bool MoleculeParametersVisible { set; }
   }
}