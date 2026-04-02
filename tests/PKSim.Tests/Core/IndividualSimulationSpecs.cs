using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using ModelConfiguration = PKSim.Core.Model.ModelConfiguration;

namespace PKSim.Core
{
   public abstract class concern_for_IndividualSimulation : ContextSpecification<IndividualSimulation>
   {
      protected ICloneManager _cloneManager;

      protected override void Context()
      {
         _cloneManager = A.Fake<ICloneManager>();
         sut = new IndividualSimulation();
      }
   }

   public class When_renaming_a_simulation : concern_for_IndividualSimulation
   {
      private string _newName;
      private ReactionBuildingBlock _reactionBuildingBlock;

      protected override void Context()
      {
         base.Context();
         _reactionBuildingBlock = new ReactionBuildingBlock();
         sut.AddReactions(_reactionBuildingBlock);
         sut.Model = new OSPSuite.Core.Domain.Model {Root = new Container()};
         sut.Settings = new SimulationSettings();
         sut.Name = "oldName";
      }

      protected override void Because()
      {
         _newName = "newName";
         sut.Name = _newName;
      }

      [Observation]
      public void the_building_blocks_and_model_should_be_renamed()
      {
         _reactionBuildingBlock.Name.ShouldBeEqualTo(_newName);
         sut.Settings.Name.ShouldBeEqualTo(_newName);
         sut.Model.Name.ShouldBeEqualTo(_newName);
         sut.Model.Root.Name.ShouldBeEqualTo(_newName);
      }
   }

   public class When_a_newly_created_simulation_is_asked_if_it_has_results : concern_for_IndividualSimulation
   {
      [Observation]
      public void should_return_false()
      {
         sut.HasResults.ShouldBeFalse();
      }
   }

   public class When_setting_a_null_data_repository_into_a_simulation : concern_for_IndividualSimulation
   {
      protected override void Because()
      {
         sut.Results = null;
      }

      [Observation]
      public void the_simulation_should_have_no_result()
      {
         sut.HasResults.ShouldBeFalse();
      }
   }

   public class When_setting_the_results_in_the_simulation : concern_for_IndividualSimulation
   {
      protected override void Because()
      {
         sut.DataRepository = new DataRepository {new DataColumn()};
      }

      [Observation]
      public void the_simulation_should_say_that_the_results_are_updated()
      {
         sut.HasUpToDateResults.ShouldBeTrue();
      }
   }

   public class When_the_simulation_version_has_changed : concern_for_IndividualSimulation
   {
      protected override void Because()
      {
         sut.DataRepository = new DataRepository {new DataColumn()};
         sut.Version++;
      }

      [Observation]
      public void the_results_should_be_outdated()
      {
         sut.HasUpToDateResults.ShouldBeFalse();
      }
   }

   public class When_the_simulation_version_has_changed_and_we_set_some_results : concern_for_IndividualSimulation
   {
      protected override void Because()
      {
         sut.DataRepository = new DataRepository {new DataColumn()};
         sut.Version++;
         sut.DataRepository = new DataRepository {new DataColumn()};
      }

      [Observation]
      public void the_results_should_be_up_to_date()
      {
         sut.HasUpToDateResults.ShouldBeTrue();
      }
   }

   public class When_updating_the_properties_of_a_simulation_from_a_source_simulation : concern_for_IndividualSimulation
   {
      private IndividualSimulation _sourceSimulation;
      private Compound _sourceCompound1, _sourceCompound2;
      private Compound _cloneCompound1, _cloneCompound2;
      private CompoundProperties _sourceCompoundProperties1;
      private Protocol _sourceProtocol;
      private Protocol _cloneProtocol;
      private CompoundProperties _sourceCompoundProperties2;
      private IDiagramModel _cloneDiagramModel;

