using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using OSPSuite.Assets;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using static OSPSuite.UI.UIConstants.Size;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationCompoundProtocolEventView : BaseGridViewOnlyUserControl, ISimulationCompoundProtocolEventView
   {
      private ISimulationCompoundProtocolEventPresenter _presenter;
      private readonly GridViewBinder<EventPlaceholderMappingDTO> _gridViewBinder;
      private readonly UxRepositoryItemComboBox _repositoryForEvent;
      private IGridViewColumn _eventKeyColumn;

      public SimulationCompoundProtocolEventView()
      {
         InitializeComponent();
         _repositoryForEvent = new UxRepositoryItemComboBox(gridView) { AllowHtmlDraw = DefaultBoolean.True };
         _gridViewBinder = new GridViewBinder<EventPlaceholderMappingDTO>(gridView);
         gridView.HorzScrollVisibility = ScrollVisibility.Never;
         layoutControl.AutoScroll = false;
      }

      public void AttachPresenter(ISimulationCompoundProtocolEventPresenter presenter)
      {
         _presenter = presenter;
      }

      public override bool HasError => _gridViewBinder.HasError;

      public void BindTo(IEnumerable<EventPlaceholderMappingDTO> eventMappings)
      {
         _gridViewBinder.BindToSource(eventMappings.ToBindingList());
         gridView.BestFitColumns();
         AdjustHeight();
      }

      public void RefreshData()
      {
         gridView.RefreshData();
      }

      public bool EventKeyVisible
      {
         set => _eventKeyColumn.UpdateVisibility(value);
      }

      public bool EventVisible
      {
         get => GridVisible;
         set => GridVisible = value;
      }

      public override void InitializeBinding()
      {
         var createEventButton = createEventButtonRepository();
         var loadEventButton = loadEventButtonRepository();

         _eventKeyColumn = _gridViewBinder.Bind(x => x.EventKey)
            .WithCaption(PKSimConstants.UI.PlaceholderEvent)
            .AsReadOnly();

         _gridViewBinder.AutoBind(x => x.Selection)
            .WithRepository(x => _repositoryForEvent)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithEditorConfiguration(configureEvent);

         _gridViewBinder.AddUnboundColumn()
            .WithCaption(Captions.EmptyColumn)
            .WithFixedWidth(EMBEDDED_BUTTON_WIDTH)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(dto => createEventButton);

         _gridViewBinder.AddUnboundColumn()
            .WithCaption(Captions.EmptyColumn)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithFixedWidth(EMBEDDED_BUTTON_WIDTH)
            .WithRepository(dto => loadEventButton);

         _gridViewBinder.Changed += () => _presenter.ViewChanged();
         createEventButton.ButtonClick += (o, e) => OnEvent(() => _presenter.CreateEventFor(_gridViewBinder.FocusedElement));
         loadEventButton.ButtonClick += (o, e) => OnEvent(() => _presenter.LoadEventForAsync(_gridViewBinder.FocusedElement));
      }

      private RepositoryItemButtonEdit createEventButtonRepository() =>
         new UxRepositoryItemButtonImage(ApplicationIcons.Create, PKSimConstants.UI.CreateBuildingBlockHint(PKSimConstants.ObjectTypes.Event));

      private RepositoryItemButtonEdit loadEventButtonRepository() =>
         new UxRepositoryItemButtonImage(ApplicationIcons.LoadFromTemplate, PKSimConstants.UI.LoadItemFromTemplate(PKSimConstants.ObjectTypes.Event));

      private void configureEvent(BaseEdit baseEdit, EventPlaceholderMappingDTO eventPlaceholderMappingDTO)
      {
         baseEdit.FillComboBoxEditorWith(_presenter.AllEventsFor(eventPlaceholderMappingDTO));
      }
   }
}
