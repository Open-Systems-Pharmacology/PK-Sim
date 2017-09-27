using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_DataRepositorySynchronizer : ContextSpecification<ISimulationResultsSynchronizer>
   {
      protected IPKAnalysesTask _populationPKAnalysesTask;
      protected ISimulationResultsCreator _simulationResultsCreator;
      protected IDisplayUnitUpdater _displayUnitUpdater;

      protected override void Context()
      {
         _populationPKAnalysesTask = A.Fake<IPKAnalysesTask>();
         _simulationResultsCreator= A.Fake<ISimulationResultsCreator>();
         _displayUnitUpdater= A.Fake<IDisplayUnitUpdater>();

         sut = new SimulationResultsSynchronizer(_populationPKAnalysesTask,_simulationResultsCreator,_displayUnitUpdater);
      }
   }

   public class When_synchronizing_the_data_for_an_individual_simulation_that_was_not_calulated_yet : concern_for_DataRepositorySynchronizer
   {
      private DataRepository _dataRepository;
      private IndividualSimulation _simulation;
      private Compound _compound;
      private BaseGrid _newBaseGrid;
      private DataColumn _newColumn;
      private DataColumn _otherColumn;
      private SimulationResults _simulationResults;
      private OutputSelections _outputSelection;

      protected override void Context()
      {
         base.Context();
         var dimension = A.Fake<IDimension>();
         _outputSelection = new OutputSelections();
         _simulation = new IndividualSimulation {SimulationSettings = new SimulationSettings{OutputSelections = _outputSelection}};
         _compound = A.Fake<Compound>();
         A.CallTo(() => _compound.Name).Returns("Drug");
         A.CallTo(() => _compound.MolWeight).Returns(20);
         _dataRepository = new DataRepository();
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("toto", PKSimBuildingBlockType.Compound) {BuildingBlock = _compound});
         _newBaseGrid = new BaseGrid("baseGrid1", "baseGrid1", dimension);
         _newColumn = new DataColumn("col1", "col1", dimension, _newBaseGrid)
         {
            QuantityInfo = new QuantityInfo("Obs", new List<string> {"Sim", "Liver", "Cell", "Drug", "Obs"}, QuantityType.Observer | QuantityType.Drug)
         };
         _otherColumn = new DataColumn("col2", "col2", dimension, _newBaseGrid)
         {
            QuantityInfo = new QuantityInfo("Obs", new List<string> { "Sim", "Liver", "Cell", "Sp1", "Obs" }, QuantityType.Observer | QuantityType.Metabolite)
         };
         _dataRepository.Add(_newBaseGrid);
         _dataRepository.Add(_newColumn);
         _dataRepository.Add(_otherColumn);
         _simulationResults= A.Fake<SimulationResults>();
         A.CallTo(() => _simulationResultsCreator.CreateResultsFrom(_dataRepository)).Returns(_simulationResults);
         _outputSelection.AddOutput(new QuantitySelection(new[] {"Liver", "Cell", "Drug", "Obs"}.ToPathString(), _newColumn.QuantityInfo.Type));
      }

      protected override void Because()
      {
         sut.Synchronize(_simulation, _dataRepository);
      }

      [Observation]
      public void should_simply_set_the_data_repository_as_results_for_the_simulation()
      {
         _simulation.DataRepository.ShouldBeEqualTo(_dataRepository);
      }

      [Observation]
      public void should_have_let_the_moleweight_of_all_other_columns_to_null()
      {
         _otherColumn.DataInfo.MolWeight.ShouldBeNull();
      }

      [Observation]
      public void should_have_created_the_simulation_results_based_on_the_data_repository()
      {
         _simulation.Results.ShouldBeEqualTo(_simulationResults);
      }

      [Observation]
      public void should_udpate_the_display_unit_used_in_the_simulation_results()
      {
         A.CallTo(() => _displayUnitUpdater.UpdateDisplayUnitsIn(_simulation.DataRepository)).MustHaveHappened();
      }

      [Observation]
      public void should_mark_output_not_selected_by_user_as_internal()
      {
         _otherColumn.IsInternal.ShouldBeTrue();
      }

      [Observation]
      public void should_mark_output_selected_by_user_as_non_internal()
      {
         _newColumn.IsInternal.ShouldBeFalse();
      }
   }

   public class When_synchronizing_the_data_for_a_simulation_that_already_has_data : concern_for_DataRepositorySynchronizer
   {
      private DataRepository _newDataRepository;
      private IndividualSimulation _simulation;
      private BaseGrid _newBaseGrid;
      private DataColumn _newColumn;
      private DataColumn _newExistingColumn;
      private DataRepository _oldDataRepository;
      private BaseGrid _oldBaseGrid;
      private DataColumn _oldColumn;
      private DataColumn _oldExistingColumn;
      private Compound _compound;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<IndividualSimulation>();
         var dimension = A.Fake<IDimension>();
         _compound = A.Fake<Compound>();
         A.CallTo(() => _compound.Name).Returns("Drug");
         //new data repository  + columns
         _newDataRepository = new DataRepository();
         _newBaseGrid = new BaseGrid("baseGrid1", "baseGrid1", dimension);
         _newBaseGrid.Values = new float[] {1, 2, 3, 4};
         _newColumn = new DataColumn("col1", "col1", dimension, _newBaseGrid);
         _newColumn.Values = new float[] {1, 1, 1, 1};
         _newColumn.QuantityInfo = new QuantityInfo("sp1", new List<string> {"Liver", "Cell", "Drug", "Obs1"}, QuantityType.Observer | QuantityType.Drug);
         _newExistingColumn = new DataColumn("col2", "col2", dimension, _newBaseGrid);
         _newExistingColumn.QuantityInfo = new QuantityInfo("sp2", new List<string> {"Liver", "Cell", "Drug", "Obs2"}, QuantityType.Observer | QuantityType.Metabolite);
         _newExistingColumn.Values = new float[] {2, 2, 2, 2};

         _newDataRepository.Add(_newBaseGrid);
         _newDataRepository.Add(_newColumn);
         _newDataRepository.Add(_newExistingColumn);


         //old data repository  + columns
         _oldDataRepository = new DataRepository();
         _oldBaseGrid = new BaseGrid("baseGrid2", "baseGrid2", dimension);
         _oldBaseGrid.Values = new float[] {1, 2, 3};
         _oldColumn = new DataColumn("col3", "col3", dimension, _oldBaseGrid);
         _oldColumn.Values = new float[] {3, 3, 3};
         _oldColumn.QuantityInfo = new QuantityInfo("sp3", new List<string> {"Liver", "Cell", "Drug", "Obs3"}, QuantityType.Observer | QuantityType.Drug);
         _oldExistingColumn = new DataColumn("col4", "col4", dimension, _oldBaseGrid);
         _oldExistingColumn.QuantityInfo = new QuantityInfo("sp2", new List<string> {"Liver", "Cell", "Drug", "Obs2"}, QuantityType.Observer | QuantityType.Drug);
         _oldExistingColumn.Values = new float[] {2, 2, 2};

         _oldDataRepository.Add(_oldBaseGrid);
         _oldDataRepository.Add(_oldColumn);
         _oldDataRepository.Add(_oldExistingColumn);

         A.CallTo(() => _simulation.HasResults).Returns(true);
         A.CallTo(() => _simulation.DataRepository).Returns(_oldDataRepository);
         A.CallTo(() => _compound.MolWeight).Returns(20);
         A.CallTo(() => _simulation.BuildingBlock<Compound>()).Returns(_compound);
      }

      protected override void Because()
      {
         sut.Synchronize(_simulation, _newDataRepository);
      }

      [Observation]
      public void should_have_update_the_values_in_the_old_columns()
      {
         _oldBaseGrid.Values.ShouldOnlyContain(_newBaseGrid.Values);
         _oldExistingColumn.Values.ShouldOnlyContain(_newExistingColumn.Values);
      }

      [Observation]
      public void should_have_added_the_new_columns_to_the_old_repository_with_the_accurate_value()
      {
         _oldDataRepository.GetColumn(_newColumn.Id).Values.ShouldOnlyContain(_newColumn.Values);
      }

      [Observation]
      public void should_have_added_the_new_columns_to_the_old_repository_with_the_accurate_base_grid()
      {
         _oldDataRepository.GetColumn(_newColumn.Id).BaseGrid.ShouldBeEqualTo(_oldBaseGrid);
      }

      [Observation]
      public void should_have_deleted_the_columns_that_are_not_used_anymore()
      {
         _oldDataRepository.Contains(_oldColumn.Id).ShouldBeFalse();
      }
   }
}