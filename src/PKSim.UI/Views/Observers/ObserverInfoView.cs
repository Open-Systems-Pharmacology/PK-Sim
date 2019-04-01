using DevExpress.XtraRichEdit;
using OSPSuite.UI.Controls;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.DTO.Observers;
using PKSim.Presentation.Presenters.Observers;
using PKSim.Presentation.Views.Observers;

namespace PKSim.UI.Views.Observers
{
   public partial class ObserverInfoView : BaseUserControl, IObserverInfoView
   {
      private IObserverInfoPresenter _presenter;

      public ObserverInfoView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IObserverInfoPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(ObserverDTO observerDTO)
      {
         Clear();
         observerDTO.Details.Each(x => richEditControl.Document.AppendHtmlText($"{x}<br>"));
         richEditControl.Document.CaretPosition = richEditControl.Document.Range.Start;
      }

      public void Clear()
      {
         richEditControl.Document.Text = "";
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         richEditControl.Document.Text = string.Empty;
         richEditControl.ActiveViewType = RichEditViewType.Simple;
      }
   }
}