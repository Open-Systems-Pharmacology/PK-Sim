using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Services
{
   public class PopulationAnalysisGroupingFieldCreator : IPopulationAnalysisGroupingFieldCreator
   {
      private readonly IApplicationController _applicationController;
      private readonly IPopulationAnalysisFieldFactory _populationAnalysisFieldFactory;

      public PopulationAnalysisGroupingFieldCreator(IApplicationController applicationController, IPopulationAnalysisFieldFactory populationAnalysisFieldFactory)
      {
         _applicationController = applicationController;
         _populationAnalysisFieldFactory = populationAnalysisFieldFactory;
      }

      public PopulationAnalysisGroupingField CreateGroupingFieldFor(IPopulationAnalysisField populationAnalysisField, IPopulationDataCollector populationDataCollector)
      {
         using (var presenter = _applicationController.Start<ICreatePopulationAnalysisGroupingFieldPresenter>())
         {
            var groupingDefiniton = presenter.CreateGrouping(populationAnalysisField, populationDataCollector);
            if (groupingDefiniton == null)
               return null;

            return _populationAnalysisFieldFactory.CreateGroupingField(groupingDefiniton, populationAnalysisField)
               .WithName(presenter.FieldName);
         }
      }

      public bool EditDerivedField(PopulationAnalysisDerivedField derivedField, IPopulationDataCollector populationDataCollector)
      {
         var groupingField = derivedField as PopulationAnalysisGroupingField;
         //only grouping fields defined so far
         if (groupingField == null)
            return false;

         using (var presenter = _applicationController.Start<IEditPopulationAnalysisGroupingFieldPresenter>())
         {
            return presenter.Edit(groupingField, populationDataCollector);
         }
      }
   }
}