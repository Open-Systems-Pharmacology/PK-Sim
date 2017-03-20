using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Chart.ParameterIdentifications;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;

namespace PKSim.IntegrationTests
{

   public class When_serializing_a_parameter_identification_residual_vs_time_chart : ContextForSerialization<ParameterIdentificationResidualVsTimeChart>
   {
      private ParameterIdentificationResidualVsTimeChart _residualVsTimeChart;
      private ParameterIdentificationResidualVsTimeChart _deserializedChart;
      private DataRepository _dataRepository;
      private IDimensionFactory _dimensionFactory;

      public override void GlobalContext()
      {
         base.GlobalContext();

         _dataRepository = DomainHelperForSpecs.IndividualSimulationDataRepositoryFor("S");
         _dimensionFactory = IoC.Resolve<IDimensionFactory>();
         _residualVsTimeChart = new ParameterIdentificationResidualVsTimeChart();
         _residualVsTimeChart.AddRepository(_dataRepository);

         var curve  = new Curve();
         curve.SetxData(_dataRepository.BaseGrid, _dimensionFactory);
         curve.SetyData(_dataRepository.FirstDataColumn(), _dimensionFactory);

         _residualVsTimeChart.AddCurve(curve);
         _deserializedChart = SerializeAndDeserialize(_residualVsTimeChart);
      }

      [Observation]
      public void should_have_been_able_to_deserialized_the_curves_referencing_chart_own_repositories()
      {
         _deserializedChart.Curves.Count.ShouldBeEqualTo(1);
      }
   
   }
}