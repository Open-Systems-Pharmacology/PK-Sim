using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationCompoundEnzymaticProcessView : SimulationCompoundProcessView<EnzymaticProcess, SimulationEnzymaticProcessSelectionDTO>, ISimulationCompoundEnzymaticProcessView
   {
      private readonly UxRepositoryItemComboBox _metabolitesRepository;
      private readonly UxAddAndRemoveButtonRepository _addAndRemoveRepository = new UxAddAndRemoveButtonRepository();
      private readonly UxAddAndDisabledRemoveButtonRepository _addAndDisableRemoveRepository = new UxAddAndDisabledRemoveButtonRepository();
      private IGridViewColumn _colMetaboliteName;

      public SimulationCompoundEnzymaticProcessView()
      {
         InitializeComponent();
         _metabolitesRepository = new UxRepositoryItemComboBox(gridViewPartialProcesses);
      }

      protected override void InitializePartialBinding()
      {
         base.InitializePartialBinding();

         _colMetaboliteName = _gridViewPartialBinder.Bind(x => x.MetaboliteName)
            .WithRepository(repositoryItemForMetabolites)
            .WithEditorConfiguration(configureMetabolitesRepository)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithCaption(PKSimConstants.UI.Metabolite);

         _gridViewPartialBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(getAddRepository)
            .WithFixedWidth(OSPSuite.UI.UIConstants.Size.EMBEDDED_BUTTON_WIDTH * 2);

         _addAndRemoveRepository.ButtonClick += (o, e) => OnEvent(() => buttonRepositoryButtonClick(e, _gridViewPartialBinder.FocusedElement));
         _addAndDisableRemoveRepository.ButtonClick += (o, e) => OnEvent(() => buttonRepositoryButtonClick(e, _gridViewPartialBinder.FocusedElement));
      }

      private void buttonRepositoryButtonClick(ButtonPressedEventArgs e, SimulationEnzymaticProcessSelectionDTO dto)
      {
         if (Equals(e.Button.Tag, ButtonType.Add))
            enzymaticProcessPresenter.AddPartialProcessMappingBaseOn(dto);
         else
            enzymaticProcessPresenter.DeletePartialProcessMapping(dto);
      }

      private RepositoryItem getAddRepository(SimulationEnzymaticProcessSelectionDTO dto)
      {
         return enzymaticProcessPresenter.CanDeletePartialProcess(dto) ? _addAndRemoveRepository : _addAndDisableRemoveRepository;
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

      public void HideMultipleCompoundsColumns()
      {
         _colMetaboliteName.UpdateVisibility(false);
      }
   }
}