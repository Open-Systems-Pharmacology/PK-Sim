using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.UI.Services;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout.Utils;

using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.UI.Views.Compounds
{
   public partial class CreateSystemicProcessView : CreateProcessView, ICreateSystemicProcessView
   {
      private ICreateSystemicProcessPresenter _presenter;
      private ScreenBinder<SystemicProcessDTO> _propertiesBinder;

      public CreateSystemicProcessView(IImageListRetriever imageListRetriever, UserLookAndFeel lookAndFeel, Shell shell): base(imageListRetriever, lookAndFeel, shell)
      {
         InitializeComponent();
      }

      public void AttachPresenter(ICreateSystemicProcessPresenter presenter)
      {
         _presenter = presenter;
         _createProcessPresenter = _presenter;
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         _propertiesBinder = new ScreenBinder<SystemicProcessDTO>();

         _propertiesBinder.Bind(dto => dto.SystemicProcessType)
            .To(tbSystemicProcessType);

         _propertiesBinder.Bind(dto => dto.DataSource)
            .To(tbDataSource);

         _propertiesBinder.Bind(dto => dto.Species)
            .To(cbSpecies)
            .WithImages(species => _imageListRetriever.ImageIndex(species.Icon))
            .WithValues(dto => _presenter.AllSpecies())
            .AndDisplays(species => species.DisplayName)
            .OnValueUpdating += (o, e) => OnEvent(() => _presenter.SpeciesChanged(e.NewValue));

         RegisterValidationFor(_propertiesBinder);
      }

      public void BindTo(SystemicProcessDTO systemicProcessDTO)
      {
         _propertiesBinder.BindToSource(systemicProcessDTO);
      }
      public override bool HasError
      {
         get { return _propertiesBinder.HasError || base.HasError; }
      }

      protected override void SetActiveControl()
      {
         ActiveControl = tbDataSource;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemProtein.Visibility = LayoutVisibilityConvertor.FromBoolean(false);
         tbSystemicProcessType.Enabled = false;
      }
   }
}