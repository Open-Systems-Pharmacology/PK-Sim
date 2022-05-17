using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using Compound = PKSim.Core.Model.Compound;
using CompoundProperties = PKSim.Core.Snapshots.CompoundProperties;
using Formulation = PKSim.Core.Model.Formulation;
using Protocol = PKSim.Core.Model.Protocol;
using Simulation = PKSim.Core.Model.Simulation;

namespace PKSim.Core
{
   public abstract class concern_for_CompoundPropertiesMapper : ContextSpecificationAsync<CompoundPropertiesMapper>
   {
      protected CalculationMethodCacheMapper _calculationMethodCacheMapper;
      protected CompoundProperties _snapshot;
      protected Model.CompoundProperties _compoundProperties;
      private CompoundGroupSelection _compoundGroupSelectionOneAlternative;
      protected Compound _compound;
      protected CalculationMethodCache _calculationMethodSnapshot;
      protected EnzymaticProcessSelection _enzymaticPartialProcessSelection;
      protected ProcessSelection _specificBindingPartialProcessSelection;
      protected SystemicProcessSelection _transportSystemicProcessSelection;
      protected Protocol _protocol;
      protected Formulation _formulation;
      protected ParameterAlternativeGroup _parameterAlternativeGroupWithOneAlternative;
      protected ParameterAlternativeGroup _parameterAlternativeGroupWithTwoAlternatives;
      protected CompoundGroupSelection _compoundGroupSelectionTwoAlternatives;
      protected PKSimProject _project;
      private ProcessMappingMapper _processMappingMapper;
      protected CompoundProcessSelection _snapshotProcess1;
      protected CompoundProcessSelection _snapshotProcess2;
      protected CompoundProcessSelection _snapshotProcess3;
      protected EnzymaticProcess _enzymaticProcess;
      protected SpecificBindingPartialProcess _specificBindingProcess;
      protected SystemicProcess _gfrTransportProcess;
      protected IOSPSuiteLogger _logger;
      protected Model.CompoundProperties _mappedCompoundProperties;
      private SystemicProcess _hepaticEnzymaticProcess;
      private SystemicProcessSelection _noEnzymaticSystemicProcessSelection;
      protected CompoundProcessSelection _snapshotProcess4;
      protected CompoundProcessSelection _snapshotProcess5;
      private EnzymaticProcessSelection _noEnzymaticPartialProcessSelection;
      private EnzymaticProcess _anotherEnzymaticProcess;
      protected SnapshotContext _baseSnapshotContext;

