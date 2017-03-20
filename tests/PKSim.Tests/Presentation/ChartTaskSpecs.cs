using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Events;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_ChartTask : ContextSpecification<IChartTask>
   {
      protected IEventPublisher _eventPublisher;
      protected IProjectChangedNotifier _projectChangedNotifier;
      private IChartTemplatingTask _chartTemplatingTask;
      private IProjectRetriever _projectRetriever;
      protected ExportChartToPDFCommand _exportChartToPDFCommand;

      protected override void Context()
      {
         _projectChangedNotifier = A.Fake<IProjectChangedNotifier>();
         _chartTemplatingTask = A.Fake<IChartTemplatingTask>();
         _projectRetriever = A.Fake<IProjectRetriever>();
         _exportChartToPDFCommand = A.Fake<ExportChartToPDFCommand>();
         sut = new ChartTask(_projectChangedNotifier, _projectRetriever, _exportChartToPDFCommand);
      }
   }

   public class When_asked_if_a_base_grid_column_should_be_displayed : concern_for_ChartTask
   {
      private DataColumn _baseGrid;

      protected override void Context()
      {
         base.Context();
         _baseGrid = new BaseGrid("Time", DomainHelperForSpecs.NoDimension());
         _baseGrid.Values = new float[] {0, 1, 2};
      }

      [Observation]
      public void should_return_false()
      {
         sut.IsColumnVisibleInDataBrowser(_baseGrid).ShouldBeFalse();
      }
   }
}