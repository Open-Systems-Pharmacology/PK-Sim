using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Events;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;
using PKSim.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_ChartTask : ContextSpecification<IChartTask>
   {
      protected IEventPublisher _eventPublisher;
      private IProjectRetriever _projectRetriever;

      protected override void Context()
      {
         _projectRetriever = A.Fake<IProjectRetriever>();
         sut = new ChartTask(_projectRetriever);
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