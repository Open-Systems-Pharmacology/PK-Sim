using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Events;
using PKSim.Core;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IPopulationAnalysisOutputFieldsPresenter : IPopulationAnalysisFieldsPresenter
   {
      void AddOutput(IQuantity output, string defaultName);
   }

   public class PopulationAnalysisOutputFieldsPresenter : PopulationAnalysisFieldsPresenter, IPopulationAnalysisOutputFieldsPresenter
   {
      public PopulationAnalysisOutputFieldsPresenter(IPopulationAnalysisFieldsView view, IPopulationAnalysesContextMenuFactory contextMenuFactory, IPopulationAnalysisFieldFactory populationAnalysisFieldFactory, IEventPublisher eventPublisher, IPopulationAnalysisGroupingFieldCreator populationAnalysisGroupingFieldCreator, IPopulationAnalysisTemplateTask populationAnalysisTemplateTask, IDialogCreator dialogCreator, IPopulationAnalysisFieldToPopulationAnalysisFieldDTOMapper fieldDTOMapper)
         : base(view, new[] {typeof (PopulationAnalysisOutputField)}, contextMenuFactory, populationAnalysisFieldFactory, eventPublisher, populationAnalysisGroupingFieldCreator, populationAnalysisTemplateTask, dialogCreator, fieldDTOMapper)
      {
         view.ColorSelectionVisible = true;
         view.CreateGroupingButtonVisible = false;
         EmptySelectionMessage = PKSimConstants.UI.ChooseOutputsToDisplay;
      }

      public void AddOutput(IQuantity output, string defaultName)
      {
         var allUsedDimensions = _populationAnalysis.AllFields.OfType<PopulationAnalysisOutputField>().Select(x => x.Dimension).Distinct().ToList();

         if (outputUsesAnotherDimension(output, allUsedDimensions))
            throw new PKSimException(PKSimConstants.Error.CannotAddOutputFieldBecauseOfDimensionMismatch(defaultName, allUsedDimensions.Select(x => x.DisplayName), output.Dimension.DisplayName));

         var outputField = _populationAnalysisFieldFactory.CreateFor(output, defaultName);
         AddField(outputField);
         //select by default

         var pivotAnalysis = _populationAnalysis as PopulationPivotAnalysis;
         pivotAnalysis?.SetPosition(outputField, PivotArea.DataArea);
      }

      private static bool outputUsesAnotherDimension(IQuantity output, List<IDimension> allUsedDimensions)
      {
         return allUsedDimensions.Any() && !allUsedDimensions.Contains(output.Dimension);
      }
   }
}