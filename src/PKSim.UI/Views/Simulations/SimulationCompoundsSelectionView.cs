using System;
using System.Collections.Generic;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Assets;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraLayout.Utils;
using PKSim.Assets;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.UI.Services;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationCompoundsSelectionView : BaseResizableUserControl, ISimulationCompoundsSelectionView
   {
      private readonly IToolTipCreator _toolTipCreator;
      private ISimulationCompoundsSelectionPresenter _presenter;
      private readonly GridViewBinder<CompoundSelectionDTO> _gridViewBinder;

      public SimulationCompoundsSelectionView(IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever)
      {
         _toolTipCreator = toolTipCreator;
         InitializeComponent();
         gridView.AllowsFiltering = false;
         gridView.ShowColumnHeaders = false;
         gridView.ShowRowIndicator = false;
         gridView.ShouldUseColorForDisabledCell = false;
         gridView.OptionsSelection.EnableAppearanceFocusedRow = true;

         _gridViewBinder = new GridViewBinder<CompoundSelectionDTO>(gridView);
         var toolTipController = new ToolTipController();
         toolTipController.Initialize(imageListRetriever);
         toolTipController.GetActiveObjectInfo += onToolTipControllerGetActiveObjectInfo;
         gridControl.ToolTipController = toolTipController;

      }

      private void onToolTipControllerGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         if (e.SelectedControl != gridControl)
            return;

         var compoundSelectionDTO = _gridViewBinder.ElementAt(e);
         if (compoundSelectionDTO == null) return;

         var superToolTip = _toolTipCreator.ToolTipFor(_presenter.ToolTipFor(compoundSelectionDTO));
         e.Info = _toolTipCreator.ToolTipControlInfoFor(compoundSelectionDTO, superToolTip);
      }

      public void AttachPresenter(ISimulationCompoundsSelectionPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(IEnumerable<CompoundSelectionDTO> compoundSelectionDTOs)
      {
         _gridViewBinder.BindToSource(compoundSelectionDTOs);
      }

      public void SetError(string errorText)
      {
         uxHintPanel.NoteText = errorText;
         layoutItemWarning.Visibility = LayoutVisibilityConvertor.FromBoolean(!string.IsNullOrEmpty(errorText));
      }

      public void HideError()
      {
         SetError(string.Empty);
      }

      public override void InitializeBinding()
      {
         _gridViewBinder.Bind(x => x.Selected)
            .WithRepository(selectRepository)
            .WithOnChanged(_presenter.SelectionChanged);

         _gridViewBinder.Changed += NotifyViewChanged;

         btnCreateCompound.Click += (o, e) => OnEvent(_presenter.AddCompound);
         btnLoadCompound.Click += (o, e) => OnEvent(_presenter.LoadCompound);
      }

      private RepositoryItem selectRepository(CompoundSelectionDTO dto)
      {
         return new UxRepositoryItemCheckEdit(gridView) {Caption = dto.DisplayName, GlyphAlignment = HorzAlignment.Near, AllowHtmlDraw = DefaultBoolean.True};
      }

      public override void Repaint()
      {
         gridView.LayoutChanged();
      }

      public override int OptimalHeight
      {
         get
         {
            var warningHeight = layoutItemWarning.Visible ? layoutItemWarning.Height : 1;
            return layoutItemCompounds.Padding.Height + gridView.OptimalHeight + layoutItemAddCompound.Height + warningHeight;
         }
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         initButton(btnCreateCompound, ApplicationIcons.Create, PKSimConstants.UI.CreateBuildingBlockHint);
         initButton(btnLoadCompound, ApplicationIcons.LoadFromTemplate, PKSimConstants.UI.LoadBuildingBlockHint);

         layoutItemAddCompound.AdjustButtonSizeWithImageOnly();
         layoutItemLoadCompound.AdjustButtonSizeWithImageOnly();

         uxHintPanel.Image = ApplicationIcons.ErrorHint;
      }

      private void initButton(SimpleButton button, ApplicationIcon icon, Func<string, string> hint)
      {
         button.InitWithImage(icon, imageLocation: ImageLocation.MiddleCenter, toolTip: hint(PKSimConstants.ObjectTypes.Compound));
      }
   }
}