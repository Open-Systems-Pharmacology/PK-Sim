using System.Collections.Generic;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Utility.Extensions;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using PKSim.Assets;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Assets;
using OSPSuite.UI;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using static OSPSuite.UI.UIConstants.Size;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationCompoundProtocolFormulationView : BaseGridViewOnlyUserControl, ISimulationCompoundProtocolFormulationView
   {
      private ISimulationCompoundProtocolFormulationPresenter _presenter;
      private readonly GridViewBinder<FormulationMappingDTO> _gridViewBinder;
      private readonly UxRepositoryItemComboBox _repositoryForFormulation;
      private IGridViewColumn _formulationKeyColumn;

      public SimulationCompoundProtocolFormulationView()
      {
         InitializeComponent();
         _repositoryForFormulation = new UxRepositoryItemComboBox(gridView) {AllowHtmlDraw = DefaultBoolean.True};
         _gridViewBinder = new GridViewBinder<FormulationMappingDTO>(gridView);
         gridView.HorzScrollVisibility = ScrollVisibility.Never;
         layoutControl.AutoScroll = false;
      }

      public void AttachPresenter(ISimulationCompoundProtocolFormulationPresenter presenter)
      {
         _presenter = presenter;
      }

      public override bool HasError => _gridViewBinder.HasError;

      public void BindTo(IEnumerable<FormulationMappingDTO> formulationMappings)
      {
         _gridViewBinder.BindToSource(formulationMappings.ToBindingList());
         gridView.BestFitColumns();
         AdjustHeight();
      }

      public void RefreshData()
      {
         gridView.RefreshData();
      }

      public bool FormulationKeyVisible
      {
         set => _formulationKeyColumn.UpdateVisibility(value);
      }

      public bool FormulationVisible
      {
         get => GridVisible;
         set => GridVisible = value;
      }

      public override void InitializeBinding()
      {
         var createFormulationButton = createdFormulationButtonRepository();
         var loadFormulationButton = loadFormulationButtonRepository();


         _formulationKeyColumn = _gridViewBinder.Bind(x => x.FormulationKey)
            .WithCaption(PKSimConstants.UI.PlaceholderFormulation)
            .AsReadOnly();

         _gridViewBinder.Bind(x => x.Route)
            .AsReadOnly();

         _gridViewBinder.AutoBind(x => x.Selection)
            .WithRepository(x => _repositoryForFormulation)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithEditorConfiguration(configureFormulation);

         _gridViewBinder.AddUnboundColumn()
            .WithCaption(Captions.EmptyColumn)
            .WithFixedWidth(EMBEDDED_BUTTON_WIDTH)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(dto => createFormulationButton);


         _gridViewBinder.AddUnboundColumn()
            .WithCaption(Captions.EmptyColumn)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithFixedWidth(EMBEDDED_BUTTON_WIDTH)
            .WithRepository(dto => loadFormulationButton);

         _gridViewBinder.Changed += () => _presenter.ViewChanged();
         createFormulationButton.ButtonClick += (o, e) => OnEvent(() => _presenter.CreateFormulationFor(_gridViewBinder.FocusedElement));
         loadFormulationButton.ButtonClick += (o, e) => OnEvent(() => _presenter.LoadFormulationForAsync(_gridViewBinder.FocusedElement));
      }

      private RepositoryItemButtonEdit createdFormulationButtonRepository()
      {
         return new UxRepositoryItemButtonImage(ApplicationIcons.Create, PKSimConstants.UI.CreateBuildingBlockHint(PKSimConstants.ObjectTypes.Formulation));
      }

      private RepositoryItemButtonEdit loadFormulationButtonRepository()
      {
         return new UxRepositoryItemButtonImage(ApplicationIcons.LoadFromTemplate, PKSimConstants.UI.LoadItemFromTemplate(PKSimConstants.ObjectTypes.Formulation));
      }

      private void configureFormulation(BaseEdit baseEdit, FormulationMappingDTO formulationMappingDTO)
      {
         baseEdit.FillComboBoxEditorWith(_presenter.AllFormulationsFor(formulationMappingDTO));
      }
   }
}