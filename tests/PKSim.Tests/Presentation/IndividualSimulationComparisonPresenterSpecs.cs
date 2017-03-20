using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Chart;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.Charts;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Charts;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Presentation.Presenters.Charts;
using OSPSuite.Presentation.Services;
using OSPSuite.Presentation.Services.Charts;
using IChartTemplatingTask = PKSim.Presentation.Services.IChartTemplatingTask;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;
using IObservedDataTask = PKSim.Core.Services.IObservedDataTask;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualSimulationComparisonPresenter : ContextSpecification<IIndividualSimulationComparisonPresenter>
   {
      protected IIndividualSimulationComparisonView _view;
      protected IChartEditorAndDisplayPresenter _chartPresenter;
      protected IIndividualPKAnalysisPresenter _pkAnalysisPresenter;
      protected IQuantityPathToQuantityDisplayPathMapper _quantityPathMapper;
      protected IChartTask _chartTask;
      protected IObservedDataTask _observedDataTask;
      protected ILazyLoadTask _lazyLoadTask;
      protected IChartEditorLayoutTask _chartLayoutTask;
      protected IChartTemplatingTask _chartTemplatingTask;
      protected IDataColumnToPathElementsMapper _dataColumnToPathElementsMapper;
      protected IProjectRetriever _projectRetriever;
      private IUserSettings _userSettings;
      private ChartPresenterContext _chartPresenterContext;

      protected override void Context()
      {
         _view = A.Fake<IIndividualSimulationComparisonView>();
         _chartPresenter = A.Fake<IChartEditorAndDisplayPresenter>();
         _pkAnalysisPresenter = A.Fake<IIndividualPKAnalysisPresenter>();
         _quantityPathMapper = A.Fake<IQuantityPathToQuantityDisplayPathMapper>();
         _chartTask = A.Fake<IChartTask>();
         _observedDataTask = A.Fake<IObservedDataTask>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _chartLayoutTask = A.Fake<IChartEditorLayoutTask>();
         _chartTemplatingTask= A.Fake<IChartTemplatingTask>();
         _dataColumnToPathElementsMapper= A.Fake<IDataColumnToPathElementsMapper>();
         _projectRetriever= A.Fake<IProjectRetriever>();
         _userSettings = A.Fake<IUserSettings>();
         _chartPresenterContext= A.Fake<ChartPresenterContext>();

         A.CallTo(() => _chartPresenterContext.ChartEditorAndDisplayPresenter).Returns(_chartPresenter);
         A.CallTo(() => _chartPresenterContext.QuantityDisplayPathMapper).Returns(_quantityPathMapper);
         A.CallTo(() => _chartPresenterContext.EditorLayoutTask).Returns(_chartLayoutTask);
         A.CallTo(() => _chartPresenterContext.TemplatingTask).Returns(_chartTemplatingTask);
         A.CallTo(() => _chartPresenterContext.ProjectRetriever).Returns(_projectRetriever);

         sut = new IndividualSimulationComparisonPresenter(_view,_chartPresenterContext, _pkAnalysisPresenter,
            _chartTask, _observedDataTask, _lazyLoadTask, _chartTemplatingTask, _userSettings);
      }
   }

   public class When_adding_a_simulation_to_the_summary_chart_containing_curves : concern_for_IndividualSimulationComparisonPresenter
   {
      private DragEventArgs _dropEventArgs;
      private IndividualSimulation _simulation;
      private IndividualSimulationComparison _individualSimulationComparison;

      protected override void Context()
      {
         base.Context();
         _individualSimulationComparison = new IndividualSimulationComparison();
         _individualSimulationComparison.Curves.Add(new Curve("TOTO"));
         sut.InitializeAnalysis(_individualSimulationComparison);
         _simulation= A.Fake<IndividualSimulation>();
         A.CallTo(() => _simulation.HasResults).Returns(true);
         var simulationNode = new SimulationNode(new ClassifiableSimulation{Subject = _simulation});
         _dropEventArgs = new DragEventArgs(new DataObject(new DragDropInfo(new List<SimulationNode>{simulationNode})), 0, 0, 0, DragDropEffects.All, DragDropEffects.All);
      }

      protected override void Because()
      {
         sut.DragDrop(this, _dropEventArgs);
      }

      [Observation]
      public void should_not_initiate_the_creation_from_template()
      {
         A.CallTo(() => _chartTemplatingTask.InitFromTemplate(
            A<ICurveChart>._, A<IChartEditorAndDisplayPresenter>._, A<IReadOnlyCollection<DataColumn>>._, 
            A<IReadOnlyCollection<IndividualSimulation>>._ , A<Func<DataColumn, string>>._, null)).MustNotHaveHappened();
      }

      [Observation]
      public void should_simply_update_the_new_curve_defined_in_the_simulation()
      {
         A.CallTo(() => _chartTemplatingTask.UpdateDefaultSettings(_chartPresenter.EditorPresenter, A<IReadOnlyCollection<DataColumn>>._, A<IReadOnlyCollection<IndividualSimulation>>._, false)).MustHaveHappened();
      }
   }
}