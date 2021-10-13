using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.UI.Services;
using OSPSuite.UI.Extensions;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using PKSim.Assets;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;
using OSPSuite.Assets;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views
{
   public partial class BuildingBlockSelectionView : BaseUserControl, IBuildingBlockSelectionView
   {
      private readonly IImageListRetriever _imageListRetriever;
      private readonly IToolTipCreator _toolTipCreator;
      private IBuildingBlockSelectionPresenter _presenter;
      private readonly ScreenBinder<BuildingBlockSelectionDTO> _screenBinder = new ScreenBinder<BuildingBlockSelectionDTO>();
      private readonly ToolTipController _toolTipController = new ToolTipController();

      public BuildingBlockSelectionView(IImageListRetriever imageListRetriever, IToolTipCreator toolTipCreator)
      {
         _imageListRetriever = imageListRetriever;
         _toolTipCreator = toolTipCreator;
         InitializeComponent();
         initializeResource();
         _toolTipController.Initialize(_imageListRetriever);
         cbBuildingBlocks.ToolTipController = _toolTipController;
         _toolTipController.GetActiveObjectInfo += onToolTipControllerGetActiveObjectInfo;
      }

      private void onToolTipControllerGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         OnEvent(() =>
         {
            if (e.SelectedControl != cbBuildingBlocks) return;

            var superToolTip = _toolTipCreator.ToolTipFor(_presenter.ToolTipPartsFor(cbBuildingBlocks.SelectedIndex));
            e.Info = _toolTipCreator.ToolTipControlInfoFor(_presenter.BuildingBlock, superToolTip);
         });
      }

      private void cleanup()
      {
         _screenBinder.Dispose();
         _toolTipController.GetActiveObjectInfo -= onToolTipControllerGetActiveObjectInfo;
         _toolTipController.RemoveClientControl(cbBuildingBlocks);
      }

      public void AttachPresenter(IBuildingBlockSelectionPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.BuildingBlock)
            .To(cbBuildingBlocks)
            .WithImages(x => _imageListRetriever.ImageIndex(_presenter.IconFor(x)))
            .WithValues(x => _presenter.AllAvailableBlocks)
            .AndDisplays(block => _presenter.DisplayFor(block));

         RegisterValidationFor(_screenBinder);

         _screenBinder.Changed += () => _presenter.ViewChanged();
         btnCreateBuildingBlock.Click += (o, e) => OnEvent(_presenter.CreateBuildingBlock);
         //TODO use the right overload
         btnLoadBuildingBlock.Click += (o, e) => OnEvent(()=>_presenter.LoadBuildingBlockAsync());
      }

      public void BindTo(BuildingBlockSelectionDTO buildingBlockSelectionDTO)
      {
         _screenBinder.BindToSource(buildingBlockSelectionDTO);
         btnCreateBuildingBlock.ToolTip = PKSimConstants.UI.CreateBuildingBlockHint(buildingBlockSelectionDTO.BuildingBockType);
         btnLoadBuildingBlock.ToolTip = PKSimConstants.UI.LoadBuildingBlockHint(buildingBlockSelectionDTO.BuildingBockType);
         _presenter.ViewChanged();
      }

      public bool DisplayIcons
      {
         set
         {
            if (value)
            {
               cbBuildingBlocks.SetImages(_imageListRetriever);
            }
            else
            {
               cbBuildingBlocks.ClearImages();
            }
         }
      }

      public void RefreshBuildingBlockList()
      {
         _screenBinder.RefreshListElements();
      }

      public override bool HasError => _screenBinder.HasError;

      private void initializeResource()
      {
         btnCreateBuildingBlock.InitWithImage(ApplicationIcons.Create, imageLocation: ImageLocation.MiddleCenter);
         btnLoadBuildingBlock.InitWithImage(ApplicationIcons.LoadFromTemplate, imageLocation: ImageLocation.MiddleCenter);
         layoutItemCreate.AdjustButtonSizeWithImageOnly();
         layoutItemLoad.AdjustButtonSizeWithImageOnly();
         cbBuildingBlocks.Properties.AllowHtmlDraw = DefaultBoolean.True;
      }
   }
}