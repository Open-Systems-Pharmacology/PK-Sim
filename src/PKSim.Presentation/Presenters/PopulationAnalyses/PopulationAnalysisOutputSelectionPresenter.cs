using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Extensions;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IPopulationAnalysisOutputSelectionPresenter : IPopulationAnalysisItemPresenter
   {
      void AddOutput();
      void RemoveOutput();
      IEnumerable<Unit> AllTimeUnits { get; }
   }

   public class PopulationAnalysisOutputSelectionPresenter : AbstractSubPresenter<IPopulationAnalysisOutputSelectionView, IPopulationAnalysisOutputSelectionPresenter>, IPopulationAnalysisOutputSelectionPresenter
   {
      private readonly IQuantityListPresenter _allOutputsPresenter;
      private readonly IPopulationAnalysisOutputFieldsPresenter _selectedOutputsPresenter;
      private readonly IPopulationAnalysisStatisticsSelectionPresenter _statisticsSelectionPresenter;
      private readonly IEntitiesInContainerRetriever _outputsRetriever;
      private readonly IEventPublisher _eventPublisher;
      private PopulationStatisticalAnalysis _populationAnalysis;
      private readonly IDimension _timeDimension;

      public PopulationAnalysisOutputSelectionPresenter(IPopulationAnalysisOutputSelectionView view, IQuantityListPresenter allOutputsPresenter,
         IPopulationAnalysisOutputFieldsPresenter selectedOutputsPresenter, IPopulationAnalysisStatisticsSelectionPresenter statisticsSelectionPresenter, IEntitiesInContainerRetriever outputsRetriever,
         IEventPublisher eventPublisher, IDimensionRepository dimensionRepository) : base(view)
      {
         _allOutputsPresenter = allOutputsPresenter;
         _selectedOutputsPresenter = selectedOutputsPresenter;
         _statisticsSelectionPresenter = statisticsSelectionPresenter;
         _outputsRetriever = outputsRetriever;
         _eventPublisher = eventPublisher;
         _timeDimension = dimensionRepository.Time;
         AddSubPresenters(allOutputsPresenter, _selectedOutputsPresenter, _statisticsSelectionPresenter);
         _view.AddPopulationOutputsView(_allOutputsPresenter.View);
         _view.AddSelectedOutputsView(_selectedOutputsPresenter.View);
         _view.AddStatisticsSelectionView(_statisticsSelectionPresenter.View);
         _allOutputsPresenter.QuantityDoubleClicked += (o, e) => addOutput(e.Quantity);
         _allOutputsPresenter.Hide(QuantityColumn.Selection);
         _statisticsSelectionPresenter.SelectionChanged += (o, e) => selectionChanged();
         _allOutputsPresenter.ExpandAllGroups = true;
      }

      private void selectionChanged()
      {
         _eventPublisher.PublishEvent(new PopulationAnalysisChartSettingsChangedEvent(_populationAnalysis));
      }

      public void StartAnalysis(IPopulationDataCollector populationDataCollector, PopulationAnalysis populationAnalysis)
      {
         _populationAnalysis = populationAnalysis.DowncastTo<PopulationStatisticalAnalysis>();
         _allOutputsPresenter.UpdateColumnSettings(populationDataCollector);
         var allOutputs = _outputsRetriever.OutputsFrom(populationDataCollector);
         _allOutputsPresenter.Edit(allOutputs);
         _selectedOutputsPresenter.StartAnalysis(populationDataCollector, populationAnalysis);
         _statisticsSelectionPresenter.StartAnalysis(populationDataCollector, populationAnalysis);
         addDefaultSelectionIfRequired(allOutputs);
         _view.BindTo(_populationAnalysis);
      }

      private void addDefaultSelectionIfRequired(PathCache<IQuantity> allOutputs)
      {
         //not one exactly
         if (allOutputs.Count != 1)
            return;

         //one already selected
         if (_populationAnalysis.All<PopulationAnalysisOutputField>().Any())
            return;

         addOutput(_allOutputsPresenter.QuantityDTOByPath(allOutputs.Keys.First()));
      }

      public void AddOutput()
      {
         addOutput(_allOutputsPresenter.SelectedQuantitiesDTO);
      }

      private void addOutput(QuantitySelectionDTO output)
      {
         addOutput(new[] {output});
      }

      private void addOutput(IEnumerable<QuantitySelectionDTO> outputs)
      {
         outputs.Each(dto => _selectedOutputsPresenter.AddOutput(dto.Quantity, dto.DisplayPathAsString));
      }

      public void RemoveOutput()
      {
         _selectedOutputsPresenter.RemoveSelection();
      }

      public IEnumerable<Unit> AllTimeUnits
      {
         get { return _timeDimension.Units; }
      }

      public override void ViewChanged()
      {
         base.ViewChanged();
         selectionChanged();
      }
   }
}