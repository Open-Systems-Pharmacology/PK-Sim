using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using PKSim.Assets;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Populations;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.Populations;

namespace PKSim.UI.Views.Populations
{
   public partial class ExtractIndividualsFromPopulationView : BaseModalView, IExtractIndividualsFromPopulationView
   {
      private IExtractIndividualsFromPopulationPresenter _presenter;
      private readonly ScreenBinder<ExtractIndividualsDTO> _screenBinder;

      public ExtractIndividualsFromPopulationView(IShell shell):base(shell)
      {
         InitializeComponent();
         _screenBinder = new ScreenBinder<ExtractIndividualsDTO>();
      }

      public void AttachPresenter(IExtractIndividualsFromPopulationPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         _screenBinder.Bind(x => x.NamingPattern)
            .To(tbNamingPattern)
            .Changing += updateOutput;

         _screenBinder.Bind(x => x.IndividualIdsExpression)
            .To(tbIndividualIdsExpression)
            .Changing += updateOutput;


         RegisterValidationFor(_screenBinder, NotifyViewChanged);
      }

      private void updateOutput()
      {
         OnEvent(() =>
         {
            _presenter.UpdateGeneratedOutput(tbNamingPattern.EditValue?.ToString(), tbIndividualIdsExpression.EditValue?.ToString());
         });
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemNamingPatternDescription.TextVisible = false;
         layoutItemIndividualIdsDescription.TextVisible = false;
         layoutItemOutput.TextVisible = false;
         layoutItemPopulationDescription.TextVisible = false;

         layoutItemNamingPattern.Text = PKSimConstants.UI.NamingPattern.FormatForLabel();
         lblNamingPatternDescription.AsDescription();
         lblNamingPatternDescription.Text = PKSimConstants.UI.IndividualExtractionNamingPatternDescription(IndividualExtractionOptions.POPULATION_NAME, IndividualExtractionOptions.INDIVIDUAL_ID).FormatForDescription();
         layoutItemIndividualIds.Text = PKSimConstants.UI.IndividualIds.FormatForLabel();
         lblIdividualIdsDescription.Text = PKSimConstants.UI.IndividualIdsDescription.FormatForDescription();
         lblIdividualIdsDescription.AsDescription();
         lblPopulationDescription.AsDescription();
         Icon = ApplicationIcons.Population;
         tbOutput.ReadOnly = true;
      }

      public void BindTo(ExtractIndividualsDTO extractIndividualDTO)
      {
         _screenBinder.BindToSource(extractIndividualDTO);
      }

      public string PopulationDescription
      {
         set => lblPopulationDescription.Text = value.FormatForDescription();
      }

      public void UpdateGeneratedOutputDescription(int count, IReadOnlyList<string> individualNames, string populationName)
      {
         var lines = new List<string>{PKSimConstants.UI.NumberOfIndividualsToExtract(count, populationName), string.Empty};
         lines.AddRange(individualNames);
         tbOutput.Lines = lines.ToArray();
      }

      public override bool HasError => _screenBinder.HasError;
   }
}