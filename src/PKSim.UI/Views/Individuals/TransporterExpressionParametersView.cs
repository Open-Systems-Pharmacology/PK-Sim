using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.UI.Views.Individuals
{
   public partial class TransporterExpressionParametersView : ExpressionParametersView<TransporterExpressionParameterDTO>,
      ITransporterExpressionParametersView
   {
      private IGridViewColumn _colDirection;

      public TransporterExpressionParametersView(IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever) :
         base(toolTipCreator, imageListRetriever)
      {
         InitializeComponent();
      }

      private ITransporterExpressionParametersPresenter transporterExpressionParametersPresenter =>
         _presenter.DowncastTo<ITransporterExpressionParametersPresenter>();

      public void AttachPresenter(ITransporterExpressionParametersPresenter presenter)
      {
         base.AttachPresenter(presenter);
      }

      public override void InitializeBinding()
      {
         InitializeGroupBinding();
         InitializeContainerBinding();
         initializeDirectionBinding();
         InitializeParameterNameBinding();
         InitializeValueBinding();
         InitializeShowInitialConcentrationBinding();
      }

      private void initializeDirectionBinding()
      {
         _colDirection = _gridViewBinder.Bind(item => item.TransportDirection)
            .WithRepository(getTransporterMembraneRepository)
            .WithEditorConfiguration(editTransporterMembraneTypeRepository)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithCaption(PKSimConstants.UI.TransportDirection)
            .WithOnValueUpdating((o, e) => OnEvent(() => transporterExpressionParametersPresenter.SetTransportDirection(o, e.NewValue)));

         _colDirection.XtraColumn.OptionsColumn.AllowMerge = DefaultBoolean.False;
      }

      private RepositoryItem getTransporterMembraneRepository(TransporterExpressionParameterDTO expressionParameterDTO)
      {
         var allMembranesTypes = transporterExpressionParametersPresenter.AllTransportDirectionsFor(expressionParameterDTO);
         var transportDirection = expressionParameterDTO.TransportDirection;
         var displayName = transportDirection.DisplayName;

         var repositoryItemImageComboBox = new UxRepositoryItemImageComboBox(_gridView, _imageListRetriever)
            {ReadOnly = (allMembranesTypes.Count == 1), AllowDropDownWhenReadOnly = DefaultBoolean.False};
         if (repositoryItemImageComboBox.ReadOnly)
            repositoryItemImageComboBox.Buttons.Clear();


         var comboBoxItem = new ImageComboBoxItem(displayName, transportDirection, transportDirection.Icon.Index);
         repositoryItemImageComboBox.Items.Add(comboBoxItem);
         return repositoryItemImageComboBox;
      }

      private void editTransporterMembraneTypeRepository(BaseEdit editor, TransporterExpressionParameterDTO containerDTO)
      {
         var allMembranesTypes = transporterExpressionParametersPresenter.AllTransportDirectionsFor(containerDTO);
         if (allMembranesTypes.Count == 1)
            return;

         editor.FillImageComboBoxEditorWith(allMembranesTypes, x => x.Icon.Index, x => x.Description);
      }

      protected override SuperToolTip GetToolTipFor(TransporterExpressionParameterDTO expressionParameterDTO, GridHitInfo hi)
      {
         if (hi.Column != _colDirection.XtraColumn)
            return base.GetToolTipFor(expressionParameterDTO, hi);

         //Show direction tool tio
         return _toolTipCreator.ToolTipFor(expressionParameterDTO);
      }
   }
}