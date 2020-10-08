using System;
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using OSPSuite.Core.Domain;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Individuals
{
   public partial class IndividualProteinExpressionsViewNew : BaseUserControlWithValueInGrid, IIndividualProteinExpressionsViewNew
   {
      private readonly IImageListRetriever _imageListRetriever;
      private IIndividualProteinExpressionsPresenterNew _presenter;
      private readonly GridViewBinder<ExpressionContainerParameterDTO> _gridViewBinder;
      private IGridViewColumn _colGrouping;
      private IGridViewColumn  _colParameterValue;
      private IGridViewColumn _colParameterName;

      public IndividualProteinExpressionsViewNew(IImageListRetriever imageListRetriever)
      {
         _imageListRetriever = imageListRetriever;
         InitializeComponent();
         gridView.ShouldUseColorForDisabledCell = false;
         gridView.OptionsView.AllowCellMerge =true;
         gridView.GroupFormat = "[#image]{1}";
         //gridView.AllowsFiltering = false;
         _gridViewBinder = new GridViewBinder<ExpressionContainerParameterDTO>(gridView);
         gridView.CustomColumnSort += customColumnSort;
         InitializeWithGrid(gridView);
      }

      //https://github.com/DevExpress-Examples/custom-gridcontrol-how-to-hide-particular-grouprow-headers-footers-t264208/blob/16.1.4%2B/CS/CustomGridControl/MyDataController.cs

      public void AttachPresenter(IIndividualProteinExpressionsPresenterNew presenter)
      {
         _presenter = presenter;
      }

      private void customColumnSort(object sender, CustomColumnSortEventArgs e)
      {
         if (e.Column != _colGrouping.XtraColumn) return;
         var container1 = e.RowObject1 as ExpressionContainerParameterDTO;
         var container2 = e.RowObject2 as ExpressionContainerParameterDTO;
         if (container1 == null || container2 == null) return;
         e.Handled = true;

         e.Result = container1.Sequence.CompareTo(container2.Sequence);
         if (e.Result != 0)
            return;

         if (container1.ContainerName != container2.ContainerName)
            return;

         //Same container and compartment, return
         if (container1.CompartmentName == container2.CompartmentName)
            return;

         //One of the two has an empty compartment. We pull this one ahead
         if (!string.IsNullOrEmpty(container1.CompartmentName) && !string.IsNullOrEmpty(container2.CompartmentName))
            return;

         // -1 will move the container1 above container 2
         e.Result = string.IsNullOrEmpty(container1.CompartmentName) ? -1 : 1;

//         Debug.Print($"{container1} && {container2}  = {e.Result}");
      }

      public override void InitializeBinding()
      {
         _colGrouping = _gridViewBinder.AutoBind(item => item.GroupingPathDTO)
            .WithRepository(dto => configureContainerRepository(dto.GroupingPathDTO))
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .AsReadOnly();
         _colGrouping.XtraColumn.GroupIndex = 0;
         _colGrouping.XtraColumn.SortMode = ColumnSortMode.Custom;

         _gridViewBinder.AutoBind(item => item.ContainerPathDTO)
            .WithRepository(dto => configureContainerRepository(dto.ContainerPathDTO))
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .AsReadOnly();

         _gridViewBinder.AutoBind(item => item.CompartmentPathDTO)
            .WithRepository(dto => configureContainerRepository(dto.CompartmentPathDTO))
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .AsReadOnly();

         _colParameterName = _gridViewBinder.AutoBind(item => item.ParameterName)
            .WithCaption(PKSimConstants.UI.Parameter)
            .AsReadOnly();
         // .WithOnValueUpdating((protein, args) => _presenter.SetRelativeExpression(protein, args.NewValue));

         //TODO AUTOBIND
         _colParameterValue = _gridViewBinder.Bind(item => item.Value)
            .WithOnValueUpdating((dto, args) => dto.Parameter.Parameter.Value = args.NewValue)
            .WithCaption(PKSimConstants.UI.Value);

         _colParameterName.XtraColumn.OptionsColumn.AllowMerge = DefaultBoolean.False;
         _colParameterValue.XtraColumn.OptionsColumn.AllowMerge = DefaultBoolean.False;

         // var col = _gridViewBinder.AutoBind(item => item.RelativeExpressionNorm)
         //    .WithCaption(PKSimConstants.UI.RelativeExpressionNorm)
         //    .WithRepository(x => _progressBarRepository)
         //    .AsReadOnly();
         //
         // //necessary to align center since double value are aligned right by default
         // col.XtraColumn.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
         // col.XtraColumn.DisplayFormat.FormatType = FormatType.None;

      }
      private RepositoryItem configureContainerRepository(PathElement pathElement)
      {
         var containerRepository = new UxRepositoryItemImageComboBox(gridView, _imageListRetriever);
         return containerRepository.AddItem(pathElement, pathElement.IconName);
      }

      public void Clear()
      {
      }

      public void AddMoleculePropertiesView(IView view)
      {
         AddViewTo(layoutItemMoleculeProperties, view);
      }

      public void BindTo(IndividualProteinDTO individualProteinDTO)
      {
         _gridViewBinder.BindToSource(individualProteinDTO.AllExpressionContainerParameters.ToBindingList());
         gridView.BestFitColumns();

      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemMoleculeProperties.TextVisible = false;
         layoutGroupMoleculeProperties.Text = PKSimConstants.UI.Properties;
         layoutGroupMoleculeLocalization.Text = PKSimConstants.UI.Localization;
      }

      // public override bool HasError => _screenBinder.HasError || _gridViewBinder.HasError;

      public override bool HasError =>  _gridViewBinder.HasError;
   }
}
