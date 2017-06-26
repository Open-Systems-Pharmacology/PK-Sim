using System.Collections.Generic;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Assets;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using OSPSuite.DataBinding;
using OSPSuite.Presentation.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views
{
   public partial class ApplicationSettingsView : BaseUserControl, IApplicationSettingsView
   {
      private IApplicationSettingsPresenter _presenter;
      private readonly GridViewBinder<SpeciesDatabaseMapDTO> _gridViewBinder;
      private readonly ScreenBinder<IApplicationSettings> _screnBinder;

      public ApplicationSettingsView()
      {
         InitializeComponent();
         gridViewDatabasePath.AllowsFiltering = false;
         gridViewDatabasePath.EditorShowMode = EditorShowMode.Default;
         _gridViewBinder = new GridViewBinder<SpeciesDatabaseMapDTO>(gridViewDatabasePath);
         _screnBinder = new ScreenBinder<IApplicationSettings>();
      }

      public void AttachPresenter(IApplicationSettingsPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Caption = PKSimConstants.UI.Application;
         layoutItemMoBiPath.Text = PKSimConstants.UI.MoBiPath.FormatForLabel();
      }

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.SytemSettings;

      public override void InitializeBinding()
      {
         var pathSelectionRepository = createButtonRepository();

         _gridViewBinder.Bind(x => x.SpeciesDisplayName)
            .WithCaption(PKSimConstants.UI.Species).AsReadOnly();

         _gridViewBinder.Bind(x => x.DatabaseFullPath)
            .WithCaption(PKSimConstants.UI.ExpressionDatabasePath)
            .WithRepository(dto => pathSelectionRepository)
            .WithShowButton(ShowButtonModeEnum.ShowAlways);

         _gridViewBinder.Changed += notifyViewChanged;

         _screnBinder.Bind(x => x.MoBiPath)
            .To(buttonMoBiPath);

         RegisterValidationFor(_screnBinder, statusChangedNotify: notifyViewChanged);
         pathSelectionRepository.ButtonClick += (o, e) => OnEvent(buttonClicked, o, e);
         buttonMoBiPath.ButtonClick += (o, e) => OnEvent(_presenter.SelectMoBiPath);

      }

      private void notifyViewChanged() => _presenter.ViewChanged();

      private RepositoryItemButtonEdit createButtonRepository()
      {
         var buttonRepository = new RepositoryItemButtonEdit();
         //First button is select path
         buttonRepository.Buttons[0].Kind = ButtonPredefines.Ellipsis;
         buttonRepository.Buttons.Add(new EditorButton(ButtonPredefines.Delete));
         return buttonRepository;
      }

      private void buttonClicked(object sender, ButtonPressedEventArgs e)
      {
         var editor = (ButtonEdit) sender;
         var buttonIndex = editor.Properties.Buttons.IndexOf(e.Button);
         if (buttonIndex == 0)
            _presenter.SelectDatabasePathFor(_gridViewBinder.FocusedElement);
         else
            _presenter.RemoveDatabasePathFor(_gridViewBinder.FocusedElement);
      }

      public void BindTo(IEnumerable<SpeciesDatabaseMapDTO> databaseMapDTOs)
      {
         _gridViewBinder.BindToSource(databaseMapDTOs);
      }

      public void BindTo(IApplicationSettings applicationSettings)
      {
         _screnBinder.BindToSource(applicationSettings);
      }

       public override bool HasError => _gridViewBinder.HasError;
   }
}