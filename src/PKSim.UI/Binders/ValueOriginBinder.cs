using System;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using OSPSuite.Core.Domain;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.Presenters;

namespace PKSim.UI.Binders
{
   public class ValueOriginBinder<T> : IDisposable where T : IWithValueOrigin
   {
      private readonly IValueOriginPresenter _valueOriginPresenter;
      private readonly IImageListRetriever _imageListRetriever;
      private readonly IToolTipCreator _toolTipCreator;
      private GridViewBinder<T> _gridViewBinder;
      private readonly RepositoryItemPopupContainerEdit _repositoryItemPopupContainerEdit = new RepositoryItemPopupContainerEdit();
      private readonly PopupContainerControl _popupControl = new PopupContainerControl();
      private UxGridView _gridView;
      private Action<T, ValueOrigin> _onValueOriginUpdated;
      private IGridViewColumn _valueOriginColumn;

      public ValueOriginBinder(IValueOriginPresenter valueOriginPresenter, IImageListRetriever imageListRetriever, IToolTipCreator toolTipCreator)
      {
         _valueOriginPresenter = valueOriginPresenter;
         _imageListRetriever = imageListRetriever;
         _toolTipCreator = toolTipCreator;
         _repositoryItemPopupContainerEdit.Buttons[0].Kind = ButtonPredefines.Combo;
         _repositoryItemPopupContainerEdit.PopupControl = _popupControl;
         _repositoryItemPopupContainerEdit.CloseOnOuterMouseClick = false;
         _repositoryItemPopupContainerEdit.QueryDisplayText += (o, e) => queryDisplayText(e);
         _repositoryItemPopupContainerEdit.CloseUp += (o, e) => closeUp(e);
         _repositoryItemPopupContainerEdit.CloseUpKey = new KeyShortcut(Keys.Enter);
         _popupControl.FillWith(_valueOriginPresenter.BaseView);
      }

      private void onToolTipControllerGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         var column = _gridView.ColumnAt(e);
         if (!Equals(_valueOriginColumn.XtraColumn, column)) return;

         var withValueOrigin = _gridViewBinder.ElementAt(e);
         if (withValueOrigin == null) return;

         var superToolTip = _toolTipCreator.ToolTipFor(withValueOrigin.ValueOrigin);
         if (superToolTip == null) return;

         e.Info = _toolTipCreator.ToolTipControlInfoFor(withValueOrigin, superToolTip);
      }

      private void closeUp(CloseUpEventArgs e)
      {
         var cancel = e.CloseMode == PopupCloseMode.Cancel;
         updateValueOrigin(cancel);
      }

      private void updateValueOrigin(bool canceled)
      {
         //Ensure that we save the edited value only if edit was not canceled
         if (!canceled && _valueOriginPresenter.ValueOriginChanged)
            _onValueOriginUpdated(_gridViewBinder.FocusedElement, _valueOriginPresenter.UpdatedValueOrigin);

         _gridView.CloseEditor();
      }

      public void InitializeBinding(GridViewBinder<T> gridViewBinder, Action<T, ValueOrigin> onValueOriginUpdated)
      {
         _gridViewBinder = gridViewBinder;
         _gridView = _gridViewBinder.GridView.DowncastTo<UxGridView>();
         _onValueOriginUpdated = onValueOriginUpdated;

         _valueOriginColumn = _gridViewBinder.AutoBind(x => x.ValueOrigin)
            .WithRepository(x => displayRepositoryFor(x.ValueOrigin))
            .WithEditRepository(x => _repositoryItemPopupContainerEdit)
            .WithEditorConfiguration((editor, withValueOrigin) => { _valueOriginPresenter.Edit(withValueOrigin.ValueOrigin); });

         initializeToolTip(_gridView.GridControl);
      }

      private void initializeToolTip(GridControl gridControl)
      {
         if (gridControl == null)
            return;

         if (gridControl.ToolTipController == null)
         {
            var toolTipController = new ToolTipController();
            toolTipController.Initialize(_imageListRetriever);
            gridControl.ToolTipController = toolTipController;
         }

         gridControl.ToolTipController.GetActiveObjectInfo += onToolTipControllerGetActiveObjectInfo;
      }

      private RepositoryItem displayRepositoryFor(ValueOrigin valueOrigin)
      {
         var repositoryItem = new UxRepositoryItemImageComboBox(_gridView, _imageListRetriever);
         repositoryItem.AddItem(valueOrigin, valueOrigin.Type.Icon);
         return repositoryItem;
      }

      private void queryDisplayText(QueryDisplayTextEventArgs e)
      {
         var withValueOrigin = _gridViewBinder.FocusedElement;
         if (withValueOrigin == null) return;
         e.DisplayText = withValueOrigin.ValueOrigin.Display;
      }

      protected virtual void Cleanup()
      {
         _valueOriginPresenter.Dispose();
      }

      public bool ColumnIsValueOrigin(GridColumn column) => Equals(column, _valueOriginColumn.XtraColumn);

      #region Disposable properties

      private bool _disposed;

      public void Dispose()
      {
         if (_disposed) return;

         Cleanup();
         GC.SuppressFinalize(this);
         _disposed = true;
      }

      ~ValueOriginBinder()
      {
         Cleanup();
      }

      #endregion
   }
}