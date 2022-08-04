using DevExpress.Utils;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Services;
using OSPSuite.UI.Views;
using PKSim.Assets;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.UI.Views.Individuals
{
   public partial class ShowOntogenyDataView : BaseModalView, IShowOntogenyDataView
   {
      private IShowOntogenyDataPresenter _presenter;
      private readonly ScreenBinder<ShowOntogenyDataDTO> _screenBinder;

      public ShowOntogenyDataView()
      {
         InitializeComponent();
      }

      public ShowOntogenyDataView(Shell shell, IImageListRetriever imageListRetriever) : base(shell)
      {
         InitializeComponent();
         _screenBinder = new ScreenBinder<ShowOntogenyDataDTO>();
         listBoxContainer.ToolTipController = new ToolTipController().Initialize(imageListRetriever);
         listBoxContainer.ToolTipController.GetActiveObjectInfo += onToolTipControllerGetActiveObjectInfo;
      }

      private void onToolTipControllerGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         if (e.SelectedControl != listBoxContainer)
            return;

         var index = listBoxContainer.IndexFromPoint(e.ControlMousePosition);
         if (index < 0) return;

         var item = listBoxContainer.GetItem(index);
         e.Info = new ToolTipControlInfo(item, _presenter.GroupDescriptionFor(index));
      }

      public void AttachPresenter(IShowOntogenyDataPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.SelectedOntogeny)
            .To(cbOntogeny)
            .WithValues(dto => _presenter.AllOntogenies())
            .AndDisplays(o => o.DisplayName)
            .Changed += () => OnEvent(_presenter.OntogenyChanged);

         _screenBinder.Bind(x => x.SelectedContainer)
            .To(listBoxContainer)
            .WithValues(dto => _presenter.AllContainers())
            .AndDisplays(x => x.DisplayName)
            .Changed += () => OnEvent(_presenter.ContainerChanged);
      }

      public void BindTo(ShowOntogenyDataDTO showOntogenyDataDTO)
      {
         _screenBinder.BindToSource(showOntogenyDataDTO);
      }

      public void UpdateContainers()
      {
         _screenBinder.RefreshListElements();
      }

      public void AddChart(IView view)
      {
         splitContainer.Panel2.FillWith(view);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         CancelVisible = false;
         Caption = PKSimConstants.UI.ShowingOntogenyData;
         layoutItemOntogeny.Text = PKSimConstants.UI.OntogenyVariabilityLike.FormatForLabel();
         lblDescription.AsDescription();
         lblDescription.Text = PKSimConstants.UI.OntogenyDescription;
      }
   }
}