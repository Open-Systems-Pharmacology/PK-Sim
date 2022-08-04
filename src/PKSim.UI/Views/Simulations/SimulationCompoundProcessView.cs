using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using OSPSuite.Assets;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using static OSPSuite.UI.UIConstants.Size;
using UxGridView = PKSim.UI.Views.Core.UxGridView;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationCompoundProcessView<TPartialProcess, TPartialProcessDTO> : BaseResizableUserControl,
      ISimulationCompoundProcessView<TPartialProcess, TPartialProcessDTO>
      where TPartialProcessDTO : SimulationPartialProcessSelectionDTO
      where TPartialProcess : PartialProcess
   {
      private readonly UxRepositoryItemComboBox _compoundMoleculeRepository;
      private readonly RepositoryItemPictureEdit _imageRepository;
      private readonly UxRepositoryItemComboBox _systemicProcessRepository;
      private readonly RepositoryItemTextEdit _repositoryItemDisabled = new RepositoryItemTextEdit {Enabled = false, ReadOnly = true};
      protected readonly GridViewBinder<TPartialProcessDTO> _gridViewPartialBinder;
      private readonly GridViewBinder<SimulationSystemicProcessSelectionDTO> _gridViewSystemicBinder;
      protected ISimulationCompoundProcessPresenter<TPartialProcess, TPartialProcessDTO> _presenter;
      private IGridViewColumn _colIndividualMolecule;
      private IGridViewColumn _colCompoundProcess;
      public ApplicationIcon Icon { get; set; }

      public SimulationCompoundProcessView()
      {
         InitializeComponent();
         _compoundMoleculeRepository = new UxRepositoryItemComboBox(gridViewPartialProcesses);
         _systemicProcessRepository = new UxRepositoryItemComboBox(gridViewSystemicProcesses);
         _imageRepository = new RepositoryItemPictureEdit();
         gridViewSystemicProcesses.AllowsFiltering = false;
         gridViewSystemicProcesses.HorzScrollVisibility = ScrollVisibility.Never;
         gridViewPartialProcesses.AllowsFiltering = false;
         gridViewPartialProcesses.HorzScrollVisibility = ScrollVisibility.Never;

         gridViewSystemicProcesses.RowStyle += rowSystemicStyle;
         gridViewPartialProcesses.RowStyle += rowPartialStyle;
         Icon = ApplicationIcons.EmptyIcon;
         _gridViewPartialBinder = new GridViewBinder<TPartialProcessDTO>(gridViewPartialProcesses);
         _gridViewSystemicBinder = new GridViewBinder<SimulationSystemicProcessSelectionDTO>(gridViewSystemicProcesses);
      }

      public void AttachPresenter(ISimulationCompoundProcessPresenter<TPartialProcess, TPartialProcessDTO> presenter)
      {
         _presenter = presenter;
      }

      public string MoleculeName
      {
         set => _colIndividualMolecule.Caption = PKSimConstants.UI.MoleculeInIndividual(value);
      }

      public string CompoundProcessCaption
      {
         set => _colCompoundProcess.Caption = value;
      }

      public bool WarningVisible
      {
         set => layoutItemWarning.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
         get => LayoutVisibilityConvertor.ToBoolean(layoutItemWarning.Visibility);
      }

      public string Warning
      {
         get => panelWarning.NoteText;
         set => updatePanelWarning(value, ApplicationIcons.ErrorHint);
      }

      private void updatePanelWarning(string message, ApplicationIcon icon)
      {
         panelWarning.Image = icon;
         panelWarning.NoteText = message;
      }

      public string Info
      {
         get => panelWarning.NoteText;
         set => updatePanelWarning(value, ApplicationIcons.Info);
      }

      public override void InitializeBinding()
      {
         InitializePartialBinding();

         InitializeSystemicBinding();
      }

      protected virtual void InitializeSystemicBinding()
      {
         _gridViewSystemicBinder.Bind(x => x.Image)
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithRepository(dto => _imageRepository)
            .WithFixedWidth(EMBEDDED_BUTTON_WIDTH)
            .AsReadOnly();

         _gridViewSystemicBinder.Bind(x => x.SystemicProcessType)
            .WithCaption(PKSimConstants.UI.SystemicProcess)
            .AsReadOnly();

         _gridViewSystemicBinder.Bind(x => x.SelectedProcess)
            .WithCaption(PKSimConstants.UI.DataSourceColumn)
            .WithRepository(repositoryItemForSystemicProcesses)
            .WithEditorConfiguration(configureSystemicProcessesRepository)
            .WithFixedWidth(UIConstants.Size.DATA_SOURCE_WIDTH)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithOnChanged(dto => OnEvent(() => _presenter.SelectionChanged(dto)));
      }

      protected virtual void InitializePartialBinding()
      {
         BindToPartialImage();
         BindToPartialIndividualMolecule();
         BindToPartialCompoundMolecule();
      }

      protected IGridViewColumn BindToPartialCompoundMolecule()
      {
         return _colCompoundProcess = _gridViewPartialBinder.Bind(x => x.CompoundProcess)
            .WithRepository(repositoryItemForCompoundProcesses)
            .WithEditorConfiguration(configureCompoundProcessesRepository)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithOnValueUpdating((dto, e) => OnEvent(() => _presenter.CompoundProcessChanged(dto, e.NewValue)))
            .WithOnChanged(dto => OnEvent(() => _presenter.SelectionChanged(dto)));
      }

      protected IGridViewColumn BindToPartialIndividualMolecule()
      {
         return _colIndividualMolecule = _gridViewPartialBinder.Bind(x => x.IndividualMolecule)
            .AsReadOnly();
      }

      protected IGridViewColumn BindToPartialImage()
      {
         return _gridViewPartialBinder.Bind(x => x.Image)
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithRepository(dto => _imageRepository)
            .WithFixedWidth(EMBEDDED_BUTTON_WIDTH)
            .AsReadOnly();
      }

      public void BindToPartialProcesses(IEnumerable<TPartialProcessDTO> allPartialProcessSelectionDTO)
      {
         _gridViewPartialBinder.BindToSource(allPartialProcessSelectionDTO);
      }

      public void BindToSystemicProcesses(IEnumerable<SimulationSystemicProcessSelectionDTO> allSystemicProcessSelectionDTO)
      {
         _gridViewSystemicBinder.BindToSource(allSystemicProcessSelectionDTO);
      }

      public override void AdjustHeight()
      {
         adjustLayoutItemForView(layoutItemPartialProcesses, gridViewPartialProcesses, _gridViewPartialBinder);
         adjustLayoutItemForView(layoutItemSystemicProcesses, gridViewSystemicProcesses, _gridViewSystemicBinder);
         base.AdjustHeight();
      }

      /// <summary>
      ///    Hide the layout if no item is being displayed. Otherwise adjust the height of the layout item to fit exactly the
      ///    space needed by the grid view
      /// </summary>
      private void adjustLayoutItemForView<T>(LayoutControlItem layoutControlItem, UxGridView gridView, GridViewBinder<T> gridViewBinder)
      {
         bool hasItems = (gridViewBinder.Source.Any());
         layoutControlItem.Visibility = LayoutVisibilityConvertor.FromBoolean(hasItems);
         layoutControlItem.SizeConstraintsType = SizeConstraintsType.Custom;
         gridView.BestFitColumns();
         layoutControlItem.AdjustControlHeight(layoutControl, gridView.OptimalHeight);
      }

      public override int OptimalHeight
      {
         get
         {
            var height = layoutItemPartialProcesses.Visible ? gridViewPartialProcesses.OptimalHeight + layoutItemPartialProcesses.Padding.Height : 0;
            height += layoutItemSystemicProcesses.Visible ? gridViewSystemicProcesses.OptimalHeight + layoutItemSystemicProcesses.Padding.Height : 0;
            height += layoutItemWarning.Visible ? layoutItemWarning.Height : 0;
            return height;
         }
      }

      public override void Repaint()
      {
         gridViewPartialProcesses.LayoutChanged();
         gridViewSystemicProcesses.LayoutChanged();
      }

      private void rowPartialStyle(object sender, RowStyleEventArgs e)
      {
         var partialProcessSelectionDTO = _gridViewPartialBinder.ElementAt(e.RowHandle);
         if (partialProcessSelectionDTO == null) return;
         var allCompoundProteins = _presenter.AllCompoundProcesses();
         gridViewPartialProcesses.AdjustAppearance(e, allCompoundProteins.Count() > 1);
      }

      private void rowSystemicStyle(object sender, RowStyleEventArgs e)
      {
         var systemicProcessSelectionDTO = _gridViewSystemicBinder.ElementAt(e.RowHandle);
         if (systemicProcessSelectionDTO == null) return;
         var allSystemicProcesses = _presenter.AllSystemicProcessesFor(systemicProcessSelectionDTO);
         gridViewSystemicProcesses.AdjustAppearance(e, allSystemicProcesses.Count() > 1);
      }

      private RepositoryItem repositoryItemForCompoundProcesses(TPartialProcessDTO simulationPartialProcessSelectionDTO)
      {
         return RepositoryItemFor(_presenter.AllCompoundProcesses(), _compoundMoleculeRepository);
      }

      private void configureCompoundProcessesRepository(BaseEdit baseEdit, TPartialProcessDTO simulationPartialProcessSelectionDTO)
      {
         ConfigureBaseEdit(baseEdit, _presenter.AllCompoundProcesses());
      }

      private RepositoryItem repositoryItemForSystemicProcesses(SimulationSystemicProcessSelectionDTO simulationSystemicProcessSelectionDTO)
      {
         return RepositoryItemFor(_presenter.AllSystemicProcessesFor(simulationSystemicProcessSelectionDTO), _systemicProcessRepository);
      }

      private void configureSystemicProcessesRepository(BaseEdit baseEdit, SimulationSystemicProcessSelectionDTO simulationSystemicProcessSelectionDTO)
      {
         ConfigureBaseEdit(baseEdit, _presenter.AllSystemicProcessesFor(simulationSystemicProcessSelectionDTO));
      }

      protected void ConfigureBaseEdit<T>(BaseEdit baseEdit, IEnumerable<T> allItems)
      {
         var list = allItems.ToList();
         baseEdit.FillComboBoxEditorWith(list);
         baseEdit.Enabled = (list.Count > 1);
      }

      protected RepositoryItem RepositoryItemFor<T>(IEnumerable<T> allItems, RepositoryItem listRepositoryItems)
      {
         var list = allItems.ToList();
         if (list.Count > 1)
            return listRepositoryItems;
         return _repositoryItemDisabled;
      }
   }
}