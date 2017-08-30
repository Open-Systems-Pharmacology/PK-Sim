using PKSim.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views.Core
{
   public partial class BuildingBlockContainerView : BaseModalContainerView
   {
      private ScreenBinder<ObjectBaseDTO> _screenBinder;
      protected IContainerPresenter _presenter;

      public BuildingBlockContainerView(Shell shell) : base(shell)
      {
         InitializeComponent();
      }

      public BuildingBlockContainerView()
      {
         InitializeComponent();
      }

      public override void InitializeBinding()
      {
         _screenBinder = new ScreenBinder<ObjectBaseDTO>();
         _screenBinder.Bind(x => x.Name).To(tbName);

         RegisterValidationFor(_screenBinder);
      }

      protected override bool ShouldCancel()
      {
         return _presenter.ShouldCancel;
      }

      public override bool HasError => _screenBinder.HasError;

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemName.Text = PKSimConstants.UI.Name.FormatForLabel();
      }

      public override void AddSubItemView(ISubPresenterItem subPresenterItem, IView viewToAdd)
      {
         panel.FillWith(viewToAdd);
      }

      public void BindToProperties(ObjectBaseDTO eventDTO)
      {
         _screenBinder.BindToSource(eventDTO);
      }

      protected override void SetActiveControl()
      {
         ActiveControl = tbName;
      }
   }
}