      protected override void Context()
      {
         base.Context();
         _sourceSimulation = new IndividualSimulation();
         _cloneDiagramModel = A.Fake<IDiagramModel>();
         _sourceSimulation.ReactionDiagramModel = A.Fake<IDiagramModel>();
         A.CallTo(() => _sourceSimulation.ReactionDiagramModel.CreateCopy(null)).Returns(_cloneDiagramModel);
         _sourceSimulation.DataRepository = A.Fake<DataRepository>();
         _sourceSimulation.Properties = new SimulationProperties();
         _sourceCompound1 = new Compound().WithId("SourceComp1");
         _sourceCompound2 = new Compound().WithId("SourceComp2");
         _cloneCompound1 = new Compound().WithId("CloneComp1");
         _cloneCompound2 = new Compound().WithId("CloneComp2");
         _sourceProtocol = new SimpleProtocol().WithId("SourceProtocol");
         _cloneProtocol = new SimpleProtocol().WithId("CloneProtocol");
         _sourceCompoundProperties1 = new CompoundProperties {Compound = _sourceCompound1};
         _sourceCompoundProperties2 = new CompoundProperties {Compound = _sourceCompound2};


         _sourceSimulation.Properties.AddCompoundProperties(_sourceCompoundProperties1);
         _sourceSimulation.Properties.AddCompoundProperties(_sourceCompoundProperties2);
         _sourceSimulation.Model = A.Fake<IModel>();

         _sourceSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("CompoundTemplate1", PKSimBuildingBlockType.Compound) {BuildingBlock = _sourceCompound1});
         _sourceSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("CompoundTemplate2", PKSimBuildingBlockType.Compound) {BuildingBlock = _sourceCompound2});
         _sourceSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("ProtocolTemplate", PKSimBuildingBlockType.Protocol) {BuildingBlock = _sourceProtocol});

         A.CallTo(() => _cloneManager.Clone((IPKSimBuildingBlock) _sourceCompound1)).Returns(_cloneCompound1);
         A.CallTo(() => _cloneManager.Clone((IPKSimBuildingBlock) _sourceCompound2)).Returns(_cloneCompound2);
         A.CallTo(() => _cloneManager.Clone((IPKSimBuildingBlock) _sourceProtocol)).Returns(_cloneProtocol);

         _sourceCompoundProperties1.ProtocolProperties.Protocol = _sourceProtocol;
      }

      protected override void Because()
      {
         sut.UpdatePropertiesFrom(_sourceSimulation, _cloneManager);
      }

      [Observation]
      public void should_have_clones_the_used_building_blocks()
      {
         sut.AllBuildingBlocks<Protocol>().ShouldOnlyContain(_cloneProtocol);
         sut.AllBuildingBlocks<Compound>().ShouldOnlyContain(_cloneCompound1, _cloneCompound2);
      }

      [Observation]
      public void should_have_cloned_the_diagram_model()
      {
         sut.ReactionDiagramModel.ShouldBeEqualTo(_cloneDiagramModel);
      }

      [Observation]
      public void should_have_clone_the_model_from_the_source_simulation()
      {
         A.CallTo(() => _cloneManager.Clone(_sourceSimulation.Model)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_references_to_the_compound_in_the_compound_properties()
      {
         sut.CompoundPropertiesList.Count.ShouldBeEqualTo(2);
         sut.CompoundPropertiesFor(_cloneCompound1).ShouldNotBeNull();
         sut.CompoundPropertiesFor(_cloneCompound2).ShouldNotBeNull();
      }

      [Observation]
      public void should_update_the_references_to_the_protocol_in_the_compound_properties()
      {
         sut.CompoundPropertiesFor(_cloneCompound1).ProtocolProperties.Protocol.ShouldBeEqualTo(_cloneProtocol);
         sut.CompoundPropertiesFor(_cloneCompound2).ProtocolProperties.Protocol.ShouldBeNull();
      }
   }

   public class When_retrieving_a_building_block_of_a_given_type_for_a_simulation : concern_for_IndividualSimulation
   {
      private UsedBuildingBlock _bbId1;
      private UsedBuildingBlock _bbId2;
      private Compound _compound;
      private Individual _individual;

      protected override void Context()
      {
         base.Context();
         _compound = A.Fake<Compound>();
         _individual = A.Fake<Individual>();
         _bbId1 = new UsedBuildingBlock("individual", PKSimBuildingBlockType.Individual) {BuildingBlock = _individual};
         _bbId2 = new UsedBuildingBlock("compound", PKSimBuildingBlockType.Compound) {BuildingBlock = _compound};
         sut.AddUsedBuildingBlock(_bbId1);
         sut.AddUsedBuildingBlock(_bbId2);
      }

      [Observation]
      public void should_return_the_building_block_registered_for_the_type_in_the_simulation()
      {
         sut.BuildingBlock<Individual>().ShouldBeEqualTo(_individual);
         sut.Individual.ShouldBeEqualTo(_individual);
         sut.BuildingBlock<Compound>().ShouldBeEqualTo(_compound);
      }

      [Observation]
      public void should_return_null_if_the_id_was_not_registered_for_another_type()
      {
         sut.BuildingBlock<Protocol>().ShouldBeNull();
      }
   }

   public class When_removing_a_building_block_of_a_given_type_for_a_simulation : concern_for_IndividualSimulation
   {
      private UsedBuildingBlock _bbId1;
      private UsedBuildingBlock _bbId2;
      private Compound _compound;
      private Individual _individual;

      protected override void Context()
      {
         base.Context();
         _compound = A.Fake<Compound>();
         _individual = A.Fake<Individual>();
         _bbId1 = new UsedBuildingBlock("individual", PKSimBuildingBlockType.Individual) {BuildingBlock = _individual};
         _bbId2 = new UsedBuildingBlock("compound", PKSimBuildingBlockType.Compound) {BuildingBlock = _compound};
         sut.AddUsedBuildingBlock(_bbId1);
         sut.AddUsedBuildingBlock(_bbId2);
      }

      [Observation]
      public void should_remove_the_compound_when_removing_multiple_building_blocks()
      {
         sut.RemoveAllBuildingBlockOfType(PKSimBuildingBlockType.Compound);
         sut.AllBuildingBlocks<Compound>().ShouldBeEmpty();
      }

      [Observation]
      public void should_remove_the_individual_when_removing_a_subject_building_block()
      {
         sut.AllBuildingBlocks<Individual>().ShouldNotBeEmpty();
         sut.RemoveAllBuildingBlockOfType(PKSimBuildingBlockType.SimulationSubject);
         sut.AllBuildingBlocks<Individual>().ShouldBeEmpty();
      }

      [Observation]
      public void should_remove_the_individual_when_removing_an_individual()
      {
         sut.AllBuildingBlocks<Individual>().ShouldNotBeEmpty();
         sut.RemoveAllBuildingBlockOfType(PKSimBuildingBlockType.Individual);
         sut.AllBuildingBlocks<Individual>().ShouldBeEmpty();
      }
   }

   public class When_removing_a_used_building_block_based_on_a_simulation_building_block : concern_for_IndividualSimulation
   {
      private UsedBuildingBlock _bbId1;
      private UsedBuildingBlock _bbId2;
      private Compound _compound;
      private Individual _individual;

      protected override void Context()
      {
         base.Context();
         _compound = new Compound().WithId("Comp");
         _individual = new Individual().WithId("Ind");
         _bbId1 = new UsedBuildingBlock("individual", PKSimBuildingBlockType.Individual) {BuildingBlock = _individual};
         _bbId2 = new UsedBuildingBlock("compound", PKSimBuildingBlockType.Compound) {BuildingBlock = _compound};
         sut.AddUsedBuildingBlock(_bbId1);
         sut.AddUsedBuildingBlock(_bbId2);
      }

      [Observation]
      public void should_remove_the_building_block_if_it_belongs_to_the_simulation()
      {
         sut.RemoveUsedBuildingBlock(_compound);
         sut.AllBuildingBlocks<Compound>().ShouldBeEmpty();
      }

      [Observation]
      public void should_not_do_anything_otherwise()
      {
         sut.RemoveUsedBuildingBlock(new Compound());
         sut.AllBuildingBlocks<Compound>().ShouldNotBeEmpty();
      }
   }

   public class When_retrieving_the_compound_name_for_a_simulation_with_a_compound : concern_for_IndividualSimulation
   {
      private Compound _compound;

      protected override void Context()
      {
         base.Context();
         _compound = new Compound().WithName("TOTO");
         sut.AddUsedBuildingBlock(new UsedBuildingBlock("compound", PKSimBuildingBlockType.Compound) {BuildingBlock = _compound});
      }

      [Observation]
      public void should_return_the_compound_name()
      {
         sut.CompoundNames.ShouldOnlyContain(_compound.Name);
      }
   }

   public class When_retrieving_the_compound_properties_for_a_compound_by_name : concern_for_IndividualSimulation
   {
      private Compound _compound;
      private CompoundProperties _compoundProperties;

      protected override void Context()
      {
         base.Context();
         _compound = new Compound().WithName("TOTO");
         sut.Properties = new SimulationProperties();
         sut.AddUsedBuildingBlock(new UsedBuildingBlock("compound", PKSimBuildingBlockType.Compound) {BuildingBlock = _compound});
         _compoundProperties = new CompoundProperties {Compound = _compound};
         sut.Properties.AddCompoundProperties(_compoundProperties);
      }

      [Observation]
      public void should_return_the_compound_name()
      {
         sut.CompoundPropertiesFor(_compound.Name).ShouldBeEqualTo(_compoundProperties);
      }
   }

   public class When_retrieving_the_compound_name_for_a_simulation_without_a_compound : concern_for_IndividualSimulation
   {
      [Observation]
      public void should_return_an_empty_string()
      {
         sut.CompoundNames.ShouldBeEmpty();
      }
   }

   public class When_adding_a_used_building_block_that_already_exist_but_was_not_loaded : concern_for_IndividualSimulation
   {
      private UsedBuildingBlock _bbIndTemplate;
      private UsedBuildingBlock _bbId1;

      protected override void Context()
      {
         base.Context();
         _bbIndTemplate = new UsedBuildingBlock("individual", PKSimBuildingBlockType.Individual);
         _bbIndTemplate.Version = 10;
         _bbIndTemplate.StructureVersion = 15;
         _bbId1 = new UsedBuildingBlock("individual", PKSimBuildingBlockType.Individual) {BuildingBlock = A.Fake<Individual>()};
      }

      protected override void Because()
      {
         sut.AddUsedBuildingBlock(_bbIndTemplate);
         sut.AddUsedBuildingBlock(_bbId1);
      }

      [Observation]
      public void should_keep_the_version_number_intact()
      {
         _bbId1.Version.ShouldBeEqualTo(_bbIndTemplate.Version);
         _bbId1.StructureVersion.ShouldBeEqualTo(_bbIndTemplate.StructureVersion);
      }
   }

   public class When_updating_a_simulation_form_an_original_simulation : concern_for_IndividualSimulation
   {
      private Simulation _originalSimulation;
      private UsedObservedData _usedObservedData;

      protected override void Context()
      {
         base.Context();
         _usedObservedData = new UsedObservedData {Id = "id"};
         _originalSimulation = new IndividualSimulation();
         _originalSimulation.Name = "sim";
         _originalSimulation.Description = "desc";
         _originalSimulation.Properties = new SimulationProperties();
         _originalSimulation.AddUsedObservedData(_usedObservedData);
      }

      protected override void Because()
      {
         sut.UpdateFromOriginalSimulation(_originalSimulation);
      }

      [Observation]
      public void should_update_the_name_of_the_simulation_to_the_name_of_the_original_simulation()
      {
         sut.Name.ShouldBeEqualTo(_originalSimulation.Name);
      }

      [Observation]
      public void should_update_the_description_of_the_simulation_to_the_description_of_the_original_simulation()
      {
         sut.Description.ShouldBeEqualTo(_originalSimulation.Description);
      }

      [Observation]
      public void should_not_set_the_properties_of_the_simulation_to_the_properties_of_the_original_simulation()
      {
         sut.Properties.ShouldNotBeEqualTo(_originalSimulation.Properties);
      }

      [Observation]
      public void should_set_the_outputs_of_the_simulation_to_the_output_of_the_original_simulation()
      {
         sut.Settings.ShouldBeEqualTo(_originalSimulation.Settings);
      }

      [Observation]
      public void should_have_added_the_used_observed_data_from_the_original_simulation_in_the_updated_simulation()
      {
         sut.UsedObservedData.ShouldOnlyContain(_usedObservedData);
      }
   }

   public class When_checking_if_a_simulation_was_imported : concern_for_IndividualSimulation
   {
      private Simulation _sim;

      protected override void Context()
      {
         base.Context();
         _sim = new IndividualSimulation();
         _sim.Properties = new SimulationProperties();
      }

      [Observation]
      public void should_return_true_if_the_simulation_as_a_defined_model_properties_and_model_configuration()
      {
         _sim.ModelProperties = new ModelProperties();
         _sim.ModelConfiguration = new ModelConfiguration();
         _sim.IsImported.ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_otherwise()
      {
         _sim.IsImported.ShouldBeTrue();
      }
   }

   public class When_adding_used_observed_data_in_the_simulation : concern_for_IndividualSimulation
   {
      private DataRepository _observedData;
      private SimulationTimeProfileChart _chartWithObservedData;
      private TimeProfileAnalysisChart _timeProfileAnalysisChart;

      protected override void Context()
      {
         base.Context();
         _observedData = new DataRepository("id");
         _chartWithObservedData = new SimulationTimeProfileChart();
         _timeProfileAnalysisChart = new TimeProfileAnalysisChart();

         sut.AddAnalysis(_chartWithObservedData);
         sut.AddAnalysis(_timeProfileAnalysisChart);
      }

      protected override void Because()
      {
         sut.AddUsedObservedData(_observedData);
      }

      [Observation]
      public void should_marked_the_observed_data_as_used()
      {
         sut.UsesObservedData(_observedData).ShouldBeTrue();
      }

      [Observation]
      public void should_add_the_observed_data_to_the_chart_with_observed_data_only()
      {
         _timeProfileAnalysisChart.UsesObservedData(_observedData).ShouldBeFalse();
         _chartWithObservedData.UsesObservedData(_observedData).ShouldBeTrue();
      }
   }

   public class When_adding_some_observed_data_in_the_simulation_that_were_already_marked_as_used : concern_for_IndividualSimulation
   {
      private DataRepository _observedData;

      protected override void Context()
      {
         base.Context();
         _observedData = new DataRepository("id");
         sut.AddUsedObservedData(_observedData);
      }

      protected override void Because()
      {
         sut.AddUsedObservedData(_observedData);
      }

      [Observation]
      public void should_marked_the_observed_data_as_used()
      {
         sut.UsesObservedData(_observedData).ShouldBeTrue();
      }
   }

   public class When_removing_observed_data_in_the_simulation_that_were_unused : concern_for_IndividualSimulation
   {
      private DataRepository _observedData;

      protected override void Context()
      {
         base.Context();
         _observedData = new DataRepository("id");
      }

      [Observation]
      public void should_not_crash()
      {
         sut.RemoveUsedObservedData(_observedData);
      }

      [Observation]
      public void should_not_use_the_observed_data()
      {
         sut.UsesObservedData(_observedData).ShouldBeFalse();
      }
   }

   public class When_removing_observed_data_in_the_simulation_that_were_already_used : concern_for_IndividualSimulation
   {
      private DataRepository _observedData;
      private SimulationTimeProfileChart _chart;

      protected override void Context()
      {
         base.Context();
         _observedData = new DataRepository("id");
         _chart = new SimulationTimeProfileChart();
         sut.AddUsedObservedData(_observedData);
         sut.AddAnalysis(_chart);
      }

      protected override void Because()
      {
         sut.RemoveUsedObservedData(_observedData);
      }

      [Observation]
      public void should_not_use_the_observed_data()
      {
         sut.UsesObservedData(_observedData).ShouldBeFalse();
      }

      [Observation]
      public void should_have_removed_the_observed_data_from_the_chart()
      {
         _chart.UsesObservedData(_observedData).ShouldBeFalse();
      }
   }

   public class When_adding_some_observed_data_to_a_simulation : concern_for_IndividualSimulation
   {
      protected override void Context()
      {
         base.Context();
         sut.HasChanged = false;
      }

      protected override void Because()
      {
         sut.AddUsedObservedData(new DataRepository {Id = "toto"});
      }

      [Observation]
      public void should_marked_the_simulation_has_having_changed()
      {
         sut.HasChanged.ShouldBeTrue();
      }
   }

   public class When_removing_some_observed_data_from_a_simulation : concern_for_IndividualSimulation
   {
      private DataRepository _dataRepository;

      protected override void Context()
      {
         base.Context();
         _dataRepository = new DataRepository {Id = "toto"};
         sut.AddUsedObservedData(_dataRepository);
         sut.HasChanged = false;
      }

      protected override void Because()
      {
         sut.RemoveUsedObservedData(_dataRepository);
      }

      [Observation]
      public void should_marked_the_simulation_has_having_changed()
      {
         sut.HasChanged.ShouldBeTrue();
      }
   }

   public class SimpleProtocolEqualityComparer : GenericEqualityComparer<SimpleProtocol>
   {

   }

   public abstract class concern_for_IndividualSimulation_compound_path_matching : concern_for_IndividualSimulation
   {
      protected override void Context()
      {
         base.Context();
         var compound = new Compound().WithName("Aspirin");
         sut.AddUsedBuildingBlock(new UsedBuildingBlock("compoundTemplate", PKSimBuildingBlockType.Compound) {BuildingBlock = compound});
      }
   }

   public class When_matching_a_parameter_path_that_contains_a_building_block_compound_name : concern_for_IndividualSimulation_compound_path_matching
   {
      [Observation]
      public void should_return_the_compound_name()
      {
         sut.CompoundNameForParameterPath("Organism|Aspirin|Lipophilicity").ShouldBeEqualTo("Aspirin");
      }
   }

   public class When_matching_a_parameter_path_that_does_not_contain_a_building_block_compound_name : concern_for_IndividualSimulation_compound_path_matching
   {
      [Observation]
      public void should_return_null()
      {
         sut.CompoundNameForParameterPath("Organism|Liver|Volume").ShouldBeNull();
      }
   }

   public class When_matching_a_parameter_path_for_an_unnamed_metabolite : concern_for_IndividualSimulation_compound_path_matching
   {
      [Observation]
      public void should_return_null_since_unnamed_metabolites_are_not_building_block_compounds()
      {
         sut.CompoundNameForParameterPath("Organism|Metabolite|SomeParam").ShouldBeNull();
      }
   }

   public class When_matching_a_parameter_path_where_compound_name_is_a_substring_but_not_a_path_element : concern_for_IndividualSimulation_compound_path_matching
   {
      [Observation]
      public void should_return_null()
      {
         sut.CompoundNameForParameterPath("Organism|AspirinDerivative|SomeParam").ShouldBeNull();
      }
   }

   public class When_matching_a_null_or_empty_parameter_path : concern_for_IndividualSimulation_compound_path_matching
   {
      [Observation]
      public void should_return_null_for_null_path()
      {
         sut.CompoundNameForParameterPath(null).ShouldBeNull();
      }

      [Observation]
      public void should_return_null_for_empty_path()
      {
         sut.CompoundNameForParameterPath(string.Empty).ShouldBeNull();
      }
   }

   public abstract class concern_for_IndividualSimulation_with_compound : concern_for_IndividualSimulation
   {
      protected Compound _compound;
      protected OverwriteParameterSet _renalImpairmentSet;
      protected OverwriteParameterSet _hepaticImpairmentSet;

      protected override void Context()
      {
         base.Context();
         _compound = new Compound().WithName("Aspirin");
         _renalImpairmentSet = new OverwriteParameterSet { Name = "RenalImpairment" };
         _hepaticImpairmentSet = new OverwriteParameterSet { Name = "HepaticImpairment" };
         sut.AddUsedBuildingBlock(new UsedBuildingBlock("compoundTemplate", PKSimBuildingBlockType.Compound) {BuildingBlock = _compound});
      }
   }

   public class When_adding_an_overwrite_parameter_set_selection_for_a_building_block_compound : concern_for_IndividualSimulation_with_compound
   {
      protected override void Because()
      {
         sut.AddOverwriteParameterSetSelection(_compound.Name, _renalImpairmentSet);
      }

      [Observation]
      public void should_store_the_selection()
      {
         sut.OverwriteParameterSetSelections.Selections.Count.ShouldBeEqualTo(1);
         sut.OverwriteParameterSetSelections.Selections[0].CompoundName.ShouldBeEqualTo(_compound.Name);
         sut.OverwriteParameterSetSelections.Selections[0].OverwriteParameterSet.ShouldBeEqualTo(_renalImpairmentSet);
      }
   }

   public class When_adding_an_overwrite_parameter_set_selection_for_a_compound_not_used_as_building_block : concern_for_IndividualSimulation_with_compound
   {
      protected override void Because()
      {
         sut.AddOverwriteParameterSetSelection("Metabolite", _renalImpairmentSet);
      }

      [Observation]
      public void should_not_store_the_selection()
      {
         sut.OverwriteParameterSetSelections.Selections.ShouldBeEmpty();
      }
   }

   public class When_removing_an_overwrite_parameter_set_selection : concern_for_IndividualSimulation_with_compound
   {
      protected override void Context()
      {
         base.Context();
         sut.AddOverwriteParameterSetSelection(_compound.Name, _renalImpairmentSet);
      }

      protected override void Because()
      {
         sut.RemoveOverwriteParameterSetSelection(_compound.Name);
      }

      [Observation]
      public void should_no_longer_contain_the_selection()
      {
         sut.OverwriteParameterSetSelections.Selections.ShouldBeEmpty();
      }
   }

   public class When_updating_an_overwrite_parameter_set_selection_for_an_existing_compound : concern_for_IndividualSimulation_with_compound
   {
      protected override void Context()
      {
         base.Context();
         sut.AddOverwriteParameterSetSelection(_compound.Name, _renalImpairmentSet);
      }

      protected override void Because()
      {
         sut.AddOverwriteParameterSetSelection(_compound.Name, _hepaticImpairmentSet);
      }

      [Observation]
      public void should_update_the_selection_without_adding_a_duplicate()
      {
         sut.OverwriteParameterSetSelections.Selections.Count.ShouldBeEqualTo(1);
         sut.OverwriteParameterSetSelections.Selections[0].OverwriteParameterSet.ShouldBeEqualTo(_hepaticImpairmentSet);
      }
   }
}