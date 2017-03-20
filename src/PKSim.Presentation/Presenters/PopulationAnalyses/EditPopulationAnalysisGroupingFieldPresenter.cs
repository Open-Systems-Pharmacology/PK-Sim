using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IEditPopulationAnalysisGroupingFieldPresenter : IDisposablePresenter
   {
      bool Edit(PopulationAnalysisGroupingField groupingField, IPopulationDataCollector populationDataCollector);
   }

   public class EditPopulationAnalysisGroupingFieldPresenter : AbstractDisposablePresenter<IEditPopulationAnalysisGroupingFieldView, IEditPopulationAnalysisGroupingFieldPresenter>, IEditPopulationAnalysisGroupingFieldPresenter
   {
      private readonly IGroupingDefinitionToGroupingDefinitionPresenterMapper _groupingDefinitionPresenterMapper;
      private IGroupingDefinitionPresenter _groupingPresenter;

      public EditPopulationAnalysisGroupingFieldPresenter(IEditPopulationAnalysisGroupingFieldView view, IGroupingDefinitionToGroupingDefinitionPresenterMapper groupingDefinitionPresenterMapper)
         : base(view)
      {
         _groupingDefinitionPresenterMapper = groupingDefinitionPresenterMapper;
      }

      public bool Edit(PopulationAnalysisGroupingField groupingField, IPopulationDataCollector populationDataCollector)
      {
         _groupingPresenter = _groupingDefinitionPresenterMapper.MapFrom(groupingField.GroupingDefinition);
         _groupingPresenter.StatusChanged += (o, e) => ViewChanged();

         var referenceField = groupingField.PopulationAnalysis.FieldByName(groupingField.ReferencedFieldName);
         _groupingPresenter.InitializeWith(referenceField, populationDataCollector);
         _groupingPresenter.Edit(groupingField.GroupingDefinition);

         View.SetGroupingView(_groupingPresenter.BaseView);
         View.Caption = PKSimConstants.UI.EditGroupingFor(groupingField.Name, referenceField.Name);
         View.Display();
         if (_view.Canceled)
            return false;

         _groupingPresenter.UpdateGroupingDefinition();
         return true;
      }

      public override void ViewChanged()
      {
         base.ViewChanged();
         View.OkEnabled = CanClose;
      }

      public override bool CanClose
      {
         get
         {
            var canClose = base.CanClose;
            if (_groupingPresenter != null)
               canClose = canClose && _groupingPresenter.CanClose;

            return canClose;
         }
      }
   }
}