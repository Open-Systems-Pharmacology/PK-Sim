using DevExpress.LookAndFeel;
using DevExpress.XtraLayout.Utils;
using OSPSuite.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Services;
using OSPSuite.UI.Views;
using PKSim.Assets;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using PKSim.UI.Extensions;

namespace PKSim.UI.Views.Compounds
{
   public partial class CreateProcessView : BaseModalView, ICreateProcessView
   {
      protected readonly IImageListRetriever _imageListRetriever;
      protected readonly UserLookAndFeel _lookAndFeel;
      private ScreenBinder<ICreateProcessPresenter> _templateBinder;
      protected ICreateProcessPresenter _createProcessPresenter;

      public CreateProcessView(IImageListRetriever imageListRetriever, UserLookAndFeel lookAndFeel, Shell shell) : base(shell)
      {
         _imageListRetriever = imageListRetriever;
         _lookAndFeel = lookAndFeel;
         InitializeComponent();
      }

      public override void InitializeBinding()
      {
         _templateBinder = new ScreenBinder<ICreateProcessPresenter>();

         _templateBinder.Bind(dto => dto.SelectedProcessTemplate)
            .To(lbProcessType)
            .WithValues(p => p.AllTemplates)
            .OnValueUpdating += (o, e) => OnEvent(() => selectedProcessTypeChanged(e.NewValue));
      }

      public void BindProcessTypes()
      {
         _templateBinder.BindToSource(_createProcessPresenter);
      }

      public void AddParametersView(IView parametersView)
      {
         panelParameters.FillWith(parametersView);
      }

      public bool SpeciesVisible
      {
         set
         {
            layoutControl.DoInBatch(() =>
            {
               layoutItemSpecies.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
               layoutItemSpeciesDescription.Visibility = layoutItemSpecies.Visibility;
               emptySpaceSpecies.Visibility = layoutItemSpecies.Visibility;
            });
         }
      }

      public string TemplateDescription
      {
         set => lblProcessDescription.Text = value.FormatForDescription();
      }

      public void SetIcon(ApplicationIcon icon)
      {
         Icon = icon;
      }

      public void AdjustParametersHeight(int optimalHeight)
      {
         layoutControl.DoInBatch(() => layoutItemParameters.AdjustControlHeight(optimalHeight));
      }

      public string ProteinCaption
      {
         set => layoutItemProtein.Text = value.FormatForLabel();
      }

      public override bool HasError => _templateBinder.HasError;

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemProtein.InitializeAsHeader(_lookAndFeel, PKSimConstants.UI.Protein);
         layoutItemDataSource.InitializeAsHeader(_lookAndFeel, PKSimConstants.UI.DataSource);
         layoutItemProcessType.InitializeAsHeader(_lookAndFeel, PKSimConstants.UI.ProcessType);
         layoutItemSystemicProcessType.InitializeAsHeader(_lookAndFeel, PKSimConstants.UI.SystemicProcess);
         layoutItemSpecies.InitializeAsHeader(_lookAndFeel, PKSimConstants.UI.Species);
         cbSpecies.SetImages(_imageListRetriever);
         lblProcessDescription.AsDescription();
         lblSpeciesDescription.AsDescription();
         lblSpeciesDescription.Text = PKSimConstants.UI.CompoundProcessSpeciesDescription.FormatForDescription();
         lblDataSourceDescription.AsDescription();
         lblDataSourceDescription.Text = PKSimConstants.UI.CompoundProcessDataSourceDescription.FormatForDescription();
         layoutControl.InitializeDisabledColors(_lookAndFeel);
         OKOnEnter = false;
      }

      private void selectedProcessTypeChanged(CompoundProcessDTO compoundProcessDTO)
      {
         //this action might be called when binding to the list even if the selection did not change
         _createProcessPresenter.ChangeProcessType(compoundProcessDTO);
      }
   }
}