using System;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Events;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IPopulationAnalysisParameterFieldsPresenter : IPopulationAnalysisFieldsPresenter
   {
      /// <summary>
      ///    Adds the given <paramref name="parameter" /> as input field for the analysis
      /// </summary>
      void AddParameter(IParameter parameter);

      /// <summary>
      ///    Returns the selected parameter or null if no parameter is selected
      /// </summary>
      IParameter SelectedParameter { get; }

      /// <summary>
      ///    Event is thrown whenever a parameter is being selected
      /// </summary>
      event EventHandler<ParameterFieldSelectedEventArgs> ParameterFieldSelected;

      /// <summary>
      ///    Event is thrown whenever a covariate is being selected
      /// </summary>
      event EventHandler<CovariateFieldSelectedEventArgs> CovariateFieldSelected;

      /// <summary>
      ///    Add a covariate field to the selection
      /// </summary>
      void AddCovariate(string covariate);
   }

   public class PopulationAnalysisParameterFieldsPresenter : PopulationAnalysisFieldsPresenter, IPopulationAnalysisParameterFieldsPresenter
   {
      private readonly IEntitiesInContainerRetriever _parametersRetriever;
      private PathCache<IParameter> _allParameterCache;
      public event EventHandler<ParameterFieldSelectedEventArgs> ParameterFieldSelected = delegate { };
      public event EventHandler<CovariateFieldSelectedEventArgs> CovariateFieldSelected = delegate { };

      public PopulationAnalysisParameterFieldsPresenter(IPopulationAnalysisFieldsView view, IPopulationAnalysesContextMenuFactory contextMenuFactory, IEntitiesInContainerRetriever parametersRetriever, IPopulationAnalysisFieldFactory populationAnalysisFieldFactory, IEventPublisher eventPublisher, IPopulationAnalysisGroupingFieldCreator populationAnalysisGroupingFieldCreator, IPopulationAnalysisTemplateTask populationAnalysisTemplateTask, IDialogCreator dialogCreator, IPopulationAnalysisFieldToPopulationAnalysisFieldDTOMapper fieldDTOMapper)
         : base(view, new[] {typeof(PopulationAnalysisParameterField), typeof(PopulationAnalysisCovariateField)}, contextMenuFactory, populationAnalysisFieldFactory, eventPublisher, populationAnalysisGroupingFieldCreator, populationAnalysisTemplateTask, dialogCreator, fieldDTOMapper)
      {
         _parametersRetriever = parametersRetriever;
         EmptySelectionMessage = PKSimConstants.UI.ChooseParametersToDisplay;
      }

      public override void StartAnalysis(IPopulationDataCollector populationDataCollector, PopulationAnalysis populationAnalysis)
      {
         _allParameterCache = _parametersRetriever.ParametersFrom(populationDataCollector);
         base.StartAnalysis(populationDataCollector, populationAnalysis);
      }

      public void AddParameter(IParameter parameter)
      {
         AddField(_populationAnalysisFieldFactory.CreateFor(parameter));
      }

      public void AddCovariate(string covariate)
      {
         AddField(_populationAnalysisFieldFactory.CreateFor(covariate, PopulationDataCollector));
      }

      public override void FieldSelected(IPopulationAnalysisField populationAnalysisField)
      {
         base.FieldSelected(populationAnalysisField);
         switch (populationAnalysisField)
         {
            case PopulationAnalysisParameterField parameterField:
               ParameterFieldSelected(this, new ParameterFieldSelectedEventArgs(parameterFromField(parameterField), parameterField));
               return;
            case PopulationAnalysisCovariateField covariateField:
               CovariateFieldSelected(this, new CovariateFieldSelectedEventArgs(covariateField.Covariate, covariateField));
               return;
         }
      }

      public IParameter SelectedParameter => parameterFromField(SelectedField<PopulationAnalysisParameterField>());

      private IParameter parameterFromField(PopulationAnalysisParameterField field)
      {
         if (field == null)
            return null;

         return _allParameterCache[field.ParameterPath];
      }
   }
}