using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.UI.Services;
using OSPSuite.UI.Extensions;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout.Utils;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Compounds
{
   public partial class BaseCompoundProcessView<TProcess, TProcessDTO> : BaseUserControl
      where TProcess : CompoundProcess
      where TProcessDTO : ProcessDTO<TProcess>
   {
      private readonly IImageListRetriever _imageListRetriever;
      protected readonly UserLookAndFeel _lookAndFeel;
      protected IProcessPresenter<TProcess> _presenter;
      protected readonly ScreenBinder<TProcessDTO> _screenBinder;

      public BaseCompoundProcessView(IImageListRetriever imageListRetriever, UserLookAndFeel lookAndFeel)
      {
         _imageListRetriever = imageListRetriever;
         _lookAndFeel = lookAndFeel;
         InitializeComponent();
         _screenBinder = new ScreenBinder<TProcessDTO>();
      }

      public void AttachPresenter(IProcessPresenter<TProcess> presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.ProcessTypeDisplayName)
            .To(lblProcessType);

         _screenBinder.Bind(x => x.Species)
            .To(cbSpecies)
            .WithImages(s => _imageListRetriever.ImageIndex(s.Icon))
            .WithValues(dto => _presenter.AllSpecies())
            .AndDisplays(s => s.DisplayName)
            .OnValueUpdating += (o, e) => OnEvent(() => _presenter.SpeciesChanged(e.NewValue));

         _screenBinder.Bind(x => x.Description)
            .To(tbDescription)
            .OnValueUpdating += (o, e) => OnEvent(() => _presenter.DescriptionChanged(e.NewValue));
      }

      public bool SpeciesVisible
      {
         set { layoutItemSpecies.Visibility = LayoutVisibilityConvertor.FromBoolean(value); }
      }

      public virtual void BindTo(TProcessDTO processDTO)
      {
         _screenBinder.BindToSource(processDTO);
      }

      public void SetParametersView(IView parametersView)
      {
         panelParameters.FillWith(parametersView);
      }

      public void AdjustParametersHeight(int parametersHeight)
      {
         layoutControl.BeginUpdate();
         layoutItemParameters.AdjustControlHeight(parametersHeight);
         layoutControl.EndUpdate();
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemDescription.InitializeAsHeader(_lookAndFeel, PKSimConstants.UI.Description);
         layoutItemSpecies.InitializeAsHeader(_lookAndFeel, PKSimConstants.UI.Species);
         layoutItemProcessType.InitializeAsHeader(_lookAndFeel, PKSimConstants.UI.ProcessType);
         cbSpecies.SetImages(_imageListRetriever);
      }
   }
}