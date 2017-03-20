using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using DevExpress.XtraEditors.Controls;
using PKSim.Assets;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;
using OSPSuite.Presentation.Extensions;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views.Core
{
   public partial class SelectFileView : BaseModalView, ISelectFileView
   {
      private ISelectFilePresenter _presenter;
      private readonly ScreenBinder<FileSelection> _screenBinder;

      public SelectFileView()
      {
         InitializeComponent();
         _screenBinder = new ScreenBinder<FileSelection>();
      }

      public void AttachPresenter(ISelectFilePresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.FilePath)
            .To(btnFileSelection);

         _screenBinder.Bind(x => x.Description)
            .To(tbDescription);

         btnFileSelection.ButtonClick += (o, e) => OnEvent(_presenter.PerformFileSelection);
         RegisterValidationFor(_screenBinder, NotifyViewChanged);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         btnFileSelection.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
         SetFileSelectionCaption(PKSimConstants.UI.FilePath);
         layoutItemDescription.Text = PKSimConstants.UI.Description.FormatForLabel();
      }

      public void SetFileSelectionCaption(string caption)
      {
         layoutItemFileSelection.Text = caption.FormatForLabel();
      }

      public void BindTo(FileSelection fileSelection)
      {
         _screenBinder.BindToSource(fileSelection);
      }

      public override bool HasError
      {
         get { return _screenBinder.HasError; }
      }
   }
}