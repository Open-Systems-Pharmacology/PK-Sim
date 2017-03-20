using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Assets;
using DevExpress.XtraTab;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Extensions;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views.Individuals
{
   public partial class ScaleIndividualView : WizardView, IScaleIndividualView
   {
      private ScreenBinder<ObjectBaseDTO> _scaleIndividualPropertiesBinder;

      public ScaleIndividualView(Shell shell) : base(shell)
      {
         InitializeComponent();
      }

      public override void InitializeBinding()
      {
         _scaleIndividualPropertiesBinder = new ScreenBinder<ObjectBaseDTO>();
         _scaleIndividualPropertiesBinder.Bind(dto => dto.Name)
            .To(tbIndividualName);

         RegisterValidationFor(_scaleIndividualPropertiesBinder, NotifyViewChanged);
      }

      public void BindToProperties(ObjectBaseDTO individualPropertiesDTO)
      {
         _scaleIndividualPropertiesBinder.BindToSource(individualPropertiesDTO);
         NotifyViewChanged();
      }

      public override bool HasError => _scaleIndividualPropertiesBinder.HasError;

      public void AttachPresenter(IScaleIndividualPresenter presenter)
      {
         WizardPresenter = presenter;
      }

      public override XtraTabControl TabControl => tabScaleIndividual;

      public override void InitializeResources()
      {
         base.InitializeResources();
         Icon = ApplicationIcons.ScaleIndividual.WithSize(IconSizes.Size16x16);
         layoutItemIndividualName.Text = PKSimConstants.UI.Name.FormatForLabel();
      }
   }
}