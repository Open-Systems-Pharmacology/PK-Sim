using OSPSuite.Utility.Extensions;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IPopulationAnalysisObservedDataSettingsPresenter : IPresenter<IPopulationAnalysisObservedDataSettingsView>
   {
      bool ApplyGroupingToObservedData { get; set; }
      void Edit(ObservedDataCollection observedDataCollection);
   }

   public class PopulationAnalysisObservedDataSettingsPresenter : AbstractPresenter<IPopulationAnalysisObservedDataSettingsView, IPopulationAnalysisObservedDataSettingsPresenter>, IPopulationAnalysisObservedDataSettingsPresenter
   {
      private readonly IObservedDataCurveOptionsToObservedDataCurveOptionsDTOMapper _dtoMapper;
      private ObservedDataCollection _observedDataCollection;

      public PopulationAnalysisObservedDataSettingsPresenter(IPopulationAnalysisObservedDataSettingsView view, IObservedDataCurveOptionsToObservedDataCurveOptionsDTOMapper dtoMapper)
         : base(view)
      {
         _dtoMapper = dtoMapper;
      }

      public void Edit(ObservedDataCollection observedDataCollection)
      {
         _observedDataCollection = observedDataCollection;
         ApplyGroupingToObservedData = observedDataCollection.ApplyGroupingToObservedData;
         _view.BindTo(_observedDataCollection.ObservedDataCurveOptions().MapAllUsing(_dtoMapper));
      }

      public bool ApplyGroupingToObservedData
      {
         get { return _observedDataCollection.ApplyGroupingToObservedData; }
         set { _observedDataCollection.ApplyGroupingToObservedData = value; }
      }
   }
}