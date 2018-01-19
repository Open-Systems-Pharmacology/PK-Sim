using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Utility.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using OSPSuite.UI.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Extensions;
using OSPSuite.UI.RepositoryItems;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationCompoundEnzymaticProcessView : SimulationCompoundProcessView<EnzymaticProcess, SimulationEnzymaticProcessSelectionDTO>, ISimulationCompoundEnzymaticProcessView
   {
      private readonly UxRepositoryItemComboBox _metabolitesRepository;
      private IGridViewColumn _colMetaboliteName;

      public SimulationCompoundEnzymaticProcessView()
      {
         InitializeComponent();
         _metabolitesRepository = new UxRepositoryItemComboBox(gridViewPartialProcesses);
      }

      protected override void InitializePartialBinding()
      {
         BindToPartialImage();
         BindToPartialIndividualMolecule();
         BindToPartialCompoundMolecule();

         _colMetaboliteName = _gridViewPartialBinder.Bind(x => x.MetaboliteName)
            .WithRepository(repositoryItemForMetabolites)
            .WithEditorConfiguration(configureMetabolitesRepository)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithCaption(PKSimConstants.UI.Metabolite);

      }

      private void configureMetabolitesRepository(BaseEdit baseEdit, SimulationEnzymaticProcessSelectionDTO dto)
      {
         ConfigureBaseEdit(baseEdit, enzymaticProcessPresenter.AllMetabolitesFor(dto));
      }

      private RepositoryItem repositoryItemForMetabolites(SimulationEnzymaticProcessSelectionDTO dto)
      {
         return RepositoryItemFor(enzymaticProcessPresenter.AllMetabolitesFor(dto), _metabolitesRepository);
      }

      private ISimulationCompoundEnzymaticProcessPresenter enzymaticProcessPresenter => _presenter.DowncastTo<ISimulationCompoundEnzymaticProcessPresenter>();

      public void HideMetaboliteColumn()
      {
         _colMetaboliteName.UpdateVisibility(false);
      }
   }
}