      protected override Task Context()
      {
         _calculationMethodCacheMapper = A.Fake<CalculationMethodCacheMapper>();
         _processMappingMapper = A.Fake<ProcessMappingMapper>();
         _logger = A.Fake<IOSPSuiteLogger>();
         _project = new PKSimProject();
         _baseSnapshotContext = new SnapshotContext(_project, ProjectVersions.Current);
         _calculationMethodSnapshot = new CalculationMethodCache();
         sut = new CompoundPropertiesMapper(_calculationMethodCacheMapper, _processMappingMapper, _logger);

         _compoundGroupSelectionOneAlternative = new CompoundGroupSelection
         {
            AlternativeName = "ALT1",
            GroupName = "ALTERNATIVE_GROUP_1"
         };

         _compoundGroupSelectionTwoAlternatives = new CompoundGroupSelection
         {
            AlternativeName = "ALT2",
            GroupName = "ALTERNATIVE_GROUP_2"
         };

         _compound = new Compound
         {
            Name = "COMP",
         };

         _protocol = new SimpleProtocol
         {
            Name = "PROTOCOL"
         };

         _parameterAlternativeGroupWithOneAlternative = new ParameterAlternativeGroup {Name = _compoundGroupSelectionOneAlternative.GroupName};
         _parameterAlternativeGroupWithTwoAlternatives = new ParameterAlternativeGroup {Name = _compoundGroupSelectionTwoAlternatives.GroupName};

         _parameterAlternativeGroupWithTwoAlternatives.AddAlternative(new ParameterAlternative {Name = "ALT1"});
         _parameterAlternativeGroupWithTwoAlternatives.AddAlternative(new ParameterAlternative {Name = "ALT2"});

         _compound.AddParameterAlternativeGroup(_parameterAlternativeGroupWithOneAlternative);
         _compound.AddParameterAlternativeGroup(_parameterAlternativeGroupWithTwoAlternatives);

         _compoundProperties = new Model.CompoundProperties();

         _compoundProperties.AddCompoundGroupSelection(_compoundGroupSelectionOneAlternative);
         _compoundProperties.AddCompoundGroupSelection(_compoundGroupSelectionTwoAlternatives);
         _compoundProperties.Compound = _compound;
         _enzymaticProcess = new EnzymaticProcess {Name = "EnzymaticProcess"};
         _anotherEnzymaticProcess = new EnzymaticProcess {Name = "AnotherEnzymaticProcess", MoleculeName = "CYP3A4"};
         _specificBindingProcess = new SpecificBindingPartialProcess {Name = "SpecificBinding"};
         _gfrTransportProcess = new SystemicProcess {Name = "Transport", SystemicProcessType = SystemicProcessTypes.GFR};
         _hepaticEnzymaticProcess = new SystemicProcess {Name = "Plasma Clearance", SystemicProcessType = SystemicProcessTypes.Hepatic};
         _compound.AddProcess(_enzymaticProcess);
         _compound.AddProcess(_specificBindingProcess);
         _compound.AddProcess(_gfrTransportProcess);
         _compound.AddProcess(_hepaticEnzymaticProcess);

         _enzymaticPartialProcessSelection = new EnzymaticProcessSelection {ProcessName = _enzymaticProcess.Name};
         _noEnzymaticSystemicProcessSelection = new SystemicProcessSelection {ProcessType = SystemicProcessTypes.Hepatic};
         _specificBindingPartialProcessSelection = new ProcessSelection {ProcessName = _specificBindingProcess.Name};
         _transportSystemicProcessSelection = new SystemicProcessSelection {ProcessName = _gfrTransportProcess.Name, ProcessType = _gfrTransportProcess.SystemicProcessType,};
         _noEnzymaticPartialProcessSelection = new EnzymaticProcessSelection {MoleculeName = _anotherEnzymaticProcess.MoleculeName};
         _compoundProperties.Processes.MetabolizationSelection.AddPartialProcessSelection(_enzymaticPartialProcessSelection);
         _compoundProperties.Processes.MetabolizationSelection.AddPartialProcessSelection(_noEnzymaticPartialProcessSelection);
         _compoundProperties.Processes.MetabolizationSelection.AddSystemicProcessSelection(_noEnzymaticSystemicProcessSelection);
         _compoundProperties.Processes.SpecificBindingSelection.AddPartialProcessSelection(_specificBindingPartialProcessSelection);
         _compoundProperties.Processes.TransportAndExcretionSelection.AddSystemicProcessSelection(_transportSystemicProcessSelection);

         _snapshotProcess1 = new CompoundProcessSelection {Name = _enzymaticPartialProcessSelection.ProcessName};
         _snapshotProcess2 = new CompoundProcessSelection {Name = _specificBindingPartialProcessSelection.ProcessName};
         _snapshotProcess3 = new CompoundProcessSelection {Name = _transportSystemicProcessSelection.ProcessName};
         _snapshotProcess4 = new CompoundProcessSelection {SystemicProcessType = _noEnzymaticSystemicProcessSelection.ProcessType.SystemicProcessTypeId.ToString()};
         _snapshotProcess5 = new CompoundProcessSelection {MoleculeName = _noEnzymaticPartialProcessSelection.MoleculeName};

         _formulation = new Formulation
         {
            Id = "123456"
         };
         _compoundProperties.ProtocolProperties.Protocol = _protocol;
         _compoundProperties.ProtocolProperties.AddFormulationMapping(new FormulationMapping
         {
            FormulationKey = "F1",
            TemplateFormulationId = _formulation.Id
         });

         _project.AddBuildingBlock(_formulation);
         A.CallTo(() => _calculationMethodCacheMapper.MapToSnapshot(_compoundProperties.CalculationMethodCache)).Returns(_calculationMethodSnapshot);
         A.CallTo(() => _processMappingMapper.MapToSnapshot(_enzymaticPartialProcessSelection)).Returns(_snapshotProcess1);
         A.CallTo(() => _processMappingMapper.MapToSnapshot(_specificBindingPartialProcessSelection)).Returns(_snapshotProcess2);
         A.CallTo(() => _processMappingMapper.MapToSnapshot(_transportSystemicProcessSelection)).Returns(_snapshotProcess3);
         A.CallTo(() => _processMappingMapper.MapToSnapshot(_noEnzymaticSystemicProcessSelection)).Returns(_snapshotProcess4);
         A.CallTo(() => _processMappingMapper.MapToSnapshot(_noEnzymaticPartialProcessSelection)).Returns(_snapshotProcess5);

         A.CallTo(() => _processMappingMapper.MapToModel(_snapshotProcess1, A<CompoundProcessSnapshotContext>.That.Matches(x => x.Process == _enzymaticProcess))).Returns(_enzymaticPartialProcessSelection);
         A.CallTo(() => _processMappingMapper.MapToModel(_snapshotProcess2, A<CompoundProcessSnapshotContext>.That.Matches(x => x.Process == _specificBindingProcess))).Returns(_specificBindingPartialProcessSelection);
         A.CallTo(() => _processMappingMapper.MapToModel(_snapshotProcess3, A<CompoundProcessSnapshotContext>.That.Matches(x => x.Process == _gfrTransportProcess))).Returns(_transportSystemicProcessSelection);
         A.CallTo(() => _processMappingMapper.MapToModel(_snapshotProcess4, A<CompoundProcessSnapshotContext>.That.Matches(x => x.Process.IsAnImplementationOf<NotSelectedSystemicProcess>()))).Returns(_noEnzymaticSystemicProcessSelection);
         A.CallTo(() => _processMappingMapper.MapToModel(_snapshotProcess5, A<CompoundProcessSnapshotContext>.That.Matches(x => x.Process.IsAnImplementationOf<EnzymaticProcess>()))).Returns(_noEnzymaticPartialProcessSelection);

         return _completed;
      }
   }

