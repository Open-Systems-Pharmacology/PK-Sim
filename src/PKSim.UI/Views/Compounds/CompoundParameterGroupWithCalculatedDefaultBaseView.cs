using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Extensions;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using PKSim.Assets;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using PKSim.UI.Views.Core;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.Compounds
{
   public partial class CompoundParameterGroupWithCalculatedDefaultBaseView<TParameterAlternativeDTO> : CompoundParameterWithDefaultAlternativeBaseView<TParameterAlternativeDTO>, ICompoundParameterGroupWithCalculatedDefaultView where TParameterAlternativeDTO : ParameterAlternativeDTO
   {
      private readonly PopupContainerControl _popupControl = new PopupContainerControl();
      private readonly RepositoryItemPopupContainerEdit _repositoryItemPopupContainerEdit = new RepositoryItemPopupContainerEdit();
      private readonly RepositoryItemTextEdit _repositoryItemConstantParameter = new RepositoryItemTextEdit();

      public CompoundParameterGroupWithCalculatedDefaultBaseView(IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever)
         : base(toolTipCreator, imageListRetriever)
      {
         InitializeComponent();
         _repositoryItemPopupContainerEdit.Buttons[0].Kind = ButtonPredefines.Combo;
         _repositoryItemPopupContainerEdit.PopupControl = _popupControl;
         _repositoryItemPopupContainerEdit.CloseOnOuterMouseClick = false;
         _repositoryItemPopupContainerEdit.QueryDisplayText += (o, e) => OnEvent(queryDisplayText, e);
         _repositoryItemPopupContainerEdit.EditValueChanged += (o, e) => OnEvent(() => _gridView.PostEditor());
      }

      public void SetDynamicParameterView(IView view)
      {
         _popupControl.FillWith(view);
      }

      private void queryDisplayText(QueryDisplayTextEventArgs e)
      {
         e.DisplayText = PKSimConstants.UI.ShowCalculatedValues;
      }

      protected override RepositoryItem GetButtonRepository(ParameterAlternativeDTO alternativeDTO)
      {
         if (alternativeDTO == null || alternativeDTO.IsDefault)
            return _addButtonRepository;

         if (IsCalculatedAlternative(alternativeDTO))
            return _addButtonRepository;

         return _addAndRemoveButtonRepository;
      }

      protected override void OnValueColumnMouseDown(UxGridView gridView, GridColumn col, int rowHandle)
      {
         var parameterAlternativeDTO = _gridViewBinder.ElementAt(rowHandle);
         if (parameterAlternativeDTO == null) return;

         _gridView.EditorShowMode = withCalculatedDefaultPresenter.IsCalculatedAlternative(parameterAlternativeDTO) ? EditorShowMode.Default : EditorShowMode.MouseUp;
      }

      private RepositoryItem getValueRepository(ParameterAlternativeDTO alternativeDTO)
      {
         return IsCalculatedAlternative(alternativeDTO) ? _repositoryItemPopupContainerEdit : _repositoryItemConstantParameter;
      }

      protected bool IsCalculatedAlternative(ParameterAlternativeDTO alternativeDTO)
      {
         return withCalculatedDefaultPresenter.IsCalculatedAlternative(alternativeDTO);
      }

      private void configureRepository(BaseEdit baseEdit, ParameterAlternativeDTO alternativeDTO)
      {
         if (IsCalculatedAlternative(alternativeDTO))
            OnEvent(withCalculatedDefaultPresenter.UpdateCalculatedValue);
         else
            ConfigureValueRepository(baseEdit, alternativeDTO);
      }

      /// <summary>
      ///    To be overriden in all sub class
      /// </summary>
      protected virtual void ConfigureValueRepository(BaseEdit baseEdit, ParameterAlternativeDTO alternativeDTO)
      {
         /*nothing to do here*/
      }

      protected void AddValueBinding(IGridViewAutoBindColumn<PermeabilityAlternativeDTO, double> colValue)
      {
         colValue.WithRepository(getValueRepository)
            .WithEditorConfiguration(configureRepository)
            .WithShowButton(ShowButtonModeEnum.ShowAlways);
      }

      private ICompoundParameterGroupWithCalculatedDefaultPresenter withCalculatedDefaultPresenter
      {
         get { return _presenter.DowncastTo<ICompoundParameterGroupWithCalculatedDefaultPresenter>(); }
      }
   }
}