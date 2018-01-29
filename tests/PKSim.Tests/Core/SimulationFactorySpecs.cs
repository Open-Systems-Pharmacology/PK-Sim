using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_SimulationFactory : ContextSpecification<ISimulationFactory>
   {
      protected IObjectBaseFactory _objectBaseFactory;
      private OriginData _originData;
      protected Individual _individual;
      protected List<Compound> _compounds;
      protected ModelProperties _modelProperties;
      protected IndividualSimulation _individualSimulation;
      protected Population _population;
      private PopulationSimulation _populationSimulation;
      protected ISimulationBuildingBlockUpdater _simulationBuildingBlockUpdater;
      protected ISimulationModelCreator _simulationModelCreator;
      protected IObjectIdResetter _objectIdResetter;
      private ICompoundPropertiesUpdater _compoundPropertiesUpdater;
      protected ISimulationParametersUpdater _simulationParametersUpdater;
      private IModelPropertiesTask _modelPropertiesTask;
      protected ICloner _cloner;
      protected IDiagramModelFactory _diagramModelFactory;
      protected IInteractionTask _interactionTask;

      protected override void Context()
      {
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _simulationBuildingBlockUpdater = A.Fake<ISimulationBuildingBlockUpdater>();
         _simulationModelCreator = A.Fake<ISimulationModelCreator>();
         _simulationParametersUpdater = A.Fake<ISimulationParametersUpdater>();
         _modelPropertiesTask = A.Fake<IModelPropertiesTask>();
         _cloner = A.Fake<ICloner>();
         _diagramModelFactory = A.Fake<IDiagramModelFactory>();
         _interactionTask = A.Fake<IInteractionTask>();
         _individual = A.Fake<Individual>();
         _compounds = new List<Compound>();
         A.CallTo(() => _individual.BuildingBlockType).Returns(PKSimBuildingBlockType.Individual);
         _population = A.Fake<Population>();
         A.CallTo(() => _population.BuildingBlockType).Returns(PKSimBuildingBlockType.Population);
         _originData = new OriginData();
         _modelProperties = A.Fake<ModelProperties>();
         _individualSimulation = A.Fake<IndividualSimulation>();
         _populationSimulation = A.Fake<PopulationSimulation>();
         _objectIdResetter = A.Fake<IObjectIdResetter>();
         _compoundPropertiesUpdater = A.Fake<ICompoundPropertiesUpdater>();
         A.CallTo(() => _objectBaseFactory.Create<IndividualSimulation>()).Returns(_individualSimulation);
         A.CallTo(() => _objectBaseFactory.Create<PopulationSimulation>()).Returns(_populationSimulation);
         _individual.OriginData = _originData;
         A.CallTo(() => _individual.Organism).Returns(A.Fake<Organism>());
         sut = new SimulationFactory(_objectBaseFactory,
            _simulationBuildingBlockUpdater, _simulationModelCreator, _objectIdResetter, _compoundPropertiesUpdater, _simulationParametersUpdater,
            _modelPropertiesTask, _cloner, _diagramModelFactory, _interactionTask);
      }
   }

   public class When_creating_a_simulation_and_replacing_the_calculation_methods_in_a_compound : concern_for_SimulationFactory
   {
      private IndividualSimulation _templateSimulation;
      private IEnumerable<CalculationMethodWithCompoundName> _newCalculationMethods;
      private IndividualSimulation _result;
      protected CalculationMethod _replacementCalculationMethod;
      private CalculationMethod _originalCalculationMethod;

      protected override void Context()
      {
         base.Context();
         _templateSimulation = new IndividualSimulation();
         _templateSimulation.Properties = new SimulationProperties();
         _templateSimulation.ModelProperties = new ModelProperties();
         _templateSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("templateId3", PKSimBuildingBlockType.Individual) {BuildingBlock = new Individual()});

         var theCompound = new Compound {Name = "theCompound"};
         _originalCalculationMethod = new CalculationMethod();
         
         var usedBuildingBlock = new UsedBuildingBlock("templateID", PKSimBuildingBlockType.Compound) { BuildingBlock = theCompound };

         var theOtherCompound = new Compound {Name = "theOtherCompound"};
         var anotherUsedBuildingBlock = new UsedBuildingBlock("templateID2", PKSimBuildingBlockType.Compound) {BuildingBlock = theOtherCompound};
         _templateSimulation.AddUsedBuildingBlock(usedBuildingBlock);
         _templateSimulation.AddUsedBuildingBlock(anotherUsedBuildingBlock);
         _replacementCalculationMethod = new CalculationMethod();
         _newCalculationMethods = new List<CalculationMethodWithCompoundName> { new CalculationMethodWithCompoundName(_replacementCalculationMethod, "theCompound") };

         var compoundProperties = new CompoundProperties { Compound = theCompound };
         compoundProperties.CalculationMethodCache.AddCalculationMethod(_originalCalculationMethod);
         _templateSimulation.Properties.AddCompoundProperties(compoundProperties);
         compoundProperties = new CompoundProperties {Compound = theOtherCompound};
         compoundProperties.CalculationMethodCache.AddCalculationMethod(_originalCalculationMethod);
         _templateSimulation.Properties.AddCompoundProperties(compoundProperties);

         A.CallTo(() => _cloner.Clone(_templateSimulation)).Returns(_templateSimulation);
         A.CallTo(() => _objectBaseFactory.Create<IndividualSimulation>()).ReturnsLazily(() => new IndividualSimulation());
      }

      protected override void Because()
      {
         _result = sut.CreateWithCalculationMethodsFrom(_templateSimulation, _newCalculationMethods).DowncastTo<IndividualSimulation>();
      }

      [Observation]
      public void the_created_simulation_should_use_the_replacement_calculation_method_when_a_replacement_is_found()
      {
         _result.CompoundPropertiesFor("theCompound").CalculationMethodCache.ShouldContain(_replacementCalculationMethod);
         _result.CompoundPropertiesFor("theOtherCompound").CalculationMethodCache.ShouldContain(_originalCalculationMethod);
      }
   }

   public class When_the_simulation_factory_is_creating_a_simulation_for_an_individual_some_compounds_and_model_properties : concern_for_SimulationFactory
   {
      private Simulation _result;
      private IDiagramModel _reactionDiagramModel;

      protected override void Context()
      {
         base.Context();
         _reactionDiagramModel = A.Fake<IDiagramModel>();
         A.CallTo(() => _diagramModelFactory.Create()).Returns(_reactionDiagramModel);
      }

      protected override void Because()
      {
         _result = sut.CreateFrom(_individual, _compounds, _modelProperties);
      }

      [Observation]
      public void should_leverage_the_object_factory_to_create_a_new_simuation()
      {
         _result.ShouldBeEqualTo(_individualSimulation);
      }

      [Observation]
      public void should_set_the_model_properties_according()
      {
         _result.ModelProperties.ShouldBeEqualTo(_modelProperties);
      }

      [Observation]
      public void should_create_a_new_model_reaction_diagram_for_the_simulation()
      {
         _result.ReactionDiagramModel.ShouldBeEqualTo(_reactionDiagramModel);
      }

      [Observation]
      public void should_have_added_the_individual_as_a_used_building_block_simulation()
      {
         A.CallTo(() => _simulationBuildingBlockUpdater.UpdateUsedBuildingBlockInSimulationFromTemplate(_individualSimulation, _individual, PKSimBuildingBlockType.SimulationSubject)).MustHaveHappened();
      }
   }

   public class When_the_simulation_factory_is_creating_a_simulation_for_a_population_and_model_properties : concern_for_SimulationFactory
   {
      private Simulation _result;

      protected override void Context()
      {
         base.Context();
         var simulation = A.Fake<PopulationSimulation>();
         A.CallTo(() => _objectBaseFactory.Create<PopulationSimulation>()).Returns(simulation);
      }

      protected override void Because()
      {
         _result = sut.CreateFrom(_population, _compounds, _modelProperties);
      }

      [Observation]
      public void should_return_a_population_simulation()
      {
         _result.ShouldBeAnInstanceOf<PopulationSimulation>();
      }

      [Observation]
      public void should_create_the_population_building_block_in_the_simulation()
      {
         A.CallTo(() => _simulationBuildingBlockUpdater.UpdateUsedBuildingBlockInSimulationFromTemplate(_result.DowncastTo<PopulationSimulation>(), _population, PKSimBuildingBlockType.SimulationSubject)).MustHaveHappened();
      }
   }

   public class When_creating_a_simulation_based_on_an_original_individual_simulation_using_an_individual : concern_for_SimulationFactory
   {
      private Simulation _originalSimulation;
      private Simulation _result;
      private UsedBuildingBlock _bb1;
      private UsedBuildingBlock _bb2;
      private UsedBuildingBlock _bb3;

      protected override void Context()
      {
         base.Context();
         _originalSimulation = A.Fake<Simulation>();
         _bb1 = new UsedBuildingBlock("Id1", PKSimBuildingBlockType.Individual);
         _bb2 = new UsedBuildingBlock("Id2", PKSimBuildingBlockType.Compound);
         _bb3 = new UsedBuildingBlock("Id3", PKSimBuildingBlockType.Event);
         A.CallTo(() => _originalSimulation.Properties).Returns(new SimulationProperties());
         A.CallTo(() => _originalSimulation.OutputSchema).Returns(new OutputSchema());
         A.CallTo(() => _originalSimulation.UsedBuildingBlocks).Returns(new[] { _bb1, _bb2, _bb3 });
      }

      protected override void Because()
      {
         _result = sut.CreateFrom(_individual, _compounds, _modelProperties, _originalSimulation);
      }

      [Observation]
      public void should_have_updated_the_basic_properties_based_on_the_original_simulation()
      {
         A.CallTo(() => _result.UpdateFromOriginalSimulation(_originalSimulation)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_individual_used_building_block_for_the_simulation()
      {
         A.CallTo(() => _simulationBuildingBlockUpdater.UpdateUsedBuildingBlockInSimulationFromTemplate(_result, _individual, PKSimBuildingBlockType.SimulationSubject)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_compound_used_building_block_for_the_simulation()
      {
         A.CallTo(() => _simulationBuildingBlockUpdater.UpdateMultipleUsedBuildingBlockInSimulationFromTemplate(_result, _compounds, PKSimBuildingBlockType.Compound)).MustHaveHappened();
      }

      [Observation]
      public void should_have_added_the_default_used_building_block_to_the_new_simulation_from_the_original_simulation()
      {
         A.CallTo(() => _result.AddUsedBuildingBlock(_bb1)).MustHaveHappened();
         A.CallTo(() => _result.AddUsedBuildingBlock(_bb2)).MustHaveHappened();
         A.CallTo(() => _result.AddUsedBuildingBlock(_bb3)).MustHaveHappened();
      }

      [Observation]
      public void should_not_remove_the_used_building_block_representing_the_individual_as_it_will_be_exchanged_in_the_update()
      {
         A.CallTo(() => _result.RemoveUsedBuildingBlock(_bb1)).MustNotHaveHappened();
      }
   }

   public class When_creating_a_simulation_based_on_an_original_individual_simulation_using_an_population : concern_for_SimulationFactory
   {
      private Simulation _originalSimulation;
      private Simulation _result;
      private UsedBuildingBlock _bb1;
      private UsedBuildingBlock _bb2;
      private UsedBuildingBlock _bb3;

      protected override void Context()
      {
         base.Context();
         _originalSimulation = A.Fake<Simulation>();
         _bb1 = new UsedBuildingBlock("Id1", PKSimBuildingBlockType.Individual);
         _bb2 = new UsedBuildingBlock("Id2", PKSimBuildingBlockType.Compound);
         _bb3 = new UsedBuildingBlock("Id3", PKSimBuildingBlockType.Event);
         A.CallTo(() => _originalSimulation.Properties).Returns(new SimulationProperties());
         A.CallTo(() => _originalSimulation.OutputSchema).Returns(new OutputSchema());
         A.CallTo(() => _originalSimulation.UsedBuildingBlocks).Returns(new[] { _bb1, _bb2, _bb3 });
         A.CallTo(() => _originalSimulation.UsedBuildingBlockInSimulation(PKSimBuildingBlockType.SimulationSubject)).Returns(_bb1);
      }

      protected override void Because()
      {
         _result = sut.CreateFrom(_population, _compounds, _modelProperties, _originalSimulation);
      }

      [Observation]
      public void should_have_updated_the_basic_properties_based_on_the_original_simulation()
      {
         A.CallTo(() => _result.UpdateFromOriginalSimulation(_originalSimulation)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_individual_used_building_block_for_the_simulation()
      {
         A.CallTo(() => _simulationBuildingBlockUpdater.UpdateUsedBuildingBlockInSimulationFromTemplate(_result, _population, PKSimBuildingBlockType.SimulationSubject)).MustHaveHappened();
      }

      [Observation]
      public void should_have_added_the_used_building_block_the_the_new_simulation_from_the_original_simulation()
      {
         A.CallTo(() => _result.AddUsedBuildingBlock(_bb3)).MustHaveHappened();
      }
   }

   public class When_creating_a_simulation_based_on_a_mobi_model : concern_for_SimulationFactory
   {
      private IModel _model;
      private IModelCoreSimulation _modelCoreSimulation;
      private Simulation _sim;

      protected override void Context()
      {
         base.Context();
         _model = A.Fake<IModel>().WithName("Test");
         _modelCoreSimulation = A.Fake<IModelCoreSimulation>().WithName("Test");
         A.CallTo(() => _modelCoreSimulation.Model).Returns(_model);
      }

      protected override void Because()
      {
         _sim = sut.CreateBasedOn<IndividualSimulation>(_modelCoreSimulation);
      }

      [Observation]
      public void should_create_a_new_simulation_and_set_the_model()
      {
         _sim.Model.ShouldBeEqualTo(_model);
      }

      [Observation]
      public void should_set_the_name_of_the_simulation_to_the_name_of_the_model()
      {
         _sim.Name.ShouldBeEqualTo(_modelCoreSimulation.Name);
      }

      [Observation]
      public void should_have_loaded_the_reaction_building_block()
      {
         _sim.Reactions.ShouldBeEqualTo(_modelCoreSimulation.BuildConfiguration.Reactions);
      }

      [Observation]
      public void should_have_resetted_the_model_id()
      {
         A.CallTo(() => _objectIdResetter.ResetIdFor(_sim)).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_origin_to_be_from_MoBi()
      {
         _sim.Origin.ShouldBeEqualTo(Origins.MoBi);
      }
   }

   public class When_creating_a_simulation_based_on_a_pk_sim_model : concern_for_SimulationFactory
   {
      private IModel _model;
      private IModelCoreSimulation _modelCoreSimulation;
      private Simulation _sim;

      protected override void Context()
      {
         base.Context();
         _model = A.Fake<IModel>().WithName("Test");
         _modelCoreSimulation = A.Fake<IModelCoreSimulation>().WithName("Test");
         A.CallTo(() => _modelCoreSimulation.Model).Returns(_model);
         _model.Root = new Container();
         var organism = A.Fake<IContainer>().WithName(Constants.ORGANISM);
         _model.Root.Add(organism);
         var children = new List<IEntity>();
         CoreConstants.Organ.StandardOrgans.Each(org => children.Add(A.Fake<IEntity>().WithName(org)));
         A.CallTo(() => organism.Children).Returns(children);
      }

      protected override void Because()
      {
         _sim = sut.CreateBasedOn<IndividualSimulation>(_modelCoreSimulation);
      }

      [Observation]
      public void should_set_the_origin_to_be_from_PKSim()
      {
         _sim.Origin.ShouldBeEqualTo(Origins.PKSim);
      }
   }

   public class When_creating_a_simulation_for_bio_availability_calculation : concern_for_SimulationFactory
   {
      private Compound _compound;
      private Protocol _ivProtocol;
      private IndividualSimulation _originalSimulation;
      private IndividualSimulation _simulationForBioavailability;
      private Protocol _originalProtocol;
      private IndividualSimulation _clonedSimulation;

      protected override void Context()
      {
         base.Context();
         _compound = new Compound().WithId("Drug").WithName("Drug");
         _originalSimulation = new IndividualSimulation { Properties = new SimulationProperties() };

         _clonedSimulation = new IndividualSimulation { Properties = new SimulationProperties() };
         _clonedSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("Compound", PKSimBuildingBlockType.Compound) { BuildingBlock = _compound });
         _clonedSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("Individual", PKSimBuildingBlockType.Individual) { BuildingBlock = _individual });
         var compoundProperties = new CompoundProperties { Compound = _compound };

         A.CallTo(() => _cloner.Clone(_originalSimulation)).Returns(_clonedSimulation);
         _clonedSimulation.Properties.AddCompoundProperties(compoundProperties);

         _originalProtocol = new SimpleProtocol().WithName("Original").WithId("Original");
         compoundProperties.ProtocolProperties.Protocol = _originalProtocol;

         _ivProtocol = new SimpleProtocol().WithName("Iv").WithId("IV");

         var pkSimulation = new IndividualSimulation { Properties = new SimulationProperties() };
         pkSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("Compound", PKSimBuildingBlockType.Compound) { BuildingBlock = _compound });
         A.CallTo(() => _objectBaseFactory.Create<IndividualSimulation>()).Returns(pkSimulation);
         A.CallTo(() => _simulationBuildingBlockUpdater.UpdateMultipleUsedBuildingBlockInSimulationFromTemplate(A<Simulation>._,
            A<IEnumerable<IPKSimBuildingBlock>>._, PKSimBuildingBlockType.Protocol))
            .Invokes(x => x.GetArgument<Simulation>(0).AddUsedBuildingBlock(new UsedBuildingBlock("Protocol", PKSimBuildingBlockType.Protocol) { BuildingBlock = _ivProtocol }));
      }

      protected override void Because()
      {
         _simulationForBioavailability = sut.CreateForBioAvailability(_ivProtocol, _compound, _originalSimulation);
      }

      [Observation]
      public void should_swap_out_the_protocol_used_by_the_compound_with_the_iv_implementation()
      {
         _simulationForBioavailability.AllBuildingBlocks<Protocol>().Contains(_originalProtocol).ShouldBeFalse();
         _simulationForBioavailability.AllBuildingBlocks<Protocol>().Contains(_ivProtocol).ShouldBeTrue();
      }

      [Observation]
      public void should_swap_out_the_reference_to_the_protocol_in_the_compound_properties()
      {
         _simulationForBioavailability.CompoundPropertiesFor(_compound).ProtocolProperties.Protocol.ShouldBeEqualTo(_ivProtocol);
      }

      [Observation]
      public void should_create_a_new_model_for_the_simulation()
      {
         A.CallTo(() => _simulationModelCreator.CreateModelFor(_simulationForBioavailability, true, false)).MustHaveHappened();
      }

      [Observation]
      public void should_reconciliate_the_values_between_the_old_and_new_simulation()
      {
         A.CallTo(() => _simulationParametersUpdater.ReconciliateSimulationParametersBetween(_originalSimulation, _simulationForBioavailability)).MustHaveHappened();
      }
   }

   public class When_creating_a_simulation_for_ddi_ratio_calculation : concern_for_SimulationFactory
   {
      private IndividualSimulation _originalSimulation;
      private IndividualSimulation _simulationForDDI;
      private IParameter _parameter;
      private IndividualSimulation _pkSimulation;
      private IndividualSimulation _clonedSimulation;

      protected override void Context()
      {
         base.Context();
         _originalSimulation = new IndividualSimulation { Properties = new SimulationProperties() };
         _clonedSimulation = A.Fake<IndividualSimulation>();
         A.CallTo(() => _cloner.Clone(_originalSimulation)).Returns(_clonedSimulation);
         _pkSimulation = A.Fake<IndividualSimulation>().WithId("Clone");
         var interactionContainer = new Container();
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(CoreConstants.Parameters.KI);
         interactionContainer.Add(_parameter);
         A.CallTo(() => _interactionTask.AllInteractionContainers(_pkSimulation)).Returns(new[] { interactionContainer });
         A.CallTo(() => _objectBaseFactory.Create<IndividualSimulation>()).Returns(_pkSimulation);
      }

      protected override void Because()
      {
         _simulationForDDI = sut.CreateForDDIRatio(_originalSimulation);
      }

      [Observation]
      public void should_return_a_clone_of_the_simulation()
      {
         _simulationForDDI.ShouldBeEqualTo(_pkSimulation);
      }

      [Observation]
      public void should_have_created_a_clone_of_the_simulation_and_set_all_inhibitor_parameters_to_infinity()
      {
         double.IsInfinity(_parameter.Value).ShouldBeTrue();
      }

      [Observation]
      public void should_have_used_a_clone_of_the_original_individual()
      {
         A.CallTo(() => _simulationBuildingBlockUpdater.UpdateUsedBuildingBlockInSimulationFromTemplate(_pkSimulation, _clonedSimulation.Individual, PKSimBuildingBlockType.SimulationSubject)).MustHaveHappened();
      }

      [Observation]
      public void should_have_used_a_clone_of_the_original_compounds()
      {
         A.CallTo(() => _simulationBuildingBlockUpdater.UpdateMultipleUsedBuildingBlockInSimulationFromTemplate(_pkSimulation, _clonedSimulation.Compounds, PKSimBuildingBlockType.Compound)).MustHaveHappened();
      }

      [Observation]
      public void should_have_used_a_clone_of_the_original_model_properties()
      {
         _simulationForDDI.ModelProperties.ShouldBeEqualTo(_clonedSimulation.ModelProperties);
      }


      [Observation]
      public void should_reconciliate_the_values_between_the_old_and_new_simulation()
      {
         A.CallTo(() => _simulationParametersUpdater.ReconciliateSimulationParametersBetween(_originalSimulation, _simulationForDDI)).MustHaveHappened();
      }
   }

   public class When_creating_a_simulation_for_vss_calculation : concern_for_SimulationFactory
   {
      private Simulation _simulationForVSS;
      private Protocol _protocol;
      private Compound _compound;
      private Compound _vssCompound;

      protected override void Context()
      {
         base.Context();
         _protocol = new SimpleProtocol().WithName("P");
         _compound = new Compound().WithId("Drug");
         _vssCompound = new Compound().WithId("VSS");
         A.CallTo(() => _cloner.Clone(_compound)).Returns(_vssCompound);
      }

      protected override void Because()
      {
         _simulationForVSS = sut.CreateForVSS(_protocol, _individual, _compound);
      }

      [Observation]
      public void the_name_of_the_created_simulation_should_not_be_empty()
      {
         string.IsNullOrEmpty(_vssCompound.Name).ShouldBeFalse();
      }
   }
}