   public class When_mapping_compound_properties_to_snapshot : concern_for_CompoundPropertiesMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_compoundProperties, _project);
      }

      [Observation]
      public void should_return_a_snapshot_having_the_expected_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_compound.Name);
      }

      [Observation]
      public void should_save_the_calculation_methods_to_snapshot()
      {
         _snapshot.CalculationMethods.ShouldBeEqualTo(_calculationMethodSnapshot);
      }

      [Observation]
      public void should_save_the_protocol_selection_to_snapshot()
      {
         _snapshot.Protocol.Name.ShouldBeEqualTo(_protocol.Name);
         _snapshot.Protocol.Formulations.Length.ShouldBeEqualTo(1);
         _snapshot.Protocol.Formulations[0].Key.ShouldBeEqualTo("F1");
         _snapshot.Protocol.Formulations[0].Name.ShouldBeEqualTo(_formulation.Name);
      }

      [Observation]
      public void should_save_the_used_processes_to_snapshot()
      {
         _snapshot.Processes.ShouldOnlyContain(_snapshotProcess1, _snapshotProcess2, _snapshotProcess3, _snapshotProcess4, _snapshotProcess5);
      }

      [Observation]
      public void should_save_alternatives_with_at_least_two_alternatives()
      {
         _snapshot.Alternatives.Length.ShouldBeEqualTo(1);
         _snapshot.Alternatives[0].AlternativeName.ShouldBeEqualTo(_compoundGroupSelectionTwoAlternatives.AlternativeName);
         _snapshot.Alternatives[0].GroupName.ShouldBeEqualTo(_compoundGroupSelectionTwoAlternatives.GroupName);
      }
   }

   public class When_mapping_a_compound_property_snapshot_to_model : concern_for_CompoundPropertiesMapper
   {
      private SnapshotContextWithSimulation _context;
      private Simulation _simulation;
      private ISimulationSubject _simulationSubject;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_compoundProperties, _project);
         _simulation = A.Fake<Simulation>();
         _simulationSubject = A.Fake<ISimulationSubject>();
         A.CallTo(() => _simulationSubject.MoleculeByName("CYP3A4")).Returns(new IndividualEnzyme {Name = "CYP3A4"});
         A.CallTo(() => _simulation.BuildingBlock<ISimulationSubject>()).Returns(_simulationSubject);
         var newModelCompoundProperties = new Model.CompoundProperties {Compound = _compoundProperties.Compound};
         _compoundProperties.CompoundGroupSelections.Each(newModelCompoundProperties.AddCompoundGroupSelection);

         A.CallTo(() => _simulation.CompoundPropertiesFor(_snapshot.Name)).Returns(newModelCompoundProperties);
         _context = new SnapshotContextWithSimulation(_simulation, _baseSnapshotContext);
      }

      protected override async Task Because()
      {
         _mappedCompoundProperties = await sut.MapToModel(_snapshot, _context);
      }

      [Observation]
      public void should_map_used_processes()
      {
         _mappedCompoundProperties.Processes.AllEnabledProcesses().Count().ShouldBeEqualTo(3);
         _mappedCompoundProperties.Processes.AllProcesses().Count().ShouldBeEqualTo(5);
      }

      [Observation]
      public void should_map_calculation_method_used_in_the_snapshot()
      {
         A.CallTo(() => _calculationMethodCacheMapper.MapToModel(_snapshot.CalculationMethods,
            A<CalculationMethodCacheSnapshotContext>.That.Matches(x => x.CalculationMethodCache == _mappedCompoundProperties.CalculationMethodCache))).MustHaveHappened();
      }
   }

   public class When_mapping_a_compound_property_snapshot_to_model_with_a_snapshot_process_mapping_that_has_a_molecule_that_does_not_exist_in_the_individual : concern_for_CompoundPropertiesMapper
   {
      private SnapshotContextWithSimulation _context;
      private Simulation _simulation;
      private ISimulationSubject _simulationSubject;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_compoundProperties, _project);
         _simulation = A.Fake<Simulation>();
         _simulationSubject = A.Fake<ISimulationSubject>();
         A.CallTo(() => _simulationSubject.MoleculeByName("CYP3A4")).Returns(null);
         A.CallTo(() => _simulation.BuildingBlock<ISimulationSubject>()).Returns(_simulationSubject);
         var newModelCompoundProperties = new Model.CompoundProperties {Compound = _compoundProperties.Compound};
         _compoundProperties.CompoundGroupSelections.Each(newModelCompoundProperties.AddCompoundGroupSelection);

         A.CallTo(() => _simulation.CompoundPropertiesFor(_snapshot.Name)).Returns(newModelCompoundProperties);
         _context = new SnapshotContextWithSimulation(_simulation, _baseSnapshotContext);
      }

      protected override async Task Because()
      {
         _mappedCompoundProperties = await sut.MapToModel(_snapshot, _context);
      }

      [Observation]
      public void should_map_used_processes()
      {
         _mappedCompoundProperties.Processes.AllEnabledProcesses().Count().ShouldBeEqualTo(3);
         //Does not map the process with the molecule that does not exist
         _mappedCompoundProperties.Processes.AllProcesses().Count().ShouldBeEqualTo(4);
      }
   }

   public class When_mapping_a_compound_property_snapshot_to_model_with_a_snapshot_process_mapping_that_has_no_molecule_nor_type : concern_for_CompoundPropertiesMapper
   {
      private SnapshotContextWithSimulation _context;
      private Simulation _simulation;
      private ISimulationSubject _simulationSubject;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_compoundProperties, _project);
         // Adds an incorrect compound process selection
         _snapshot.Processes = new List<CompoundProcessSelection>(_snapshot.Processes) {new CompoundProcessSelection()}.ToArray();
         _simulation = A.Fake<Simulation>();
         _simulationSubject = A.Fake<ISimulationSubject>();
         A.CallTo(() => _simulationSubject.MoleculeByName("CYP3A4")).Returns(new IndividualEnzyme {Name = "CYP3A4"});
         A.CallTo(() => _simulation.BuildingBlock<ISimulationSubject>()).Returns(_simulationSubject);
         var newModelCompoundProperties = new Model.CompoundProperties {Compound = _compoundProperties.Compound};
         _compoundProperties.CompoundGroupSelections.Each(newModelCompoundProperties.AddCompoundGroupSelection);

         A.CallTo(() => _simulation.CompoundPropertiesFor(_snapshot.Name)).Returns(newModelCompoundProperties);
         _context = new SnapshotContextWithSimulation(_simulation, _baseSnapshotContext);
      }

      protected override async Task Because()
      {
         _mappedCompoundProperties = await sut.MapToModel(_snapshot, _context);
      }

      [Observation]
      public void should_map_used_processes()
      {
         _mappedCompoundProperties.Processes.AllEnabledProcesses().Count().ShouldBeEqualTo(3);
         _mappedCompoundProperties.Processes.AllProcesses().Count().ShouldBeEqualTo(5);
      }
   }

   public class When_mapping_a_compound_property_snapshot_to_model_and_the_parameter_group_is_not_found_in_the_compound_alternative : concern_for_CompoundPropertiesMapper
   {
      private SnapshotContextWithSimulation _context;
      private Simulation _simulation;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_compoundProperties, _project);
         _simulation = A.Fake<Simulation>();

         var newModelCompoundProperties = new Model.CompoundProperties {Compound = _compoundProperties.Compound};
         _compoundProperties.CompoundGroupSelections.Each(newModelCompoundProperties.AddCompoundGroupSelection);

         A.CallTo(() => _simulation.CompoundPropertiesFor(_snapshot.Name)).Returns(newModelCompoundProperties);
         _context = new SnapshotContextWithSimulation(_simulation, _baseSnapshotContext);

         _snapshot.Alternatives[0].GroupName = "UNKNOWN";
      }

      protected override async Task Because()
      {
         _mappedCompoundProperties = await sut.MapToModel(_snapshot, _context);
      }

      [Observation]
      public void should_warn_the_user_that_the_group_is_not_found()
      {
         A.CallTo(() => _logger.AddToLog(A<string>.That.Contains("UNKNOWN"), LogLevel.Error, A<string>._)).MustHaveHappened();
      }
   }

   public class When_mapping_a_compound_property_snapshot_to_model_and_the_parameter_group_is_not_found_in_the_compound : concern_for_CompoundPropertiesMapper
   {
      private SnapshotContextWithSimulation _context;
      private Simulation _simulation;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_compoundProperties, _project);
         _simulation = A.Fake<Simulation>();

         var newModelCompoundProperties = new Model.CompoundProperties {Compound = _compoundProperties.Compound};
         _compoundProperties.CompoundGroupSelections.Each(newModelCompoundProperties.AddCompoundGroupSelection);

         A.CallTo(() => _simulation.CompoundPropertiesFor(_snapshot.Name)).Returns(newModelCompoundProperties);
         _context = new SnapshotContextWithSimulation(_simulation, _baseSnapshotContext);

         _parameterAlternativeGroupWithTwoAlternatives.Name = "UNKNOWN";
      }

      protected override async Task Because()
      {
         _mappedCompoundProperties = await sut.MapToModel(_snapshot, _context);
      }

      [Observation]
      public void should_warn_the_user_that_the_group_is_not_found()
      {
         A.CallTo(() => _logger.AddToLog(A<string>._, LogLevel.Error, A<string>._)).MustHaveHappened();
      }
   }

   public class When_mapping_a_compound_property_snapshot_to_model_and_the_parameter_group_alternative_is_not_found_in_the_compound : concern_for_CompoundPropertiesMapper
   {
      private SnapshotContextWithSimulation _context;
      private Simulation _simulation;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_compoundProperties, _project);
         _simulation = A.Fake<Simulation>();

         var newModelCompoundProperties = new Model.CompoundProperties {Compound = _compoundProperties.Compound};
         _compoundProperties.CompoundGroupSelections.Each(newModelCompoundProperties.AddCompoundGroupSelection);

         A.CallTo(() => _simulation.CompoundPropertiesFor(_snapshot.Name)).Returns(newModelCompoundProperties);
         _context = new SnapshotContextWithSimulation(_simulation, _baseSnapshotContext);

         _snapshot.Alternatives[0].AlternativeName = "UNKNOWN";
      }

      protected override Task Because()
      {
         return sut.MapToModel(_snapshot, _context);
      }

      [Observation]
      public void should_warn_the_user_that_the_group_is_not_found()
      {
         A.CallTo(() => _logger.AddToLog(A<string>.That.Contains("UNKNOWN"), LogLevel.Error, A<string>._)).MustHaveHappened();
      }
   }
}