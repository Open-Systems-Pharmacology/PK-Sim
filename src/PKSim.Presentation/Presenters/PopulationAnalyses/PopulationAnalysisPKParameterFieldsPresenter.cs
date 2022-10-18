using System;
using PKSim.Assets;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Events;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;

using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IPopulationAnalysisPKParameterFieldsPresenter : IPopulationAnalysisFieldsPresenter
   {
      /// <summary>
      ///    Adds the given <paramref name="quantityPKParameter" /> as pk parameter in the analysis
      /// </summary>
      void AddPKParameter(QuantityPKParameter quantityPKParameter, string quantityDisplayPath);

      /// <summary>
      ///    Returns the pk parameter currently selected
      /// </summary>
      QuantityPKParameter SelectedPKParameter { get; }

      /// <summary>
      ///    Event is thrown whenever a PK-Parameter is being selected
      /// </summary>
      event EventHandler<PKParameterFieldSelectedEventArgs> PKParameterSelected;
   }

   public class PopulationAnalysisPKParameterFieldsPresenter : PopulationAnalysisFieldsPresenter, IPopulationAnalysisPKParameterFieldsPresenter
   {
      private readonly IEntitiesInContainerRetriever _entitiesInContainerRetriever;
      private PathCache<IQuantity> _allQuantities;
      public event EventHandler<PKParameterFieldSelectedEventArgs> PKParameterSelected = delegate { };

      public PopulationAnalysisPKParameterFieldsPresenter(IPopulationAnalysisFieldsView view, IPopulationAnalysesContextMenuFactory contextMenuFactory, IPopulationAnalysisFieldFactory populationAnalysisFieldFactory, IEventPublisher eventPublisher, IPopulationAnalysisGroupingFieldCreator populationAnalysisGroupingFieldCreator, IPopulationAnalysisTemplateTask populationAnalysisTemplateTask, IEntitiesInContainerRetriever entitiesInContainerRetriever, IDialogCreator dialogCreator, IPopulationAnalysisFieldToPopulationAnalysisFieldDTOMapper fieldDTOMapper)
         : base(view, new[] {typeof (PopulationAnalysisPKParameterField)}, contextMenuFactory, populationAnalysisFieldFactory, eventPublisher, populationAnalysisGroupingFieldCreator, populationAnalysisTemplateTask, dialogCreator,fieldDTOMapper)
      {
         _entitiesInContainerRetriever = entitiesInContainerRetriever;
         EmptySelectionMessage = PKSimConstants.UI.ChoosePKParametersToDisplay;
      }

      public override void StartAnalysis(IPopulationDataCollector populationDataCollector, PopulationAnalysis populationAnalysis)
      {
         _allQuantities = _entitiesInContainerRetriever.QuantitiesFrom(populationDataCollector);
         base.StartAnalysis(populationDataCollector, populationAnalysis);
      }

      public void AddPKParameter(QuantityPKParameter quantityPKParameter, string defaultName)
      {
         AddField(createFieldFrom(quantityPKParameter, defaultName));
      }

      private PopulationAnalysisPKParameterField createFieldFrom(QuantityPKParameter pkParameter, string quantityDisplayPath)
      {
         var quantity = _allQuantities[pkParameter.QuantityPath];

         // if the quantity cannot be found then we are assuming this is a global type that should be created anyway.
         if (quantity == null)
         {
            return _populationAnalysisFieldFactory.CreateFor(pkParameter, QuantityType.Drug | QuantityType.Observer, quantityDisplayPath);
         }

         return _populationAnalysisFieldFactory.CreateFor(pkParameter, quantity.QuantityType, quantityDisplayPath);
      }

      public QuantityPKParameter SelectedPKParameter => pkParameterFrom(SelectedField<PopulationAnalysisPKParameterField>());

      private QuantityPKParameter pkParameterFrom(PopulationAnalysisPKParameterField populationAnalysisPKParameterField)
      {
         if (populationAnalysisPKParameterField == null)
            return null;

         return PopulationDataCollector.PKParameterFor(populationAnalysisPKParameterField.QuantityPath, populationAnalysisPKParameterField.PKParameter);
      }

      public override void FieldSelected(IPopulationAnalysisField populationAnalysisField)
      {
         base.FieldSelected(populationAnalysisField);

         var pkParameterField = populationAnalysisField as PopulationAnalysisPKParameterField;
         if (pkParameterField == null)
            return;

         var pkParameter = pkParameterFrom(pkParameterField);
         if (pkParameter == null)
            return;

         PKParameterSelected(this, new PKParameterFieldSelectedEventArgs(pkParameter, pkParameterField));
      }
   }
}