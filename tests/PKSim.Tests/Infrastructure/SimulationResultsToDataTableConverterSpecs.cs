using System.Data;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_SimulationResultsToDataTableConverter : ContextSpecification<ISimulationResultsToDataTableConverter>
   {
      private IEntitiesInContainerRetriever _quantityRetriever;
      private IDimensionRepository _dimensionRepository;
      protected IDisplayUnitRetriever _displayUnitRetriever;

      protected override void Context()
      {
         _quantityRetriever = A.Fake<IEntitiesInContainerRetriever>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _displayUnitRetriever= A.Fake<IDisplayUnitRetriever>();
         sut = new SimulationResultsToDataTableConverter(_dimensionRepository, _quantityRetriever,_displayUnitRetriever);
      }
   }

   public class When_creating_a_data_table_based_on_the_pk_analyses_of_a_population_simulation : concern_for_SimulationResultsToDataTableConverter
   {
      private DataTable _dataTable;
      private PopulationSimulation _populationSimulation;

      protected override void Context()
      {
         base.Context();
         var pKAnalyses = new PopulationSimulationPKAnalyses();
         _populationSimulation = new PopulationSimulation {PKAnalyses = pKAnalyses};
         var pkParameter = new QuantityPKParameter {QuantityPath = "Liver", Name = "P"};
         var dimension = A.Fake<IDimension>();
         var unit = A.Fake<Unit>();  
         A.CallTo(() => unit.Name).Returns("UNIT");
         A.CallTo(() => _displayUnitRetriever.PreferredUnitFor(pkParameter, null)).Returns(unit);

         pkParameter.Dimension = dimension;
         pkParameter.SetNumberOfIndividuals(2);
         pkParameter.SetValue(0, 10);
         pkParameter.SetValue(1, 11);

         A.CallTo(() => dimension.BaseUnitValueToUnitValue(unit, 10)).Returns(100.10);
         A.CallTo(() => dimension.BaseUnitValueToUnitValue(unit, 11)).Returns(110.20);

         pKAnalyses.AddPKAnalysis(pkParameter);
      }

      protected override void Because()
      {
         _dataTable = sut.PKAnalysesToDataTable(_populationSimulation).Result;
      }

      [Observation]
      public void should_return_a_table_containing_the_expected_columns_in_the_expected_order()
      {
         _dataTable.Columns.Count.ShouldBeEqualTo(5);
         _dataTable.Columns[0].ColumnName.ShouldBeEqualTo(CoreConstants.SimulationResults.IndividualId);
         _dataTable.Columns[1].ColumnName.ShouldBeEqualTo(CoreConstants.SimulationResults.QuantityPath);
         _dataTable.Columns[2].ColumnName.ShouldBeEqualTo(CoreConstants.SimulationResults.Parameter);
         _dataTable.Columns[3].ColumnName.ShouldBeEqualTo(CoreConstants.SimulationResults.Value);
         _dataTable.Columns[4].ColumnName.ShouldBeEqualTo(CoreConstants.SimulationResults.Unit);
      }

      [Observation]
      public void should_have_exported_the_value_in_the_default_unit()
      {
         _dataTable.Rows.Count.ShouldBeEqualTo(2);
         _dataTable.Rows[0][CoreConstants.SimulationResults.Value].ShouldBeEqualTo("100.1");
         _dataTable.Rows[1][CoreConstants.SimulationResults.Value].ShouldBeEqualTo("110.2");
      }

      [Observation]
      public void should_have_set_the_expcted_value_in_other_columns()
      {
         _dataTable.Rows[0][CoreConstants.SimulationResults.IndividualId].ShouldBeEqualTo(0);
         _dataTable.Rows[0][CoreConstants.SimulationResults.QuantityPath].ShouldBeEqualTo("Liver");
         _dataTable.Rows[0][CoreConstants.SimulationResults.Parameter].ShouldBeEqualTo("P");
         _dataTable.Rows[0][CoreConstants.SimulationResults.Unit].ShouldBeEqualTo("UNIT");
         _dataTable.Rows[1][CoreConstants.SimulationResults.IndividualId].ShouldBeEqualTo(1);
         _dataTable.Rows[1][CoreConstants.SimulationResults.QuantityPath].ShouldBeEqualTo("Liver");
         _dataTable.Rows[1][CoreConstants.SimulationResults.Parameter].ShouldBeEqualTo("P");
         _dataTable.Rows[1][CoreConstants.SimulationResults.Unit].ShouldBeEqualTo("UNIT");
      }
   }
}