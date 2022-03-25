using System.Drawing;
using DevExpress.XtraTab;
using OSPSuite.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Extensions;
using OSPSuite.UI.Views;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.UI.Views.Individuals
{
   public partial class ScaleIndividualView : WizardView, IScaleIndividualView
   {
      private ScreenBinder<ObjectBaseDTO> _scaleIndividualPropertiesBinder;

      public ScaleIndividualView(Shell shell) : base(shell)
      {
         InitializeComponent();
         ClientSize = new Size(UIConstants.Size.INDIVIDUAL_VIEW_WIDTH, UIConstants.Size.INDIVIDUAL_VIEW_HEIGHT);
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
         ApplicationIcon = ApplicationIcons.ScaleIndividual;
         layoutItemIndividualName.Text = PKSimConstants.UI.Name.FormatForLabel();
      }
   }
}