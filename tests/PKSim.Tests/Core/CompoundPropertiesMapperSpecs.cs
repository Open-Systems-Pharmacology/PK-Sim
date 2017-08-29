using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using Compound = PKSim.Core.Model.Compound;
using CompoundProperties = PKSim.Core.Snapshots.CompoundProperties;
using Formulation = PKSim.Core.Model.Formulation;
using Protocol = PKSim.Core.Model.Protocol;

namespace PKSim.Core
{
   public abstract class concern_for_CompoundPropertiesMapper : ContextSpecificationAsync<CompoundPropertiesMapper>
   {
      private CalculationMethodCacheMapper _calculationMethodCacheMapper;
      protected CompoundProperties _snapshot;
      protected Model.CompoundProperties _compoundProperties;
      private CompoundGroupSelection _compoungGroupSelectionOneAlternative;
      protected Compound _compound;
      protected CalculationMethodCache _calculationMethodSnapshot;
      protected EnzymaticProcessSelection _enzymaticPartialProcessSelection;
      protected ProcessSelection _specificBindingPartialProcessSelection;
      protected SystemicProcessSelection _transportSystemicProcessSelection;
      protected Protocol _protocol;
      protected Formulation _formulation;
      private ParameterAlternativeGroup _parameterAlternativeGroupWithOneAlternative;
      private ParameterAlternativeGroup _parameterAlternativeGroupWithTwoAlternatives;
      protected CompoundGroupSelection _compoungGroupSelectioTwoAlternatives;
      protected PKSimProject _project;
      private ProcessMappingMapper _processMappingMapper;
      protected CompoundProcessSelection _snapshotProcess1;
      protected CompoundProcessSelection _snapshotProcess2;
      protected CompoundProcessSelection _snapshotProcess3;
      protected EnzymaticProcess _enzymaticProcess;
      protected SpecificBindingPartialProcess _specificBindingProcess;
      protected SystemicProcess _transportProcess;

      protected override Task Context()
      {
         _calculationMethodCacheMapper = A.Fake<CalculationMethodCacheMapper>();
         _processMappingMapper = A.Fake<ProcessMappingMapper>();
         _project = new PKSimProject();
         _calculationMethodSnapshot = new CalculationMethodCache();
         sut = new CompoundPropertiesMapper(_calculationMethodCacheMapper, _processMappingMapper);

         _compoungGroupSelectionOneAlternative = new CompoundGroupSelection
         {
            AlternativeName = "ALT1",
            GroupName = "ALTERNATIVE_GROUP_1"
         };

         _compoungGroupSelectioTwoAlternatives = new CompoundGroupSelection
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

         _parameterAlternativeGroupWithOneAlternative = new ParameterAlternativeGroup {Name = _compoungGroupSelectionOneAlternative.GroupName};
         _parameterAlternativeGroupWithTwoAlternatives = new ParameterAlternativeGroup {Name = _compoungGroupSelectioTwoAlternatives.GroupName};

         _parameterAlternativeGroupWithTwoAlternatives.AddAlternative(new ParameterAlternative {Name = "ALT1"});
         _parameterAlternativeGroupWithTwoAlternatives.AddAlternative(new ParameterAlternative {Name = "ALT2"});

         _compound.AddParameterAlternativeGroup(_parameterAlternativeGroupWithOneAlternative);
         _compound.AddParameterAlternativeGroup(_parameterAlternativeGroupWithTwoAlternatives);

         _compoundProperties = new Model.CompoundProperties();

         _compoundProperties.AddCompoundGroupSelection(_compoungGroupSelectionOneAlternative);
         _compoundProperties.AddCompoundGroupSelection(_compoungGroupSelectioTwoAlternatives);
         _compoundProperties.Compound = _compound;
         _enzymaticProcess = new EnzymaticProcess {Name = "EnzymaticProcess"};
         _specificBindingProcess = new SpecificBindingPartialProcess {Name = "SpecificBinding"};
         _transportProcess = new SystemicProcess {Name = "Transport", SystemicProcessType = SystemicProcessTypes.GFR};
         _compound.AddProcess(_enzymaticProcess);
         _compound.AddProcess(_specificBindingProcess);
         _compound.AddProcess(_transportProcess);

         _enzymaticPartialProcessSelection = new EnzymaticProcessSelection {ProcessName = _enzymaticProcess.Name};
         _specificBindingPartialProcessSelection = new ProcessSelection {ProcessName = _specificBindingProcess.Name};
         _transportSystemicProcessSelection = new SystemicProcessSelection {ProcessName = _transportProcess.Name, ProcessType = _transportProcess.SystemicProcessType,};
         _compoundProperties.Processes.MetabolizationSelection.AddPartialProcessSelection(_enzymaticPartialProcessSelection);
         _compoundProperties.Processes.SpecificBindingSelection.AddPartialProcessSelection(_specificBindingPartialProcessSelection);
         _compoundProperties.Processes.TransportAndExcretionSelection.AddSystemicProcessSelection(_transportSystemicProcessSelection);

         _snapshotProcess1 = new CompoundProcessSelection();
         _snapshotProcess2 = new CompoundProcessSelection();
         _snapshotProcess3 = new CompoundProcessSelection();

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
         _snapshot.Processes.ShouldOnlyContain(_snapshotProcess1, _snapshotProcess2, _snapshotProcess3);
      }

      [Observation]
      public void should_save_alternatives_with_at_least_two_alternatives()
      {
         _snapshot.Alternatives.Length.ShouldBeEqualTo(1);
         _snapshot.Alternatives[0].AlternativeName.ShouldBeEqualTo(_compoungGroupSelectioTwoAlternatives.AlternativeName);
         _snapshot.Alternatives[0].GroupName.ShouldBeEqualTo(_compoungGroupSelectioTwoAlternatives.GroupName);
      }
   }
}