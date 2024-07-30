using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout.Utils;
using OSPSuite.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Presentation.Extensions;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using PKSim.Assets;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Populations;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.Populations;
using PKSim.UI.Extensions;
using static OSPSuite.UI.UIConstants.Size;

namespace PKSim.UI.Views.Populations
{
   public partial class ImportPopulationSettingsView : BaseUserControl, IImportPopulationSettingsView
   {
      private IImportPopulationSettingsPresenter _presenter;
      private readonly UxBuildingBlockSelection _uxIndividualSelection;
      private readonly ScreenBinder<ImportPopulationSettingsDTO> _screenBinder;
      private readonly GridViewBinder<PopulationFileSelectionDTO> _gridViewBinder;
      private readonly RepositoryItemButtonEdit _removeButtonRepository = new UxRemoveButtonRepository();
      private readonly RepositoryItemButtonEdit _filePathRepository = new RepositoryItemButtonEdit();
      private readonly RepositoryItemPictureEdit _statusIconRepository = new RepositoryItemPictureEdit();

      public ImportPopulationSettingsView()
      {
         InitializeComponent();
         gridView.AllowsFiltering = false;
         gridView.ShowRowIndicator = false;
         gridView.OptionsSelection.EnableAppearanceFocusedRow = true;

         _uxIndividualSelection = new UxBuildingBlockSelection();
         _screenBinder = new ScreenBinder<ImportPopulationSettingsDTO>();
         _filePathRepository.Buttons[0].Kind = ButtonPredefines.Ellipsis;

         _gridViewBinder = new GridViewBinder<PopulationFileSelectionDTO>(gridView)
         {
            BindingMode = BindingMode.TwoWay
         };
      }

      public override void InitializeBinding()
      {
         layoutItemIndividual.FillWith(_uxIndividualSelection);
         _screenBinder.Bind(x => x.Individual)
            .To(_uxIndividualSelection)
            .OnValueUpdating += (o, e) => OnEvent(_presenter.IndividualSelectionChanged, e.NewValue);

         RegisterValidationFor(_screenBinder, NotifyViewChanged);

         _gridViewBinder.Bind(x => x.Image)
            .WithCaption(Captions.EmptyColumn)
            .WithRepository(dto => _statusIconRepository)
            .WithFixedWidth(EMBEDDED_BUTTON_WIDTH);

         _gridViewBinder.Bind(x => x.FilePath)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(x => _filePathRepository)
            .WithCaption(PKSimConstants.UI.FilePath);

         _gridViewBinder.Bind(x => x.Count)
            .WithFixedWidth(UIConstants.Size.PARAMETER_WIDTH)
            .WithFormat(new NullIntParameterFormatter())
            .AsReadOnly();

         _gridViewBinder.AddUnboundColumn()
            .WithCaption(Captions.EmptyColumn)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(x => _removeButtonRepository)
            .WithFixedWidth(EMBEDDED_BUTTON_WIDTH * 2);

         gridView.FocusedRowChanged += (o, e) => OnEvent(selectedRowChanged, e);
         _removeButtonRepository.ButtonClick += (o, e) => OnEvent(_presenter.RemoveFile, _gridViewBinder.FocusedElement);
         _filePathRepository.ButtonClick += (o, e) => OnEvent(_presenter.ChangePath, _gridViewBinder.FocusedElement);
         btnAddFile.Click += (o, e) => OnEvent(_presenter.AddFile, notifyViewChanged: true);

         _gridViewBinder.Changed += NotifyViewChanged;
      }

      private void selectedRowChanged(FocusedRowChangedEventArgs e)
      {
         var fileSelectionDTO = _gridViewBinder.ElementAt(e.FocusedRowHandle);
         if (fileSelectionDTO == null) return;
         tbLog.Lines = fileSelectionDTO.Messages.ToArray();
      }

      public void AttachPresenter(IImportPopulationSettingsPresenter presenter)
      {
         _presenter = presenter;
      }

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.Population;

      public void BindTo(ImportPopulationSettingsDTO importPopulationSettingsDTO)
      {
         _gridViewBinder.BindToSource(importPopulationSettingsDTO.PopulationFiles);
         _screenBinder.BindToSource(importPopulationSettingsDTO);
         selectFirstRow(importPopulationSettingsDTO.PopulationFiles);
      }

      private void selectFirstRow(IList<PopulationFileSelectionDTO> populationFiles)
      {
         if (!populationFiles.Any()) return;
         gridView.FocusedRowHandle = _gridViewBinder.RowHandleFor(populationFiles.First());
      }

      public bool CreatingPopulation
      {
         set => layoutGroupImportPopulation.Enabled = !value;
         get => !layoutGroupImportPopulation.Enabled;
      }

      public void UpdateLayoutForEditing()
      {
         CreatingPopulation = true;
         var visibility = LayoutVisibilityConvertor.FromBoolean(false);
         layoutItemIndividual.Visibility = visibility;
         layoutItemDescription.Visibility = visibility;
         layoutItemButtonAdd.Visibility = visibility;
         emptySpaceItem.Visibility = visibility;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemButtonAdd.AdjustLargeButtonSize(layoutControl);
         btnAddFile.InitWithImage(ApplicationIcons.Create, PKSimConstants.UI.AddFile);
         layoutItemIndividual.Text = PKSimConstants.UI.BasedOnIndividual.FormatForLabel();
         Caption = PKSimConstants.UI.ImportPopulationSettings;
         lblDescription.AsDescription();
         lblDescription.Text = PKSimConstants.UI.ImportPopulationSettingsDescription;
      }

      public override bool HasError => _screenBinder.HasError || _gridViewBinder.HasError;
   }
}