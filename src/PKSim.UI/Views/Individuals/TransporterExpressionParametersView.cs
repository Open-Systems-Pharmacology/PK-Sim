using System.Linq;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
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
         InitializeDirectionBinding();
         InitializeParameterNameBinding();
         InitializeValueBinding();
         InitializeShowInitialConcentrationBinding();
      }

      private void InitializeDirectionBinding()
      {
         _colDirection = _gridViewBinder.Bind(item => item.TransportDirection)
            .WithRepository(getTransporterMembraneRepository)
            .WithEditorConfiguration(editTransporterMembraneTypeRepository)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithCaption(PKSimConstants.UI.Direction)
            .WithOnValueUpdating((o, e) => OnEvent(() => transporterExpressionParametersPresenter.SetTransportDirection(o, e.NewValue)));

         _colDirection.XtraColumn.OptionsColumn.AllowMerge = DefaultBoolean.False;

      }

      private string transportDirectionDisplayName(TransportDirection transportDirection, TransporterExpressionParameterDTO expressionParameterDTO)
      {
         return $"{expressionParameterDTO.ContainerPathDTO.DisplayName} ({transportDirection})";
      }

      private RepositoryItem getTransporterMembraneRepository(TransporterExpressionParameterDTO expressionParameterDTO)
      {
         var allMembranesTypes = transporterExpressionParametersPresenter.AllTransportDirectionsFor(expressionParameterDTO);
         var displayName = expressionParameterDTO.TransportDirection== TransportDirection.None ? "" : expressionParameterDTO.TransportDirection.ToString();

         var repositoryItemImageComboBox = new UxRepositoryItemImageComboBox(_gridView, _imageListRetriever)
            {ReadOnly = (allMembranesTypes.Count == 1), AllowDropDownWhenReadOnly = DefaultBoolean.False};
         if (repositoryItemImageComboBox.ReadOnly)
            repositoryItemImageComboBox.Buttons.Clear();


         var comboBoxItem = new ImageComboBoxItem(displayName, expressionParameterDTO.TransportDirection,
            transporterExpressionParametersPresenter.TransporterDirectionIconFor(expressionParameterDTO.TransportDirection).Index);
         repositoryItemImageComboBox.Items.Add(comboBoxItem);
         return repositoryItemImageComboBox;
      }

      private void editTransporterMembraneTypeRepository(BaseEdit editor, TransporterExpressionParameterDTO containerDTO)
      {
         var allMembranesTypes = transporterExpressionParametersPresenter.AllTransportDirectionsFor(containerDTO);
         if (allMembranesTypes.Count == 1)
            return;

         editor.FillImageComboBoxEditorWith(allMembranesTypes, x => transporterExpressionParametersPresenter.TransporterDirectionIconFor(x).Index,
            x => transportDirectionDisplayName(x, containerDTO));
      }
   }
}