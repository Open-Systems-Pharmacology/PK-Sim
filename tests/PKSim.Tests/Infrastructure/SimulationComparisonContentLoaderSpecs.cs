using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain.Data;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.Queries;
using PKSim.Infrastructure.Services;
using Individual = PKSim.Core.Batch.Individual;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_SimulationComparisonContentLoader : ContextSpecification<SimulationComparisonContentLoader>
   {
      private ICompressedSerializationManager _compressedSerializationManager;
      private ISimulationComparisonMetaDataContentQuery _simulationComparisonMetaDataContentQuery;

      protected override void Context()
      {
         _compressedSerializationManager = A.Fake<ICompressedSerializationManager>();
         _simulationComparisonMetaDataContentQuery = A.Fake<ISimulationComparisonMetaDataContentQuery>();
         sut = new SimulationComparisonContentLoader(_simulationComparisonMetaDataContentQuery, _compressedSerializationManager);
      }
   }

   public class When_loading_content_for_simulation_comparison : concern_for_SimulationComparisonContentLoader
   {
      private IndividualSimulationComparison _simulationComparison;
      private DataRepository _dataRepository;
      private DataColumn _dataColumn;

      protected override void Context()
      {
         base.Context();
         _simulationComparison = new IndividualSimulationComparison();
         var curve = A.Fake<Curve>();
         _dataRepository = new DataRepository();
         _dataColumn = new DataColumn {Repository = _dataRepository};
         A.CallTo(() => curve.xData).Returns(_dataColumn);
         _simulationComparison.AddCurve(curve);
      }

      protected override void Because()
      {
         sut.LoadContentFor(_simulationComparison);
      }

      [Observation]
      public void should_update_the_used_observed_data()
      {
         _simulationComparison.UsesObservedData(_dataRepository).ShouldBeTrue();
      }
   }
}
