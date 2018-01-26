using DevExpress.XtraLayout.Utils;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using PKSim.Presentation.DTO.Events;
using PKSim.Presentation.Presenters.Events;
using PKSim.Presentation.Views.Events;

namespace PKSim.UI.Views.Events
{
   public partial class EventSettingsView : BaseUserControl, IEventSettingsView
   {
      private IEventSettingsPresenter _presenter;
      private readonly ScreenBinder<EventDTO> _screenBinder;

      public EventSettingsView()
      {
         InitializeComponent();
         _screenBinder = new ScreenBinder<EventDTO>();
      }

      public void AttachPresenter(IEventSettingsPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.Description)
            .To(lblDescription);

         _screenBinder.Bind(x => x.Template)
            .To(cbTemplateName)
            .WithValues(dto => _presenter.AllTemplates())
            .AndDisplays(eventTemplate => _presenter.DisplayNameFor(eventTemplate))
            .Changed += () => _presenter.OnTemplateChanged();
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         lblDescription.AsDescription();
      }

      public void BindTo(EventDTO eventDTO)
      {
         _screenBinder.BindToSource(eventDTO);
      }

      public void AddParameterView(IView view)
      {
         panelParameters.FillWith(view);
      }

      public bool EventTemplateVisible
      {
         set => layoutItemTemplate.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
         get => LayoutVisibilityConvertor.ToBoolean(layoutItemTemplate.Visibility);
      }
   }
}