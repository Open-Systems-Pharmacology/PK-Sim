using System.Collections.Generic;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.Assets;
using OSPSuite.Utility.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout.Utils;
using PKSim.Assets;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using PKSim.UI.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.UI.Controls;
using OSPSuite.Presentation.Extensions;

namespace PKSim.UI.Views.Individuals
{
   public partial class IndividualScalingConfigurationView : BaseUserControl, IIndividualScalingConfigurationView
   {
      private IIndividualScalingConfigurationPresenter _presenter;
      private GridViewBinder<ParameterScalingDTO> _gridViewBinder;
      private readonly ScreenBinder<IIndividualScalingConfigurationPresenter> _screenBinder;
      private readonly RepositoryItemComboBox _scalingMethodRepository;

      public IndividualScalingConfigurationView()
      {
         InitializeComponent();
         _scalingMethodRepository = new UxRepositoryItemComboBox(gridView);
         _screenBinder = new ScreenBinder<IIndividualScalingConfigurationPresenter>();
         gridView.AllowsFiltering = false;
         gridView.CustomDrawEmptyForeground += (o, e) => OnEvent(addMessageInEmptyArea, e);
      }

      public void AttachPresenter(IIndividualScalingConfigurationPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _gridViewBinder = new GridViewBinder<ParameterScalingDTO>(gridView);

         _gridViewBinder.Bind(item => item.ParameterFullPathDisplay)
            .WithCaption(PKSimConstants.UI.Parameter)
            .AsReadOnly();


         _gridViewBinder.Bind(item => item.SourceDefaultValue)
            .WithFormat(dto => dto.SourceParameter.ParameterFormatter())
            .WithCaption(PKSimConstants.UI.SourceDefaultValue)
            .AsReadOnly();

         _gridViewBinder.Bind(item => item.SourceValue)
            .WithFormat(dto => dto.SourceParameter.ParameterFormatter())
            .WithCaption(PKSimConstants.UI.SourceValue)
            .AsReadOnly();

         _gridViewBinder.Bind(item => item.TargetDefaultValue)
            .WithFormat(dto => dto.TargetParameter.ParameterFormatter())
            .WithCaption(PKSimConstants.UI.TargetDefaultValue)
            .AsReadOnly();

         _gridViewBinder.Bind(item => item.TargetScaledValue)
            .WithFormat(dto => new UnitFormatter(dto.SourceParameter.DisplayUnit))
            .WithCaption(PKSimConstants.UI.TargetScaledValue)
            .AsReadOnly();

         _gridViewBinder.Bind(item => item.ScalingMethod)
            .WithCaption(PKSimConstants.UI.ScalingMethod)
            .WithRepository(item => _scalingMethodRepository)
            .WithEditorConfiguration(configureScalingMethodRepository)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .OnValueSet += (o, e) => _presenter.ScalingMethodChanged(o, e.NewValue);

         _screenBinder.Bind(x => x.Weight).To(uxWeight);
      }

      private void configureScalingMethodRepository(BaseEdit activeEditor, ParameterScalingDTO parameterScalingDTO)
      {
         activeEditor.FillComboBoxEditorWith(_presenter.AllScalingMethodsFor(parameterScalingDTO));
      }

      public override string Caption
      {
         get { return PKSimConstants.UI.IndividualScalingConfiguration; }
      }

      public override ApplicationIcon ApplicationIcon
      {
         get { return ApplicationIcons.ScaleIndividual; }
      }

      public void BindTo(IEnumerable<ParameterScalingDTO> parameterScalingsDTO)
      {
         _gridViewBinder.BindToSource(parameterScalingsDTO);
         gridView.BestFitColumns();
      }

      public void BindToWeight()
      {
         _screenBinder.BindToSource(individualScalingConfigurationPresenter);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         uxWeight.Enabled = false;
         layoutItemTargetBodyWeight.Text = PKSimConstants.UI.TargetBodyWeight.FormatForLabel();
      }

      public bool WeightVisible
      {
         get { return LayoutVisibilityConvertor.ToBoolean(layoutItemTargetBodyWeight.Visibility); }
         set
         {
            layoutItemTargetBodyWeight.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
            spaceItemBodyWeight.Visibility = layoutItemTargetBodyWeight.Visibility;
         }
      }

      private void addMessageInEmptyArea(CustomDrawEventArgs e)
      {
         gridView.AddMessageInEmptyArea(e, PKSimConstants.Information.NoParameterAvailableForScaling);
      }

      private IIndividualScalingConfigurationPresenter individualScalingConfigurationPresenter
      {
         get { return _presenter.DowncastTo<IIndividualScalingConfigurationPresenter>(); }
      }
   }
}