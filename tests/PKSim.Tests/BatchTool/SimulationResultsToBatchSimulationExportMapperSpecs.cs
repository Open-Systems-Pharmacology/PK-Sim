using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Presentation.Mappers;
using PKSim.BatchTool.Mappers;
using PKSim.BatchTool.Services;
using PKSim.Core;

namespace PKSim.BatchTool
{
   public abstract class concern_for_SimulationResultsToBatchSimulationExportMapper : ContextSpecification<ISimulationResultsToBatchSimulationExportMapper>
   {
      protected IQuantityPathToQuantityDisplayPathMapper _quantityDisplayPathMapper;
      protected IObjectPathFactory _objectPathFactory;

      protected override void Context()
      {
         _quantityDisplayPathMapper = A.Fake<IQuantityPathToQuantityDisplayPathMapper>();
         _objectPathFactory = A.Fake<IObjectPathFactory>();
         sut = new SimulationResultsToBatchSimulationExportMapper(_quantityDisplayPathMapper, _objectPathFactory);
      }
   }

   public class When_mapping_simulation_results_to_batch_simulation_export : concern_for_SimulationResultsToBatchSimulationExportMapper
   {
      private DataRepository _dataRepository;
      private ISimulation _simulation;
      private BatchSimulationExport _simulationExport;
      private BaseGrid _baseGrid;
      private DataColumn _col1;
      private DataColumn _col2;
      private Parameter _parameter;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<ISimulation>().WithName("SIM");
         _parameter = new Parameter().WithName("P1").WithFormula(new ConstantFormula(10));
         _simulation.Model = new Model
         {
            Root = new Container
            {
               _parameter
            }
         };

         _baseGrid = new BaseGrid("Time", DomainHelperForSpecs.TimeDimensionForSpecs()) {Values = new[] {1f, 2f, 3f}};
         _col1 = new DataColumn("Drug1", DomainHelperForSpecs.ConcentrationDimensionForSpecs(), _baseGrid) {Values = new[] {10f, 20f, 30f}};
         _col2 = new DataColumn("Drug2", DomainHelperForSpecs.ConcentrationDimensionForSpecs(), _baseGrid) {Values = new[] {100f, 200f, 300f}};

         _dataRepository = new DataRepository {_col1, _col2};

         A.CallTo(() => _quantityDisplayPathMapper.DisplayPathAsStringFor(_simulation, _col1, false)).Returns("PATH1");
         A.CallTo(() => _quantityDisplayPathMapper.DisplayPathAsStringFor(_simulation, _col2, false)).Returns("PATH2");
         A.CallTo(() => _objectPathFactory.CreateAbsoluteObjectPath(_parameter)).Returns(new ObjectPath("Sim", "P1"));
      }

      protected override void Because()
      {
         _simulationExport = sut.MapFrom(_simulation, _dataRepository);
      }

      [Observation]
      public void should_return_an_object_having_the_expected_simulation_properties()
      {
         _simulationExport.Name.ShouldBeEqualTo(_simulation.Name);
      }

      [Observation]
      public void should_have_created_one_output_value_for_each_output_results()
      {
         _simulationExport.OutputValues.Count.ShouldBeEqualTo(2);
         verifyOutputExport(_simulationExport.OutputValues[0], _col1, "PATH1", 1.5);
         verifyOutputExport(_simulationExport.OutputValues[1], _col2, "PATH2", 1.5);
      }

      private void verifyOutputExport(BatchOutputValues outputValues, DataColumn column, string path, double threshold)
      {
         outputValues.Path.ShouldBeEqualTo(path);
         outputValues.Values.ShouldBeEqualTo(column.ConvertToDisplayValues(column.Values));
         outputValues.Threshold.ShouldBeEqualTo(threshold);

      }

      [Observation]
      public void should_have_set_the_time_values_using_display_units()
      {
         _simulationExport.Time.ShouldBeEqualTo(_baseGrid.ConvertToDisplayValues(_baseGrid.Values));
      }

      [Observation]
      public void should_have_exported_parameter_values()
      {
         _simulationExport.ParameterValues.Count.ShouldBeEqualTo(1);
         _simulationExport.ParameterValues[0].Path.ShouldBeEqualTo(_objectPathFactory.CreateAbsoluteObjectPath(_parameter).PathAsString);
         _simulationExport.ParameterValues[0].Value.ShouldBeEqualTo(_parameter.Value);
      }
   }
}