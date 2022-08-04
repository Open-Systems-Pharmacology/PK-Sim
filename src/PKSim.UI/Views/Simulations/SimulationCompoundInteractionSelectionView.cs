using System.Collections.Generic;
using System.Linq;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Assets;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout.Utils;
using PKSim.Assets;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.UI;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using static OSPSuite.UI.UIConstants.Size;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationCompoundInteractionSelectionView : BaseResizableUserControl, ISimulationCompoundInteractionSelectionView
   {
      private ISimulationCompoundInteractionSelectionPresenter _presenter;
      private readonly GridViewBinder<SimulationInteractionProcessSelectionDTO> _gridViewBinder;
      private readonly UxRepositoryItemComboBox _compoundProcessRepository;
      private readonly UxRepositoryItemComboBox _individualMoleculeRepository;
      private readonly UxRepositoryItemButtonEdit _removeField;

      public SimulationCompoundInteractionSelectionView()
      {
         InitializeComponent();
         _gridViewBinder = new GridViewBinder<SimulationInteractionProcessSelectionDTO>(gridView);
         _compoundProcessRepository = new UxRepositoryItemComboBox(gridView);
         _individualMoleculeRepository = new UxRepositoryItemComboBox(gridView);
         gridView.AllowsFiltering = false;
         gridView.ShowRowIndicator = false;
         _removeField = new UxRemoveButtonRepository();
      }

      public void AttachPresenter(ISimulationCompoundInteractionSelectionPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(IEnumerable<SimulationInteractionProcessSelectionDTO> allPartialProcessesDTO)
      {
         _gridViewBinder.BindToSource(allPartialProcessesDTO);
         AdjustHeight();
      }

      public override int OptimalHeight
      {
         get
         {
            var height = gridView.OptimalHeight + layoutItemAddInteraction.Height + layoutItemInteractionSelection.Padding.Height;
            height += layoutItemWarning.Visible ? layoutItemWarning.Height : 1;
            return height;
         }
      }

      public override void Repaint()
      {
         gridView.LayoutChanged();
      }

      public override void InitializeBinding()
      {
         BindToPartialIndividualMolecule();
         BindToPartialCompoundProcess();
         BindToDeleteInteraction();
         btnAddInteraction.Click += (o, e) => OnEvent(_presenter.AddInteraction);
         _removeField.ButtonClick += (o, e) => OnEvent(() => _presenter.RemoveInteraction(_gridViewBinder.FocusedElement));
      }

      protected IGridViewColumn BindToPartialCompoundProcess()
      {
         return _gridViewBinder.Bind(x => x.CompoundProcess)
            .WithRepository(repositoryItemForCompoundProcess)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithCaption(PKSimConstants.UI.InteractionProcessInCompound)
            .WithOnValueUpdating((dto, e) => OnEvent(() => _presenter.CompoundProcessSelectionChanged(dto, e.NewValue)));
      }

      protected IGridViewColumn BindToPartialIndividualMolecule()
      {
         return _gridViewBinder.Bind(x => x.IndividualMolecule)
            .WithRepository(repositoryItemForIndividualMolecules)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithCaption(PKSimConstants.UI.MoleculeInIndividual(PKSimConstants.UI.Molecule))
            .WithOnValueUpdating((dto, e) => OnEvent(() => _presenter.IndividualMoleculeSelectionChanged(dto, e.NewValue)));
      }

      protected IGridViewColumn BindToDeleteInteraction()
      {
         return _gridViewBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(x => _removeField)
            .WithFixedWidth(EMBEDDED_BUTTON_WIDTH);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemInteractionSelection.TextVisible = false;
         btnAddInteraction.InitWithImage(ApplicationIcons.Create, text: PKSimConstants.UI.AddInteraction);
         layoutItemAddInteraction.AdjustLargeButtonSize(layoutControl);
      }

      protected void ConfigureBaseEdit<T>(BaseEdit baseEdit, IEnumerable<T> allItems)
      {
         var list = allItems.ToList();
         baseEdit.FillComboBoxEditorWith(list);
         baseEdit.Enabled = (list.Count > 1);
      }

      protected RepositoryItem RepositoryItemFor<T>(IEnumerable<T> allItems, UxRepositoryItemComboBox listRepositoryItems)
      {
         listRepositoryItems.FillComboBoxRepositoryWith(allItems);
         return listRepositoryItems;
      }

      private RepositoryItem repositoryItemForCompoundProcess(SimulationInteractionProcessSelectionDTO simulationPartialProcessSelectionDTO)
      {
         return RepositoryItemFor(_presenter.AllCompoundProcesses(), _compoundProcessRepository);
      }

      private RepositoryItem repositoryItemForIndividualMolecules(SimulationInteractionProcessSelectionDTO dto)
      {
         return RepositoryItemFor(_presenter.AllIndividualMolecules(), _individualMoleculeRepository);
      }

      public bool WarningVisible
      {
         set
         {
            layoutItemWarning.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
            AdjustHeight();
         }
         get { return LayoutVisibilityConvertor.ToBoolean(layoutItemWarning.Visibility); }
      }

      public string Warning
      {
         get { return panelWarning.NoteText; }
         set { updatePanelWarning(value, ApplicationIcons.Warning); }
      }

      private void updatePanelWarning(string message, ApplicationIcon icon)
      {
         panelWarning.Image = icon;
         panelWarning.NoteText = message;
      }
   }
}